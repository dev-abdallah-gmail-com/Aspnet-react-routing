import { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

/**
 * القائمة الجانبية
 * Sidebar Navigation Component
 */
function Sidebar() {
  const { user, menu, logout } = useAuth();
  const navigate = useNavigate();
  const [openSections, setOpenSections] = useState({});

  // فتح/إغلاق قسم في القائمة
  const toggleSection = (controller) => {
    setOpenSections((prev) => ({
      ...prev,
      [controller]: !prev[controller],
    }));
  };

  // تسجيل الخروج
  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <aside className="sidebar">
      {/* رأس القائمة */}
      <div className="sidebar-header">
        <h2>Role-Based Routing</h2>
        <p>نظام التوجيه بناءً على الأدوار</p>
      </div>

      {/* معلومات المستخدم */}
      <div className="user-info">
        <div className="name">{user?.fullName}</div>
        <div className="role">
          <span>الدور:</span>
          <span className="role-badge">{user?.role}</span>
        </div>
      </div>

      {/* قائمة التنقل */}
      <nav className="nav-menu">
        {menu.map((section) => (
          <div
            key={section.controller}
            className={`nav-section ${openSections[section.controller] ? 'open' : ''}`}
          >
            {/* عنوان القسم */}
            <div
              className="nav-section-title"
              onClick={() => toggleSection(section.controller)}
            >
              <span className="icon">{section.icon}</span>
              <span>{section.title}</span>
              <span className="arrow">◀</span>
            </div>

            {/* عناصر القسم */}
            <div className="nav-items">
              {section.actions.map((action) => (
                <NavLink
                  key={action.action}
                  to={action.url}
                  className={({ isActive }) =>
                    `nav-item ${isActive ? 'active' : ''}`
                  }
                >
                  {action.title}
                </NavLink>
              ))}
            </div>
          </div>
        ))}
      </nav>

      {/* زر تسجيل الخروج */}
      <button className="logout-btn" onClick={handleLogout}>
        تسجيل الخروج
      </button>
    </aside>
  );
}

export default Sidebar;
