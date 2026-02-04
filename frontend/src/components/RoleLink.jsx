import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

/**
 * مكون الرابط المبني على الدور
 * Role-based Link Component
 *
 * الاستخدام:
 * <RoleLink controller="products" action="list">قائمة المنتجات</RoleLink>
 *
 * سيتم تحويله تلقائياً للـ URL الصحيح:
 * <a href="/admin/products/list">قائمة المنتجات</a>
 */
function RoleLink({ controller, action, children, className, ...props }) {
  const { user, hasAccess, getRouteUrl } = useAuth();

  // إذا لم يكن مسجل دخول
  if (!user) {
    return null;
  }

  // إذا لم يكن لديه صلاحية
  if (!hasAccess(controller)) {
    return null;
  }

  const url = getRouteUrl(controller, action);

  return (
    <Link to={url} className={className} {...props}>
      {children}
    </Link>
  );
}

export default RoleLink;
