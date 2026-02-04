/**
 * Application Configuration
 * إعدادات التطبيق
 *
 * تعديل API_BASE_URL حسب بيئة الاستضافة
 */

// تحديد البيئة
const isDevelopment = import.meta.env.DEV;

// إعدادات الـ API
const config = {
  // Development: استخدم localhost
  // Production: غير هذا لعنوان الـ API الخاص بك
  API_BASE_URL: isDevelopment
    ? 'http://localhost:5000/api'
    : '/api', // في الإنتاج، استخدم مسار نسبي أو عنوان كامل

  // مثال لاستضافة منفصلة:
  // API_BASE_URL: 'https://api.yourdomain.com/api'

  // App Info
  APP_NAME: 'Role-Based Routing Demo',
  APP_VERSION: '1.0.0',
};

export default config;
