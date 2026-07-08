import { useRef, useState } from "react";
import { analyzeOcr } from "../api/client";
import { useCensus } from "../state/CensusContext";

interface Props {
  personIndex: number;
}

// يحوّل ملفًا إلى base64 (بدون البادئة data:) + نوع المحتوى
function fileToBase64(
  file: File,
): Promise<{ base64: string; mediaType: string }> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      const comma = result.indexOf(",");
      resolve({ base64: result.slice(comma + 1), mediaType: file.type });
    };
    reader.onerror = () => reject(new Error("تعذّر قراءة الصورة"));
    reader.readAsDataURL(file);
  });
}

// زر التعرّف الضوئي: تصوير/رفع صورة الهوية ثم استخراج البيانات بالذكاء الصناعي
export default function PhotoCapture({ personIndex }: Props) {
  const { persons, applyExtraction } = useCensus();
  const [busy, setBusy] = useState(false);
  const [status, setStatus] = useState<string | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const onFile = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setBusy(true);
    setStatus("جارٍ التعرّف على صورة الهوية…");
    setPreview(URL.createObjectURL(file));
    try {
      const { base64, mediaType } = await fileToBase64(file);
      const res = await analyzeOcr(base64, mediaType, persons[personIndex]);
      applyExtraction(personIndex, res.fields, "ocr", res.rawText);
      setStatus("تم التعرّف على المستند وتعبئة الحقول.");
    } catch (err) {
      setStatus(`خطأ: ${(err as Error).message}`);
    } finally {
      setBusy(false);
      if (inputRef.current) inputRef.current.value = "";
    }
  };

  return (
    <div className="capture-box">
      <strong>📷 التعرّف الضوئي (تصوير الهوية)</strong>
      <div className="capture-actions">
        <input
          ref={inputRef}
          type="file"
          accept="image/*"
          capture="environment"
          onChange={onFile}
          disabled={busy}
        />
      </div>
      {preview && (
        <img className="capture-preview" src={preview} alt="صورة الهوية" />
      )}
      {status && <div className="capture-status">{status}</div>}
    </div>
  );
}
