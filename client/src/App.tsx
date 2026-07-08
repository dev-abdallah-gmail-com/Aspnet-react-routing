import { Outlet, Route, Routes, Link, useLocation } from "react-router-dom";
import LoggingPanel from "./components/LoggingPanel";
import StartPage from "./pages/StartPage";
import HouseholdHeadPage from "./pages/HouseholdHeadPage";
import MemberPage from "./pages/MemberPage";
import ReviewPage from "./pages/ReviewPage";

// التخطيط العام: ترويسة + منطقة العمل + شاشة التسجيل الجانبية
function Layout() {
  const location = useLocation();
  const onStart = location.pathname === "/";
  return (
    <div className="app-shell">
      <header className="app-header">
        <Link to="/" className="app-title">
          🗂️ نظام تعداد الأسر
        </Link>
      </header>
      <div className={`app-body ${onStart ? "no-panel" : ""}`}>
        <main className="app-main">
          <Outlet />
        </main>
        {!onStart && <LoggingPanel />}
      </div>
    </div>
  );
}

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<StartPage />} />
        <Route path="/head" element={<HouseholdHeadPage />} />
        <Route path="/member/:index" element={<MemberPage />} />
        <Route path="/review" element={<ReviewPage />} />
      </Route>
    </Routes>
  );
}
