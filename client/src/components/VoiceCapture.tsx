import { useState } from "react";
import { analyzeVoice } from "../api/client";
import { useSpeech } from "../hooks/useSpeech";
import { useCensus } from "../state/CensusContext";

interface Props {
  personIndex: number;
}

// زر التعرّف الصوتي: يحوّل الكلام إلى نص عبر المتصفح ثم يحلّله بالذكاء الصناعي
export default function VoiceCapture({ personIndex }: Props) {
  const { persons, applyExtraction } = useCensus();
  const [busy, setBusy] = useState(false);
  const [status, setStatus] = useState<string | null>(null);

  const handleFinal = async (text: string) => {
    setBusy(true);
    setStatus(`جارٍ تحليل: "${text}"`);
    try {
      const res = await analyzeVoice(text, persons[personIndex]);
      // النص الخام = ما نطقه المستخدم؛ الحقول = ما استخرجه الذكاء الصناعي
      applyExtraction(personIndex, res.fields, "voice", res.rawText || text);
      setStatus("تم تحليل الكلام وتعبئة الحقول.");
    } catch (e) {
      setStatus(`خطأ: ${(e as Error).message}`);
    } finally {
      setBusy(false);
    }
  };

  const { supported, listening, interim, error, start, stop } = useSpeech({
    onFinal: handleFinal,
  });

  if (!supported) {
    return (
      <div className="capture-box">
        <strong>🎤 التعرّف الصوتي</strong>
        <p className="capture-warn">
          هذا المتصفح لا يدعم التعرّف الصوتي (Web Speech API). استخدم Chrome أو
          Edge، أو أدخل البيانات يدويًا.
        </p>
      </div>
    );
  }

  return (
    <div className="capture-box">
      <strong>🎤 التعرّف الصوتي</strong>
      <div className="capture-actions">
        {!listening ? (
          <button type="button" onClick={start} disabled={busy}>
            بدء الاستماع
          </button>
        ) : (
          <button type="button" className="btn-stop" onClick={stop}>
            إيقاف الاستماع
          </button>
        )}
      </div>
      {listening && <div className="capture-live">🔴 يستمع… {interim}</div>}
      {status && <div className="capture-status">{status}</div>}
      {error && <div className="capture-warn">تعذّر الاستماع: {error}</div>}
    </div>
  );
}
