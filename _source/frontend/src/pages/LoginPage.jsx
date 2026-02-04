import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

/**
 * صفحة تسجيل الدخول
 * Login Page Component
 */
function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    const result = await login(username, password);

    if (result.success) {
      // التوجيه للصفحة الرئيسية بناءً على دور المستخدم
      navigate(`/${result.user.role}/dashboard/index`);
    } else {
      setError(result.message);
    }

    setIsLoading(false);
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h1>تسجيل الدخول</h1>
        <p>Role-Based Routing Demo</p>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="username">اسم المستخدم</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="أدخل اسم المستخدم"
              required
              autoFocus
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">كلمة المرور</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="أدخل كلمة المرور"
              required
            />
          </div>

          <button type="submit" className="login-btn" disabled={isLoading}>
            {isLoading ? 'جاري تسجيل الدخول...' : 'تسجيل الدخول'}
          </button>
        </form>

        <div className="users-hint">
          <h4>المستخدمون المتاحون للتجربة:</h4>
          <table>
            <tbody>
              <tr>
                <td><code>admin</code></td>
                <td><code>admin123</code></td>
                <td>كل الصلاحيات</td>
              </tr>
              <tr>
                <td><code>editor</code></td>
                <td><code>editor123</code></td>
                <td>Dashboard, Products, Orders</td>
              </tr>
              <tr>
                <td><code>evaluator</code></td>
                <td><code>eval123</code></td>
                <td>Dashboard, Users, Orders</td>
              </tr>
              <tr>
                <td><code>user</code></td>
                <td><code>user123</code></td>
                <td>Dashboard, Orders</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
