import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

/**
 * Hook للتنقل بناءً على الدور
 * Custom hook for role-based navigation
 *
 * الاستخدام:
 * const { navigateTo } = useRoleNavigation();
 * navigateTo('products', 'list'); // سيتم التوجيه تلقائياً لـ /{role}/products/list
 */
export function useRoleNavigation() {
  const navigate = useNavigate();
  const { user, getRouteUrl, hasAccess } = useAuth();

  /**
   * التنقل لصفحة معينة
   * Navigate to a specific page
   *
   * @param {string} controller - اسم الـ Controller
   * @param {string} action - اسم الـ Action
   * @param {object} options - خيارات إضافية للـ navigate
   */
  const navigateTo = (controller, action, options = {}) => {
    if (!user) {
      navigate('/login');
      return;
    }

    // التحقق من الصلاحية
    if (!hasAccess(controller)) {
      console.warn(`No access to ${controller}`);
      return false;
    }

    // الحصول على الـ URL الصحيح وتنفيذ التنقل
    const url = getRouteUrl(controller, action);
    navigate(url, options);
    return true;
  };

  /**
   * الحصول على الـ URL بدون تنفيذ التنقل
   * Get URL without navigating
   */
  const getUrl = (controller, action) => {
    return getRouteUrl(controller, action);
  };

  return {
    navigateTo,
    getUrl,
    currentRole: user?.role,
  };
}

export default useRoleNavigation;
