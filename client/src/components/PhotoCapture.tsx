import { useEffect, useRef, useState } from "react";
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

// التعرّف الضوئي: التقاط مباشر بالكاميرا أو رفع صورة، ثم استخراج البيانات بالذكاء الصناعي
export default function PhotoCapture({ personIndex }: Props) {
  const { persons, applyExtraction } = useCensus();
  const [busy, setBusy] = useState(false);
  const [status, setStatus] = useState<string | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [cameraOn, setCameraOn] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const videoRef = useRef<HTMLVideoElement>(null);
  const streamRef = useRef<MediaStream | null>(null);

  // إيقاف الكاميرا وتحرير الموارد
  const stopCamera = () => {
    streamRef.current?.getTracks().forEach((t) => t.stop());
    streamRef.current = null;
    setCameraOn(false);
  };

  // تنظيف عند إزالة المكوّن
  useEffect(() => () => stopCamera(), []);

  // منطق مشترك: إرسال صورة base64 للتحليل وتعبئة الحقول
  const processImage = async (base64: string, mediaType: string) => {
    setBusy(true);
    setStatus("جارٍ التعرّف على صورة الهوية…");
    try {
      const res = await analyzeOcr(base64, mediaType, persons[personIndex]);
      applyExtraction(personIndex, res.fields, "ocr", res.rawText);
      if (res.error) {
        setStatus(`⚠️ ${res.error}`);
      } else {
        const filled = Object.values(res.fields).filter(
          (v) => v && v.trim(),
        ).length;
        setStatus(
          filled > 0
            ? `تم التعرّف على المستند وتعبئة ${filled} حقل.`
            : "تم التعرّف لكن لم يُستخرج أي حقل من الصورة.",
        );
      }
    } catch (err) {
      setStatus(`خطأ: ${(err as Error).message}`);
    } finally {
      setBusy(false);
    }
  };

  // فتح الكاميرا (البث الحيّ)
  const startCamera = async () => {
    setStatus(null);
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: "environment" },
        audio: false,
      });
      streamRef.current = stream;
      setCameraOn(true);
      // ربط البث بعنصر الفيديو بعد ظهوره
      requestAnimationFrame(() => {
        if (videoRef.current) {
          videoRef.current.srcObject = stream;
          videoRef.current.play().catch(() => {});
        }
      });
    } catch (err) {
      setStatus(`تعذّر فتح الكاميرا: ${(err as Error).message}`);
    }
  };

  // التقاط إطار من الكاميرا وإرساله للتحليل
  const capturePhoto = async () => {
    const video = videoRef.current;
    if (!video || !video.videoWidth) return;
    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    const dataUrl = canvas.toDataURL("image/jpeg", 0.9);
    setPreview(dataUrl);
    stopCamera();
    const base64 = dataUrl.slice(dataUrl.indexOf(",") + 1);
    await processImage(base64, "image/jpeg");
  };

  // رفع صورة من الجهاز
  const onFile = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setPreview(URL.createObjectURL(file));
    const { base64, mediaType } = await fileToBase64(file);
    await processImage(base64, mediaType);
    if (inputRef.current) inputRef.current.value = "";
  };

  return (
    <div className="capture-box">
      <strong>📷 التعرّف الضوئي (تصوير الهوية)</strong>

      {cameraOn ? (
        <div className="camera-live">
          <video
            ref={videoRef}
            className="camera-video"
            autoPlay
            playsInline
            muted
          />
          <div className="capture-actions">
            <button
              type="button"
              className="btn-primary"
              onClick={capturePhoto}
              disabled={busy}
            >
              📸 التقاط
            </button>
            <button type="button" className="btn-stop" onClick={stopCamera}>
              إغلاق الكاميرا
            </button>
          </div>
        </div>
      ) : (
        <div className="capture-actions capture-actions-col">
          <button type="button" onClick={startCamera} disabled={busy}>
            فتح الكاميرا
          </button>
          <label className="upload-label">
            أو رفع صورة:
            <input
              ref={inputRef}
              type="file"
              accept="image/*"
              capture="environment"
              onChange={onFile}
              disabled={busy}
            />
          </label>
        </div>
      )}

      {preview && !cameraOn && (
        <img className="capture-preview" src={preview} alt="صورة الهوية" />
      )}
      {status && <div className="capture-status">{status}</div>}
    </div>
  );
}
