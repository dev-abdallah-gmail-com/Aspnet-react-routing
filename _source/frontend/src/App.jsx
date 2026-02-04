import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import LoginPage from './pages/LoginPage';
import DynamicPage from './pages/DynamicPage';

/**
 * التطبيق الرئيسي - Main Application
 *
 * نظام التوجيه (Routing):
 * ───────────────────────
 *
 * 1. /login - صفحة تسجيل الدخول
 *
 * 2. /:role/:controller/:action - الصفحات الديناميكية
 *    مثال: /admin/dashboard/index
 *           /editor/products/list
 *           /user/orders/details
 *
 * كيف يعمل النظام:
 * ────────────────
 * 1. المستخدم يسجل دخول ويحصل على JWT token
 * 2. النظام يجلب القائمة المتاحة بناءً على الدور
 * 3. عند الضغط على أي رابط:
 *    - يتم التحقق من الصلاحية
 *    - يتم استدعاء API: GET /api/{controller}/{action}
 *    - يتم عرض البيانات مع الـ URL: /{role}/{controller}/{action}
 */
function App() {
  const { isAuthenticated, user, loading } = useAuth();

  // عرض شاشة التحميل
  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <Routes>
      {/* صفحة تسجيل الدخول */}
      <Route
        path="/login"
        element={
          isAuthenticated ? (
            <Navigate to={`/${user.role}/dashboard/index`} replace />
          ) : (
            <LoginPage />
          )
        }
      />

      {/* الصفحات المحمية - تتطلب تسجيل دخول */}
      <Route element={<Layout />}>
        {/*
          Route ديناميكي يقبل أي role/controller/action
          Dynamic route that accepts any role/controller/action
        */}
        <Route path="/:role/:controller/:action" element={<DynamicPage />} />
      </Route>

      {/* الصفحة الرئيسية - توجيه للوحة التحكم أو تسجيل الدخول */}
      <Route
        path="/"
        element={
          isAuthenticated ? (
            <Navigate to={`/${user.role}/dashboard/index`} replace />
          ) : (
            <Navigate to="/login" replace />
          )
        }
      />

      {/* أي مسار آخر - 404 */}
      <Route
        path="*"
        element={
          <div style={{ padding: '50px', textAlign: 'center' }}>
            <h1>404</h1>
            <p>الصفحة غير موجودة</p>
          </div>
        }
      />
    </Routes>
  );
}

export default App;
