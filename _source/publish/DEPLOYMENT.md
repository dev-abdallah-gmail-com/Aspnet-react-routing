# دليل النشر على Windows Hosting (IIS)

## المتطلبات الأساسية

### على جهاز التطوير:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

### على السيرفر:
- Windows Server 2016+ أو Windows 10+
- IIS 10+
- [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) (Runtime)
- [URL Rewrite Module](https://www.iis.net/downloads/microsoft/url-rewrite)

---

## الخطوة 1: بناء المشروع

### الطريقة السهلة (سكربت واحد):
```batch
cd publish\scripts
publish-all.bat
```

### أو بناء كل جزء منفصلاً:

#### بناء الـ Backend:
```batch
cd publish\scripts
publish-backend.bat
```

#### بناء الـ Frontend:
```batch
cd publish\scripts
publish-frontend.bat
```

---

## الخطوة 2: تجهيز الملفات للنشر

بعد تشغيل السكربتات، ستجد الملفات في:
```
publish/
├── backend/
│   └── app/          ← ملفات الـ Backend (API)
└── frontend/
    └── app/          ← ملفات الـ Frontend (React)
```

### تعديل الإعدادات قبل النشر:

#### 1. Backend - تعديل `appsettings.Production.json`:
```json
{
  "Jwt": {
    "SecretKey": "YOUR_VERY_LONG_SECRET_KEY_HERE"  // غير هذا!
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com"  // عنوان موقعك
    ]
  }
}
```

#### 2. Frontend - تعديل `src/config.js` قبل البناء:
```javascript
const config = {
  API_BASE_URL: 'https://api.yourdomain.com/api'  // عنوان الـ API
};
```

---

## الخطوة 3: النشر على IIS

### السيناريو 1: Backend و Frontend على نفس السيرفر

```
wwwroot/
├── api/              ← Backend files
│   ├── backend.dll
│   ├── web.config
│   └── ...
└── frontend/         ← Frontend files
    ├── index.html
    ├── web.config
    └── assets/
```

#### إعداد IIS:

1. **إنشاء Application Pool للـ Backend:**
   - Name: `RoleBasedRoutingAPI`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`

2. **إنشاء Website:**
   - Site name: `RoleBasedRouting`
   - Physical path: `C:\wwwroot\frontend`
   - Application Pool: `DefaultAppPool`

3. **إضافة Application للـ API:**
   - Right-click on site → Add Application
   - Alias: `api`
   - Physical path: `C:\wwwroot\api`
   - Application Pool: `RoleBasedRoutingAPI`

4. **تعديل Frontend config:**
   ```javascript
   API_BASE_URL: '/api'
   ```

### السيناريو 2: Backend و Frontend على سيرفرات منفصلة

#### سيرفر الـ API (api.yourdomain.com):
1. إنشاء Application Pool (No Managed Code)
2. إنشاء Website يشير لمجلد الـ Backend

#### سيرفر الـ Frontend (www.yourdomain.com):
1. إنشاء Website يشير لمجلد الـ Frontend
2. تأكد من تثبيت URL Rewrite Module

---

## الخطوة 4: التحقق من الإعدادات

### التحقق من الـ Backend:
```
https://api.yourdomain.com/swagger
https://api.yourdomain.com/api/auth/login
```

### التحقق من الـ Frontend:
```
https://www.yourdomain.com
https://www.yourdomain.com/login
```

---

## حل المشاكل الشائعة

### 1. خطأ 500.19 - web.config Error
```
تأكد من تثبيت ASP.NET Core Hosting Bundle
```

### 2. خطأ 404 في React Routes
```
تأكد من تثبيت URL Rewrite Module
تحقق من وجود web.config في مجلد الـ Frontend
```

### 3. CORS Errors
```
تحقق من إعدادات AllowedOrigins في appsettings.Production.json
```

### 4. الـ API لا يعمل
```batch
# تحقق من الـ logs
type C:\wwwroot\api\logs\stdout*.log
```

### 5. تشغيل يدوي للتشخيص:
```batch
cd C:\wwwroot\api
dotnet backend.dll
```

---

## إعدادات SSL (مهم للإنتاج)

### الحصول على شهادة SSL مجانية:
1. [Let's Encrypt](https://letsencrypt.org/) مع [win-acme](https://www.win-acme.com/)
2. أو من مزود الاستضافة

### إعداد HTTPS في IIS:
1. Import SSL Certificate
2. Edit Site Bindings → Add HTTPS (443)
3. Enable "Require SSL" في SSL Settings

---

## نصائح للإنتاج

1. **غير Secret Key** في `appsettings.Production.json`
2. **فعّل HTTPS** إجباري
3. **اضبط CORS** للدومين الخاص بك فقط
4. **فعّل Logging** لمراقبة الأخطاء
5. **استخدم Application Insights** للمراقبة (اختياري)

---

## هيكل الملفات النهائي على السيرفر

```
C:\wwwroot\
├── api\
│   ├── backend.dll
│   ├── backend.deps.json
│   ├── backend.runtimeconfig.json
│   ├── web.config
│   ├── appsettings.json
│   ├── appsettings.Production.json
│   └── logs\
│       └── stdout_*.log
│
└── frontend\
    ├── index.html
    ├── web.config
    └── assets\
        ├── index-xxxxx.js
        └── index-xxxxx.css
```
