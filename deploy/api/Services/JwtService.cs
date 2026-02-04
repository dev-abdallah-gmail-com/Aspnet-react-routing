using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

/// <summary>
/// خدمة JWT لإنشاء والتحقق من التوكنات
/// JWT Service for creating and validating tokens
/// </summary>
public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456789";
        _issuer = configuration["Jwt:Issuer"] ?? "RoleBasedRoutingDemo";
        _audience = configuration["Jwt:Audience"] ?? "RoleBasedRoutingDemoUsers";
    }

    /// <summary>
    /// إنشاء توكن JWT للمستخدم
    /// Generate JWT token for user
    /// </summary>
    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // الـ Claims تحتوي على معلومات المستخدم المخزنة في التوكن
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // صالح لمدة 24 ساعة
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// التحقق من صحة التوكن واستخراج المعلومات
    /// Validate token and extract claims
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
