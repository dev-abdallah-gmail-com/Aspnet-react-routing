# Plesk Deployment Guide for Mochahost
# دليل النشر على Plesk - Mochahost

## هيكل الملفات

```
deploy/
├── api/                 # Backend (ASP.NET Core API)
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Middleware/
│   ├── Program.cs
│   ├── backend.csproj
│   ├── appsettings.json
│   ├── appsettings.Production.json
│   └── web.config
│
└── www/                 # Frontend (React - Built)
    ├── index.html
    ├── web.config
    └── assets/
        ├── index-xxx.css
        └── index-xxx.js
```

---

## الخطوة 1: إعداد الـ Backend (API)

### Option A: استخدام Plesk Git Deployment (الأسهل)

1. **في Plesk، اذهب لـ:**
   - Websites & Domains → Your Domain → Git

2. **أضف Repository جديد:**
   - Repository URL: رابط الـ Git الخاص بك
   - أو ارفع مجلد `api` مباشرة

3. **إعداد Deployment:**
   - Document Root: `/httpdocs/api`
   - Enable "Deploy after push"

### Option B: رفع الملفات يدوياً

1. **ارفع مجلد `api` بالكامل إلى:**
   ```
   /var/www/vhosts/yourdomain.com/api/
   ```

2. **في Plesk:**
   - Websites & Domains → yourdomain.com
   - Add Subdomain: `api.yourdomain.com`
   - Document Root: `/api`
   - Hosting Type: ASP.NET Core

3. **إعداد ASP.NET Core في Plesk:**
   - Application Root: `/api`
   - Application Startup: `backend.dll`
   - Application Pool: إنشاء جديد

---

## الخطوة 2: إعداد الـ Frontend

1. **ارفع محتويات مجلد `www` إلى:**
   ```
   /var/www/vhosts/yourdomain.com/httpdocs/
   ```

2. **أو في Plesk File Manager:**
   - اذهب لـ Files
   - ارفع ملفات `www/*` إلى `httpdocs/`

---

## الخطوة 3: إعداد الاتصال بين Frontend و Backend

### إذا كان API على subdomain:

1. **عدّل `assets/index-xxx.js`:**
   - ابحث عن `/api`
   - غيرها إلى `https://api.yourdomain.com/api`

### إذا كان API على نفس الـ domain:

1. **في Plesk، أضف Application:**
   - Websites & Domains → yourdomain.com
   - Add Application (ASP.NET)
   - Path: `/api`
   - Document Root: مجلد الـ API

---

## الخطوة 4: إعدادات مهمة

### تعديل `appsettings.Production.json`:

```json
{
  "Jwt": {
    "SecretKey": "YOUR_VERY_LONG_SECRET_KEY_CHANGE_THIS_123456789"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  }
}
```

---

## المستخدمون للتجربة

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Admin (كل الصلاحيات) |
| editor | editor123 | Editor |
| evaluator | eval123 | Evaluator |
| user | user123 | User |

---

## التحقق من النشر

1. **Frontend:**
   ```
   https://yourdomain.com
   https://yourdomain.com/login
   ```

2. **Backend API:**
   ```
   https://yourdomain.com/api
   https://yourdomain.com/api/auth/login
   ```

---

## حل المشاكل

### مشكلة CORS:
تأكد من إعدادات `AllowedOrigins` في `appsettings.Production.json`

### صفحات React لا تعمل (404):
تأكد من وجود `web.config` في مجلد الـ Frontend مع URL Rewrite

### API لا يعمل:
- تحقق من Application Pool settings
- تأكد من تثبيت ASP.NET Core Runtime على السيرفر
