import type { AiExtractionResponse, PersonFields } from "../types";

// نداءات الذكاء الصناعي وحفظ البيانات — تمر عبر بروكسي Vite إلى /api

async function postJson<T>(url: string, body: unknown): Promise<T> {
  const res = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  const data = (await res.json().catch(() => ({}))) as T;
  if (!res.ok) {
    const message =
      (data as { error?: string })?.error ?? `فشل الطلب (${res.status})`;
    throw new Error(message);
  }
  return data;
}

// التعرّف الصوتي: يُرسل النص المنطوق + القيم الحالية، ويعيد الحقول المستخرجة
export function analyzeVoice(
  transcript: string,
  currentFields: PersonFields,
): Promise<AiExtractionResponse> {
  return postJson<AiExtractionResponse>("/api/ai/voice", {
    transcript,
    currentFields,
  });
}

// التعرّف الضوئي: يُرسل صورة الهوية (base64) + القيم الحالية
export function analyzeOcr(
  imageBase64: string,
  mediaType: string,
  currentFields: PersonFields,
): Promise<AiExtractionResponse> {
  return postJson<AiExtractionResponse>("/api/ai/ocr", {
    imageBase64,
    mediaType,
    currentFields,
  });
}

export interface SaveHouseholdPayload {
  phoneNumber: string;
  memberCount: number;
  persons: Array<Record<string, unknown>>;
  logs: Array<Record<string, unknown>>;
}

export function saveHousehold(
  payload: SaveHouseholdPayload,
): Promise<{ id: number }> {
  return postJson<{ id: number }>("/api/households", payload);
}
