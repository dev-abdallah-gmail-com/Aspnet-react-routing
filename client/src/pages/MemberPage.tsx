import { useNavigate, useParams } from "react-router-dom";
import PersonForm from "../components/PersonForm";
import { useCensus } from "../state/CensusContext";

// صفحة بيانات فرد من أفراد الأسرة (index = 1..memberCount-1)
export default function MemberPage() {
  const { index } = useParams();
  const navigate = useNavigate();
  const { memberCount } = useCensus();

  const idx = Number(index);
  if (!Number.isInteger(idx) || idx < 1 || idx >= memberCount) {
    return (
      <div className="page">
        <p>فرد غير موجود.</p>
        <button className="btn-primary" onClick={() => navigate("/head")}>
          العودة لرب الأسرة
        </button>
      </div>
    );
  }

  const isLast = idx === memberCount - 1;
  const goPrev = () =>
    navigate(idx > 1 ? `/member/${idx - 1}` : "/head");
  const goNext = () =>
    navigate(isLast ? "/review" : `/member/${idx + 1}`);

  return (
    <div className="page">
      <div className="progress-badge">
        الفرد {idx} من {memberCount - 1}
      </div>

      {/* key يضمن إعادة تركيب النموذج عند تغيّر الفرد */}
      <PersonForm key={idx} personIndex={idx} />

      <div className="page-nav">
        <button type="button" onClick={goPrev}>
          → السابق
        </button>
        <button type="button" className="btn-primary" onClick={goNext}>
          {isLast ? "المراجعة والحفظ" : "التالي ←"}
        </button>
      </div>
    </div>
  );
}
