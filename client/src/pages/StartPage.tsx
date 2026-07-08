import { Link } from "react-router-dom";

export default function StartPage() {
  return (
    <div className="start-page">
      <h1>نظام تعداد الأسر</h1>
      <p className="lead">
        برنامج لجمع بيانات أفراد الأسرة مع دعم <strong>التعرّف الصوتي</strong>{" "}
        و<strong>التعرّف الضوئي على المستندات</strong> بالذكاء الصناعي.
      </p>
      <ol className="steps">
        <li>نبدأ ببيانات رب الأسرة + رقم الهاتف + عدد أفراد الأسرة.</li>
        <li>ثم نجمع بيانات كل فرد بناءً على العدد المُدخل.</li>
        <li>
          يمكنك إدخال البيانات يدويًا، أو نطقها صوتيًا، أو تصوير الهوية —
          والذكاء الصناعي يعبّئ الحقول ويسجّل كل عملية.
        </li>
      </ol>
      <Link className="btn-primary" to="/head">
        بدء التعداد
      </Link>
    </div>
  );
}
