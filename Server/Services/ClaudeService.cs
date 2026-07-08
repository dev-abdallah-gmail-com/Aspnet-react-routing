using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using Server.Models;

namespace Server.Services;

// خدمة استدعاء Claude لتحليل النص الصوتي واستخراج الحقول + التعرّف الضوئي (OCR)
public class ClaudeService
{
    private readonly AnthropicClient? _client;
    private readonly ILogger<ClaudeService> _logger;

    // مفاتيح الحقول الثمانية بالعربية لتوجيه النموذج
    private const string FieldGuide =
        "name = الاسم، birthDate = تاريخ الميلاد، gender = النوع، occupation = الوظيفة، " +
        "nationality = الجنسية، idNumber = رقم الهوية، idExpiryDate = تاريخ انتهاء صلاحية الهوية، " +
        "relationship = علاقته بالأسرة.";

    public ClaudeService(ILogger<ClaudeService> logger)
    {
        _logger = logger;
        var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _client = new AnthropicClient { ApiKey = apiKey };
        }
    }

    public bool IsConfigured => _client is not null;

    // المخطّط الصارم (Structured Outputs) المشترك بين voice و ocr
    private static Dictionary<string, JsonElement> BuildSchema()
    {
        object StringProp() => new { type = "string" };
        var properties = new
        {
            rawText = StringProp(),
            name = StringProp(),
            birthDate = StringProp(),
            gender = StringProp(),
            occupation = StringProp(),
            nationality = StringProp(),
            idNumber = StringProp(),
            idExpiryDate = StringProp(),
            relationship = StringProp(),
        };
        var required = new[]
        {
            "rawText", "name", "birthDate", "gender", "occupation",
            "nationality", "idNumber", "idExpiryDate", "relationship",
        };
        return new Dictionary<string, JsonElement>
        {
            ["type"] = JsonSerializer.SerializeToElement("object"),
            ["properties"] = JsonSerializer.SerializeToElement(properties),
            ["required"] = JsonSerializer.SerializeToElement(required),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false),
        };
    }

    private static string CurrentFieldsJson(PersonFieldsDto? f)
    {
        f ??= new PersonFieldsDto();
        return JsonSerializer.Serialize(new
        {
            name = f.Name ?? "",
            birthDate = f.BirthDate ?? "",
            gender = f.Gender ?? "",
            occupation = f.Occupation ?? "",
            nationality = f.Nationality ?? "",
            idNumber = f.IdNumber ?? "",
            idExpiryDate = f.IdExpiryDate ?? "",
            relationship = f.Relationship ?? "",
        });
    }

    // التعرّف الصوتي: استخراج الحقول من نص منطوق بالعربية
    public async Task<AiExtractionResponse> AnalyzeVoiceAsync(string transcript, PersonFieldsDto? current)
    {
        if (_client is null)
            return NotConfigured(transcript);

        var system =
            "أنت مساعد لتعبئة بيانات تعداد سكاني بالعربية. استخرج حقول الفرد من النص المنطوق. " +
            $"الحقول: {FieldGuide} " +
            "أعِد كل حقل تعرّفت عليه فقط، واترك الباقي سلسلة فارغة \"\". " +
            "لديك قيم حالية للحقول؛ إذا ذكر النص قيمة جديدة لحقل موجود فاستبدلها بالقيمة الجديدة. " +
            "ضع في rawText نصًا موجزًا لما فهمته من الكلام. أعِد النتيجة حسب المخطّط JSON فقط.";

        var userText =
            $"القيم الحالية: {CurrentFieldsJson(current)}\n\nالنص المنطوق: {transcript}";

        try
        {
            var response = await _client.Messages.Create(new MessageCreateParams
            {
                Model = Model.ClaudeOpus4_8,
                MaxTokens = 1024,
                System = system,
                OutputConfig = new OutputConfig
                {
                    Format = new JsonOutputFormat { Schema = BuildSchema() },
                },
                Messages = [new() { Role = Role.User, Content = userText }],
            });

            var parsed = ParseResponse(response);
            if (string.IsNullOrWhiteSpace(parsed.RawText))
                parsed.RawText = transcript;
            return parsed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل تحليل الصوت عبر Claude");
            return new AiExtractionResponse { RawText = transcript, Error = ex.Message };
        }
    }

    // التعرّف الضوئي: قراءة صورة الهوية واستخراج الحقول
    public async Task<AiExtractionResponse> AnalyzeOcrAsync(string imageBase64, string mediaType, PersonFieldsDto? current)
    {
        if (_client is null)
            return NotConfigured("");

        var system =
            "أنت مساعد لتعبئة بيانات تعداد سكاني بالعربية عبر التعرّف الضوئي على مستندات الهوية. " +
            $"اقرأ كل النص الظاهر في الصورة، ثم استخرج حقول الفرد. الحقول: {FieldGuide} " +
            "أعِد كل حقل تعرّفت عليه فقط، واترك الباقي سلسلة فارغة \"\". " +
            "لديك قيم حالية؛ إذا أظهرت الصورة قيمة أوضح لحقل موجود فاستبدلها. " +
            "ضع في rawText كل النص الذي تعرّفت عليه من المستند. أعِد النتيجة حسب المخطّط JSON فقط.";

        var userText = $"القيم الحالية: {CurrentFieldsJson(current)}\n\nحلّل صورة الهوية المرفقة.";

        try
        {
            var response = await _client.Messages.Create(new MessageCreateParams
            {
                Model = Model.ClaudeOpus4_8,
                MaxTokens = 1024,
                System = system,
                OutputConfig = new OutputConfig
                {
                    Format = new JsonOutputFormat { Schema = BuildSchema() },
                },
                Messages =
                [
                    new()
                    {
                        Role = Role.User,
                        Content = new List<ContentBlockParam>
                        {
                            new ImageBlockParam
                            {
                                Source = new Base64ImageSource
                                {
                                    Data = imageBase64,
                                    MediaType = MapMediaType(mediaType),
                                },
                            },
                            new TextBlockParam { Text = userText },
                        },
                    },
                ],
            });

            return ParseResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل التعرّف الضوئي عبر Claude");
            return new AiExtractionResponse { Error = ex.Message };
        }
    }

    private static MediaType MapMediaType(string mediaType) => mediaType.ToLowerInvariant() switch
    {
        "image/png" => MediaType.ImagePng,
        "image/gif" => MediaType.ImageGif,
        "image/webp" => MediaType.ImageWebP,
        _ => MediaType.ImageJpeg,
    };

    // يستخرج نص الاستجابة ويحوّله إلى الحقول
    private static AiExtractionResponse ParseResponse(Message response)
    {
        var text = response.Content
            .Select(b => b.Value)
            .OfType<TextBlock>()
            .Select(t => t.Text)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(text))
            return new AiExtractionResponse { Error = "استجابة فارغة من النموذج" };

        try
        {
            var raw = JsonSerializer.Deserialize<ExtractionModel>(text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (raw is null)
                return new AiExtractionResponse { Error = "تعذّر تحليل استجابة النموذج" };

            return new AiExtractionResponse
            {
                RawText = raw.RawText ?? "",
                Fields = new PersonFieldsDto
                {
                    Name = raw.Name,
                    BirthDate = raw.BirthDate,
                    Gender = raw.Gender,
                    Occupation = raw.Occupation,
                    Nationality = raw.Nationality,
                    IdNumber = raw.IdNumber,
                    IdExpiryDate = raw.IdExpiryDate,
                    Relationship = raw.Relationship,
                },
            };
        }
        catch (JsonException)
        {
            return new AiExtractionResponse { RawText = text, Error = "تنسيق استجابة غير متوقع" };
        }
    }

    private static AiExtractionResponse NotConfigured(string rawText) => new()
    {
        RawText = rawText,
        Error = "لم يتم ضبط مفتاح ANTHROPIC_API_KEY في بيئة الخادم. يمكنك الإدخال يدويًا.",
    };

    private sealed class ExtractionModel
    {
        public string? RawText { get; set; }
        public string? Name { get; set; }
        public string? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public string? Nationality { get; set; }
        public string? IdNumber { get; set; }
        public string? IdExpiryDate { get; set; }
        public string? Relationship { get; set; }
    }
}
