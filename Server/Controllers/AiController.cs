using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly ClaudeService _claude;

    public AiController(ClaudeService claude) => _claude = claude;

    // التعرّف الصوتي: تحليل النص المنطوق واستخراج الحقول
    [HttpPost("voice")]
    public async Task<ActionResult<AiExtractionResponse>> Voice([FromBody] VoiceRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Transcript))
            return BadRequest(new AiExtractionResponse { Error = "النص المنطوق فارغ" });

        var result = await _claude.AnalyzeVoiceAsync(req.Transcript, req.CurrentFields);
        return Ok(result);
    }

    // التعرّف الضوئي: تحليل صورة الهوية واستخراج الحقول
    [HttpPost("ocr")]
    public async Task<ActionResult<AiExtractionResponse>> Ocr([FromBody] OcrRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.ImageBase64))
            return BadRequest(new AiExtractionResponse { Error = "الصورة فارغة" });

        var result = await _claude.AnalyzeOcrAsync(req.ImageBase64, req.MediaType, req.CurrentFields);
        return Ok(result);
    }
}
