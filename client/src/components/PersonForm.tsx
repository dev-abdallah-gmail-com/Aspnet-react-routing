import { PERSON_FIELDS } from "../types";
import { personLabel, useCensus } from "../state/CensusContext";
import VoiceCapture from "./VoiceCapture";
import PhotoCapture from "./PhotoCapture";

interface Props {
  personIndex: number;
}

// نموذج بيانات فرد واحد: الحقول الثمانية + أدوات الصوت والتصوير
export default function PersonForm({ personIndex }: Props) {
  const { persons, updateField } = useCensus();
  const person = persons[personIndex];
  const isHead = person.isHead;

  return (
    <div className="person-form">
      <h3>
        بيانات {personLabel(personIndex)}
        {isHead && <span className="badge-head">رب الأسرة</span>}
      </h3>

      <div className="capture-row">
        <VoiceCapture personIndex={personIndex} />
        <PhotoCapture personIndex={personIndex} />
      </div>

      <div className="fields-grid">
        {PERSON_FIELDS.map((f) => (
          <label key={f.key} className="field">
            <span className="field-label">{f.label}</span>
            <input
              type="text"
              value={person[f.key] ?? ""}
              placeholder={f.placeholder}
              onChange={(e) =>
                updateField(personIndex, f.key, e.target.value, "manual")
              }
            />
          </label>
        ))}
      </div>
    </div>
  );
}
