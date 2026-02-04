import axios from 'axios';

/**
 * إعداد Axios للاتصال بالـ API
 * Configure Axios for API communication
 */

const API_BASE_URL = 'http://localhost:5000/api';

// إنشاء instance من Axios
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor لإضافة التوكن لكل الطلبات
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor للتعامل مع الأخطاء
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // التوكن منتهي أو غير صالح
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

/**
 * خدمات المصادقة - Auth Services
 */
export const authService = {
  // تسجيل الدخول
  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    return response.data;
  },

  // الحصول على معلومات المستخدم الحالي
  getCurrentUser: async () => {
    const response = await api.get('/auth/me');
    return response.data;
  },

  // الحصول على القائمة المتاحة
  getMenu: async () => {
    const response = await api.get('/auth/menu');
    return response.data;
  },
};

/**
 * خدمة عامة للوصول للـ Controllers
 * Generic service to access any controller/action
 */
export const pageService = {
  /**
   * استدعاء أي action من أي controller
   * Call any action from any controller
   *
   * @param {string} controller - اسم الـ Controller
   * @param {string} action - اسم الـ Action
   * @param {object} params - parameters إضافية
   */
  getPage: async (controller, action, params = {}) => {
    const queryString = new URLSearchParams(params).toString();
    const url = `/${controller.toLowerCase()}/${action.toLowerCase()}${queryString ? `?${queryString}` : ''}`;
    const response = await api.get(url);
    return response.data;
  },
};

export default api;
