import { Outlet, Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Sidebar from './Sidebar';

/**
 * Layout الرئيسي للتطبيق
 * Main Application Layout
 */
function Layout() {
  const { isAuthenticated, loading } = useAuth();

  // عرض شاشة التحميل
  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
      </div>
    );
  }

  // التوجيه لصفحة تسجيل الدخول إذا لم يكن المستخدم مسجلاً
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="app-layout">
      <Sidebar />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}

export default Layout;
