import { useCensus } from "../state/CensusContext";
import type { LogEntry } from "../types";

const sourceIcon = (s: LogEntry["source"]) =>
  s === "voice" ? "🎤" : s === "ocr" ? "📷" : "✍️";

const opClass = (op: LogEntry["operation"]) =>
  op === "add" ? "log-add" : op === "edit" ? "log-edit" : "log-raw";

// شاشة التسجيل المشتركة: تعرض كل نص صوتي/ضوئي خام وكل عملية إضافة/تعديل
export default function LoggingPanel() {
  const { logs } = useCensus();

  return (
    <aside className="logging-panel">
      <h2>شاشة التسجيل (Logging)</h2>
      <p className="logging-hint">
        يُسجَّل هنا كل ما يُنطق أو يُتعرَّف عليه من المستندات، وكل عملية إضافة أو
        تعديل على الحقول.
      </p>
      <div className="log-list">
        {logs.length === 0 && (
          <div className="log-empty">لا توجد عمليات بعد…</div>
        )}
        {[...logs].reverse().map((log) => (
          <div key={log.id} className={`log-item ${opClass(log.operation)}`}>
            <span className="log-time">
              {new Date(log.timestamp).toLocaleTimeString("ar-EG")}
            </span>
            <span className="log-source">{sourceIcon(log.source)}</span>
            <span className="log-msg">{log.message}</span>
          </div>
        ))}
      </div>
    </aside>
  );
}
