using Application.Exceptions;
using Application.jwt;
using Application.Security.Cryptography;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Implementation.jwt
{
    public class JwtManager : IJwtManager
    {
        private readonly DatabaseContext _db;
        private readonly JwtOptions _jwt;
        private readonly IPasswordHasher _hasher;

        public JwtManager(DatabaseContext db, IOptions<JwtOptions> jwtOptions, IPasswordHasher hasher)
        {
            _db = db;
            _jwt = jwtOptions.Value;
            _hasher = hasher;
        }

        public async Task<string> MakeToken(string email, string password, CancellationToken ct = default)
        {
        
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);


            if (user is null)
                throw new UnauthorizedException("Invalid credentials. (email)");

            if (!_hasher.Verify(password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials. (password)");

          
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_jwt.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            // 4) Potpis + token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
