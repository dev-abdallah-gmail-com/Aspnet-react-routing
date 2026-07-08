import { useNavigate } from "react-router-dom";
import PersonForm from "../components/PersonForm";
import { useCensus } from "../state/CensusContext";

// بيانات رب الأسرة + السؤالان: رقم الهاتف وعدد أفراد الأسرة
export default function HouseholdHeadPage() {
  const { phoneNumber, memberCount, setPhone, setMemberCount } = useCensus();
  const navigate = useNavigate();

  const goNext = () => {
    // إذا كان هناك أفراد آخرون غير رب الأسرة ننتقل لأول فرد، وإلا للمراجعة
    if (memberCount > 1) navigate("/member/1");
    else navigate("/review");
  };

  return (
    <div className="page">
      <div className="household-questions">
        <label className="field">
          <span className="field-label">رقم الهاتف</span>
          <input
            type="tel"
            value={phoneNumber}
            placeholder="رقم هاتف الأسرة"
            onChange={(e) => setPhone(e.target.value)}
          />
        </label>
        <label className="field">
          <span className="field-label">عدد أفراد الأسرة (شامل رب الأسرة)</span>
          <input
            type="number"
            min={1}
            value={memberCount}
            onChange={(e) => setMemberCount(Number(e.target.value))}
          />
        </label>
      </div>

      <PersonForm personIndex={0} />

      <div className="page-nav">
        <button type="button" className="btn-primary" onClick={goNext}>
          التالي ←
        </button>
      </div>
    </div>
  );
}
