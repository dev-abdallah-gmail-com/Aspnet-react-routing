# Role-Based Routing Demo

نظام توجيه (Routing) مبني على أدوار المستخدمين باستخدام ASP.NET Core و React.

## 📋 الهدف من المشروع

شرح مفهوم الـ Role-Based Routing للمبرمجين، حيث يتم توجيه المستخدمين لصفحات مختلفة بناءً على أدوارهم.

## 🏗️ هيكل المشروع

```
/Aspnet-react-routing
├── /backend                    # ASP.NET Core API
│   ├── /Controllers
│   │   ├── AuthController.cs      # تسجيل الدخول
│   │   ├── BaseApiController.cs   # Controller أساسي
│   │   ├── DashboardController.cs # لوحة التحكم
│   │   ├── UsersController.cs     # المستخدمون
│   │   ├── ProductsController.cs  # المنتجات
│   │   ├── OrdersController.cs    # الطلبات
│   │   └── SettingsController.cs  # الإعدادات
│   ├── /Models
│   │   └── User.cs               # نماذج البيانات
│   ├── /Services
│   │   ├── JwtService.cs         # خدمة JWT
│   │   └── UserService.cs        # خدمة المستخدمين
│   ├── /Middleware
│   │   └── RoleAuthorizationMiddleware.cs
│   ├── Program.cs
│   └── appsettings.json
│
└── /frontend                   # React Application
    ├── /src
    │   ├── /components
    │   │   ├── Layout.jsx        # التخطيط الرئيسي
    │   │   ├── Sidebar.jsx       # القائمة الجانبية
    │   │   └── RoleLink.jsx      # روابط ديناميكية
    │   ├── /pages
    │   │   ├── LoginPage.jsx     # صفحة تسجيل الدخول
    │   │   └── DynamicPage.jsx   # صفحة ديناميكية
    │   ├── /context
    │   │   └── AuthContext.jsx   # سياق المصادقة
    │   ├── /services
    │   │   └── api.js            # خدمات API
    │   ├── /hooks
    │   │   └── useRoleNavigation.js
    │   ├── App.jsx
    │   ├── main.jsx
    │   └── styles.css
    ├── index.html
    ├── package.json
    └── vite.config.js
```

## 👥 المستخدمون المتاحون

| المستخدم | كلمة المرور | الدور | الصلاحيات |
|----------|-------------|-------|-----------|
| `admin` | `admin123` | Admin | كل الصلاحيات |
| `editor` | `editor123` | Editor | Dashboard, Products, Orders |
| `evaluator` | `eval123` | Evaluator | Dashboard, Users, Orders |
| `user` | `user123` | User | Dashboard, Orders |

## 🔗 نظام الـ Routing

### نمط الـ URL
```
/{role}/{controller}/{action}
```

### أمثلة:
```
/admin/dashboard/index
/admin/users/list
/editor/products/create
/user/orders/details
```

## 📦 الـ Controllers و Actions

### 1. Dashboard Controller
| Action | الوصف |
|--------|-------|
| Index | الصفحة الرئيسية |
| Stats | الإحصائيات |
| Reports | التقارير |
| Summary | الملخص |

### 2. Users Controller
| Action | الوصف |
|--------|-------|
| List | قائمة المستخدمين |
| Details | تفاصيل المستخدم |
| Create | إضافة مستخدم |
| Edit | تعديل مستخدم |

### 3. Products Controller
| Action | الوصف |
|--------|-------|
| List | قائمة المنتجات |
| Details | تفاصيل المنتج |
| Create | إضافة منتج |
| Edit | تعديل منتج |

### 4. Orders Controller
| Action | الوصف |
|--------|-------|
| List | قائمة الطلبات |
| Details | تفاصيل الطلب |
| Process | معالجة الطلب |
| Cancel | إلغاء الطلب |

### 5. Settings Controller (Admin فقط)
| Action | الوصف |
|--------|-------|
| General | إعدادات عامة |
| Security | الأمان |
| Notifications | الإشعارات |
| Backup | النسخ الاحتياطي |

## 🔐 نظام الصلاحيات

```
Admin      → [Dashboard, Users, Products, Orders, Settings]
Editor     → [Dashboard, Products, Orders]
Evaluator  → [Dashboard, Users, Orders]
User       → [Dashboard, Orders]
```

## 🚀 تشغيل المشروع

### 1. تشغيل الـ Backend
```bash
cd backend
dotnet restore
dotnet run
```
سيعمل على: `http://localhost:5000`

### 2. تشغيل الـ Frontend
```bash
cd frontend
npm install
npm run dev
```
سيعمل على: `http://localhost:3000`

## 🔧 كيف يعمل النظام

### 1. تسجيل الدخول
```
POST /api/auth/login
Body: { "username": "admin", "password": "admin123" }
Response: { token: "jwt...", user: {...} }
```

### 2. جلب القائمة
```
GET /api/auth/menu
Headers: Authorization: Bearer {token}
Response: [{ controller, title, actions: [...] }]
```

### 3. استدعاء صفحة
```
GET /api/{controller}/{action}
Headers: Authorization: Bearer {token}
Response: {
  userName: "...",
  userRole: "...",
  controller: "...",
  action: "...",
  routeUrl: "/{role}/{controller}/{action}",
  data: {...}
}
```

## 📱 استخدام الـ Hooks في React

### useRoleNavigation
```jsx
import { useRoleNavigation } from './hooks/useRoleNavigation';

function MyComponent() {
  const { navigateTo, getUrl } = useRoleNavigation();

  // التنقل لصفحة (يتم إضافة الـ role تلقائياً)
  const goToProducts = () => {
    navigateTo('products', 'list');
    // سيتم التوجيه لـ /admin/products/list (إذا كان admin)
  };

  // الحصول على URL فقط
  const url = getUrl('orders', 'details');
  // returns: /admin/orders/details
}
```

### RoleLink Component
```jsx
import RoleLink from './components/RoleLink';

// الرابط سيظهر فقط إذا كان للمستخدم صلاحية
<RoleLink controller="products" action="list">
  قائمة المنتجات
</RoleLink>
```

## 🎯 ما الذي تتعلمه من هذا المشروع؟

1. **JWT Authentication** - كيفية إنشاء والتحقق من التوكنات
2. **Role-Based Authorization** - تقييد الوصول بناءً على الأدوار
3. **Dynamic Routing** - توجيه ديناميكي في React
4. **API Design** - تصميم API موحد
5. **React Context** - إدارة حالة المصادقة
6. **ASP.NET Core Middleware** - إنشاء middleware مخصص

## 📚 الموارد

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [React Router Documentation](https://reactrouter.com)
- [JWT.io](https://jwt.io)
