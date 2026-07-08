import { useEffect, useRef, useState } from "react";

// أنواع مبسطة لواجهة Web Speech API (غير معرّفة في lib.dom القياسية)
interface SpeechRecognitionResultLike {
  0: { transcript: string };
  isFinal: boolean;
}
interface SpeechRecognitionEventLike {
  resultIndex: number;
  results: { length: number; [i: number]: SpeechRecognitionResultLike };
}
interface SpeechRecognitionLike {
  lang: string;
  continuous: boolean;
  interimResults: boolean;
  start: () => void;
  stop: () => void;
  onresult: ((e: SpeechRecognitionEventLike) => void) | null;
  onerror: ((e: { error: string }) => void) | null;
  onend: (() => void) | null;
}

function getRecognitionCtor(): (new () => SpeechRecognitionLike) | null {
  const w = window as unknown as {
    SpeechRecognition?: new () => SpeechRecognitionLike;
    webkitSpeechRecognition?: new () => SpeechRecognitionLike;
  };
  return w.SpeechRecognition ?? w.webkitSpeechRecognition ?? null;
}

interface UseSpeechOptions {
  lang?: string;
  // يُستدعى عند اكتمال جملة (نص نهائي)
  onFinal: (text: string) => void;
}

export function useSpeech({ lang = "ar-SA", onFinal }: UseSpeechOptions) {
  const [supported, setSupported] = useState(false);
  const [listening, setListening] = useState(false);
  const [interim, setInterim] = useState("");
  const [error, setError] = useState<string | null>(null);
  const recRef = useRef<SpeechRecognitionLike | null>(null);
  const onFinalRef = useRef(onFinal);
  onFinalRef.current = onFinal;

  useEffect(() => {
    const Ctor = getRecognitionCtor();
    if (!Ctor) {
      setSupported(false);
      return;
    }
    setSupported(true);
    const rec = new Ctor();
    rec.lang = lang;
    rec.continuous = true;
    rec.interimResults = true;

    rec.onresult = (e) => {
      let interimText = "";
      for (let i = e.resultIndex; i < e.results.length; i++) {
        const r = e.results[i];
        const text = r[0].transcript;
        if (r.isFinal) {
          const trimmed = text.trim();
          if (trimmed) onFinalRef.current(trimmed);
        } else {
          interimText += text;
        }
      }
      setInterim(interimText);
    };
    rec.onerror = (e) => {
      setError(e.error);
      setListening(false);
    };
    rec.onend = () => {
      setListening(false);
      setInterim("");
    };

    recRef.current = rec;
    return () => {
      try {
        rec.stop();
      } catch {
        /* تجاهل */
      }
    };
  }, [lang]);

  const start = () => {
    setError(null);
    try {
      recRef.current?.start();
      setListening(true);
    } catch {
      /* قد يُطلق خطأ إذا كان يعمل بالفعل */
    }
  };
  const stop = () => {
    recRef.current?.stop();
    setListening(false);
  };

  return { supported, listening, interim, error, start, stop };
}
