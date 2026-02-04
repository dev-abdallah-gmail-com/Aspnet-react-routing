import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/api';

/**
 * AuthContext - سياق المصادقة
 * يدير حالة تسجيل الدخول والمستخدم الحالي
 */
const AuthContext = createContext(null);

/**
 * AuthProvider - مزود سياق المصادقة
 */
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [menu, setMenu] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // التحقق من وجود جلسة مسجلة عند التحميل
  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('token');
      const savedUser = localStorage.getItem('user');

      if (token && savedUser) {
        try {
          setUser(JSON.parse(savedUser));
          const menuData = await authService.getMenu();
          setMenu(menuData);
        } catch (err) {
          // التوكن غير صالح
          localStorage.removeItem('token');
          localStorage.removeItem('user');
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  /**
   * تسجيل الدخول
   * Login function
   */
  const login = async (username, password) => {
    setError(null);
    setLoading(true);

    try {
      const response = await authService.login(username, password);

      if (response.success) {
        // حفظ التوكن والمستخدم
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        setUser(response.user);

        // جلب القائمة المتاحة للمستخدم
        const menuData = await authService.getMenu();
        setMenu(menuData);

        return { success: true, user: response.user };
      } else {
        setError(response.message);
        return { success: false, message: response.message };
      }
    } catch (err) {
      const message = err.response?.data?.message || 'خطأ في تسجيل الدخول';
      setError(message);
      return { success: false, message };
    } finally {
      setLoading(false);
    }
  };

  /**
   * تسجيل الخروج
   * Logout function
   */
  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    setMenu([]);
  };

  /**
   * التحقق من صلاحية الوصول
   * Check access permission
   */
  const hasAccess = (controller) => {
    return menu.some(
      (item) => item.controller.toLowerCase() === controller.toLowerCase()
    );
  };

  /**
   * الحصول على URL الصحيح بناءً على الدور
   * Get correct URL based on user role
   */
  const getRouteUrl = (controller, action) => {
    if (!user) return '/login';
    return `/${user.role}/${controller.toLowerCase()}/${action.toLowerCase()}`;
  };

  const value = {
    user,
    menu,
    loading,
    error,
    login,
    logout,
    hasAccess,
    getRouteUrl,
    isAuthenticated: !!user,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

/**
 * Hook للوصول لسياق المصادقة
 * Custom hook to access auth context
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

export default AuthContext;
