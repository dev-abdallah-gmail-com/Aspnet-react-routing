import {
  createContext,
  useContext,
  useReducer,
  type ReactNode,
} from "react";
import {
  PERSON_FIELDS,
  type LogEntry,
  type LogSource,
  type Person,
  type PersonFieldKey,
  type PersonFields,
} from "../types";

interface CensusState {
  phoneNumber: string;
  memberCount: number; // إجمالي أفراد الأسرة شامل رب الأسرة
  persons: Person[]; // persons[0] دائمًا رب الأسرة (isHead = true)
  logs: LogEntry[];
}

const fieldLabel = (key: PersonFieldKey): string =>
  PERSON_FIELDS.find((f) => f.key === key)?.label ?? key;

const personLabel = (index: number): string =>
  index === 0 ? "رب الأسرة" : `الفرد ${index}`;

const now = () => new Date().toISOString();
const uid = () =>
  typeof crypto !== "undefined" && "randomUUID" in crypto
    ? crypto.randomUUID()
    : Math.random().toString(36).slice(2);

function emptyPerson(isHead: boolean): Person {
  return {
    isHead,
    relationship: isHead ? "رب الأسرة" : "",
  };
}

const initialState: CensusState = {
  phoneNumber: "",
  memberCount: 1,
  persons: [emptyPerson(true)],
  logs: [],
};

type Action =
  | { type: "SET_PHONE"; value: string }
  | { type: "SET_MEMBER_COUNT"; value: number }
  | {
      type: "UPDATE_FIELD";
      index: number;
      field: PersonFieldKey;
      value: string;
      source: LogSource;
    }
  | {
      type: "APPLY_EXTRACTION";
      index: number;
      fields: PersonFields;
      source: LogSource;
      rawText: string;
    }
  | { type: "RESET" };

// تُنشئ سطر تسجيل لعملية تغيير حقل (إضافة أو تعديل)
function changeLog(
  index: number,
  field: PersonFieldKey,
  oldValue: string,
  newValue: string,
  source: LogSource,
): LogEntry {
  const op = oldValue.trim() ? "edit" : "add";
  const label = personLabel(index);
  const fLabel = fieldLabel(field);
  return {
    id: uid(),
    timestamp: now(),
    source,
    operation: op,
    personLabel: label,
    field,
    fieldLabel: fLabel,
    oldValue,
    newValue,
    message:
      op === "add"
        ? `[${label}] إضافة «${fLabel}» = ${newValue}`
        : `[${label}] تعديل «${fLabel}»: ${oldValue} ← ${newValue}`,
  };
}

function reducer(state: CensusState, action: Action): CensusState {
  switch (action.type) {
    case "SET_PHONE":
      return { ...state, phoneNumber: action.value };

    case "SET_MEMBER_COUNT": {
      const total = Math.max(1, Math.floor(action.value) || 1);
      const persons = [...state.persons];
      if (total > persons.length) {
        while (persons.length < total) persons.push(emptyPerson(false));
      } else if (total < persons.length) {
        persons.length = total; // الاحتفاظ برب الأسرة والأوائل
      }
      return { ...state, memberCount: total, persons };
    }

    case "UPDATE_FIELD": {
      const { index, field, value, source } = action;
      const persons = [...state.persons];
      const person = { ...persons[index] };
      const oldValue = (person[field] ?? "").toString();
      if (oldValue === value) return state;
      person[field] = value;
      persons[index] = person;
      const logs =
        value.trim() || oldValue.trim()
          ? [...state.logs, changeLog(index, field, oldValue, value, source)]
          : state.logs;
      return { ...state, persons, logs };
    }

    case "APPLY_EXTRACTION": {
      const { index, fields, source, rawText } = action;
      const persons = [...state.persons];
      const person = { ...persons[index] };
      const newLogs: LogEntry[] = [];
      const label = personLabel(index);

      // أولًا: تسجيل النص الخام المُتعرَّف عليه (صوتي أو ضوئي)
      if (rawText.trim()) {
        newLogs.push({
          id: uid(),
          timestamp: now(),
          source,
          operation: "raw",
          personLabel: label,
          rawText,
          message:
            source === "voice"
              ? `[${label}] 🎤 نص منطوق: ${rawText}`
              : `[${label}] 📷 نص مُتعرَّف عليه: ${rawText}`,
        });
      }

      // ثم: تطبيق كل حقل غير فارغ مختلف عن القيمة الحالية + سطر تسجيل لكل تغيير
      (Object.keys(fields) as PersonFieldKey[]).forEach((field) => {
        const newValue = (fields[field] ?? "").toString().trim();
        if (!newValue) return;
        const oldValue = (person[field] ?? "").toString();
        if (oldValue === newValue) return;
        person[field] = newValue;
        newLogs.push(changeLog(index, field, oldValue, newValue, source));
      });

      persons[index] = person;
      return { ...state, persons, logs: [...state.logs, ...newLogs] };
    }

    case "RESET":
      return { ...initialState, persons: [emptyPerson(true)] };

    default:
      return state;
  }
}

interface CensusContextValue extends CensusState {
  setPhone: (value: string) => void;
  setMemberCount: (value: number) => void;
  updateField: (
    index: number,
    field: PersonFieldKey,
    value: string,
    source?: LogSource,
  ) => void;
  applyExtraction: (
    index: number,
    fields: PersonFields,
    source: LogSource,
    rawText: string,
  ) => void;
  reset: () => void;
}

const CensusContext = createContext<CensusContextValue | null>(null);

export function CensusProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(reducer, initialState);

  const value: CensusContextValue = {
    ...state,
    setPhone: (v) => dispatch({ type: "SET_PHONE", value: v }),
    setMemberCount: (v) => dispatch({ type: "SET_MEMBER_COUNT", value: v }),
    updateField: (index, field, v, source = "manual") =>
      dispatch({ type: "UPDATE_FIELD", index, field, value: v, source }),
    applyExtraction: (index, fields, source, rawText) =>
      dispatch({ type: "APPLY_EXTRACTION", index, fields, source, rawText }),
    reset: () => dispatch({ type: "RESET" }),
  };

  return (
    <CensusContext.Provider value={value}>{children}</CensusContext.Provider>
  );
}

export function useCensus(): CensusContextValue {
  const ctx = useContext(CensusContext);
  if (!ctx) throw new Error("useCensus must be used within CensusProvider");
  return ctx;
}

export { personLabel };
