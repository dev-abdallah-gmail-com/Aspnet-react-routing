import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { pageService } from '../services/api';

/**
 * صفحة ديناميكية تعرض بيانات أي Controller/Action
 * Dynamic Page Component - displays any controller/action data
 *
 * هذه الصفحة هي جوهر النظام:
 * 1. تستقبل role, controller, action من الـ URL
 * 2. تتحقق من صلاحية الوصول
 * 3. تجلب البيانات من الـ API
 * 4. تعرض معلومات المستخدم والصفحة
 */
function DynamicPage() {
  const { role, controller, action } = useParams();
  const { user, hasAccess } = useAuth();
  const navigate = useNavigate();

  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      setError(null);

      try {
        // التحقق من أن الـ role في الـ URL يطابق role المستخدم
        if (user && role !== user.role) {
          // إعادة التوجيه للـ URL الصحيح
          navigate(`/${user.role}/${controller}/${action}`, { replace: true });
          return;
        }

        // التحقق من الصلاحية
        if (!hasAccess(controller)) {
          setError('ليس لديك صلاحية للوصول لهذه الصفحة');
          setLoading(false);
          return;
        }

        // جلب البيانات من الـ API
        const response = await pageService.getPage(controller, action);
        setData(response);
      } catch (err) {
        if (err.response?.status === 403) {
          setError('ليس لديك صلاحية للوصول لهذه الصفحة');
        } else if (err.response?.status === 404) {
          setError('الصفحة غير موجودة');
        } else {
          setError('حدث خطأ في تحميل البيانات');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [role, controller, action, user, navigate, hasAccess]);

  // شاشة التحميل
  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
      </div>
    );
  }

  // شاشة الخطأ
  if (error) {
    return (
      <div className="page-content">
        <div className="error-message">{error}</div>
      </div>
    );
  }

  return (
    <>
      {/* رأس الصفحة */}
      <div className="page-header">
        <h1>{data?.message}</h1>
        <div className="breadcrumb">
          <span>{role}</span> / <span>{controller}</span> / <span>{action}</span>
        </div>
      </div>

      {/* محتوى الصفحة */}
      <div className="page-content">
        {/* بطاقات المعلومات الأساسية */}
        <div className="info-grid">
          <div className="info-card highlight">
            <label>اسم المستخدم</label>
            <div className="value">{data?.userName}</div>
          </div>

          <div className="info-card highlight">
            <label>دور المستخدم</label>
            <div className="value">{data?.userRole}</div>
          </div>

          <div className="info-card">
            <label>Controller</label>
            <div className="value">{data?.controller}</div>
          </div>

          <div className="info-card">
            <label>Action</label>
            <div className="value">{data?.action}</div>
          </div>
        </div>

        {/* عنوان URL */}
        <div className="info-card" style={{ marginBottom: '24px' }}>
          <label>Route URL</label>
          <div className="value" style={{ fontFamily: 'monospace', direction: 'ltr', textAlign: 'left' }}>
            {data?.routeUrl}
          </div>
        </div>

        {/* البيانات المرجعة من الـ API */}
        {data?.data && (
          <div className="data-section">
            <h3>البيانات المرجعة من الـ API:</h3>
            <div className="data-box">
              {JSON.stringify(data.data, null, 2)}
            </div>
          </div>
        )}
      </div>
    </>
  );
}

export default DynamicPage;
