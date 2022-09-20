using KatmanliMimariJwt.Core.Configurations;
using KatmanliMimariJwt.Core.DTOs;
using KatmanliMimariJwt.Core.Models;
using KatmanliMimariJwt.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager; //Kullanıcı bilgilerini almak için lazım
        private readonly CustomTokenOptions _tokenOptions;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> tokenOptions)
        {
            _userManager = userManager;
            _tokenOptions = tokenOptions.Value;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        private async Task<IEnumerable<Claim>> GetClaim(UserApp userApp, List<String> Audiences)
        {
            var userRoles = await _userManager.GetRolesAsync(userApp).ConfigureAwait(false);
            var userList = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //Claim ID'si olacak
                new Claim("city",userApp.City),
                new Claim("birthDate",userApp.BirthDate.ToShortDateString())
            };
            userList.AddRange(Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x))); //Rol bazlı claim kaydetme ve yetki verme.
            return userList;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x))); //Audience Claim'i için dizi şeklinde oluşturma
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString());
            return claims;

        }

        public async Task<TokenDto> CreateToken(UserApp userApp)
        {

            var AccessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var RefreshTokenExpiraton = DateTime.Now.AddMinutes(_tokenOptions.RefreshTokenExpiration);
            var SecurityKey = SignService.GetSymetricSecurityKey(_tokenOptions.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                expires: AccessTokenExpiration,
                notBefore: DateTime.Now,
                claims: await GetClaim(userApp, _tokenOptions.Audience),
                signingCredentials: signingCredentials);
            var handler = new JwtSecurityTokenHandler();
            var AccessToken = handler.WriteToken(jwtSecurityToken);
            var TokenDto = new TokenDto
            {
                AccessToken = AccessToken,
                AccessTokenExpiration = AccessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = RefreshTokenExpiraton
            };
            return TokenDto;
        }

        public ClientTokenDto TokenByClient(Client client)
        {
            var AccessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var SecurityKey = SignService.GetSymetricSecurityKey(_tokenOptions.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                expires: AccessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials);
            var handler = new JwtSecurityTokenHandler();
            var AccessToken = handler.WriteToken(jwtSecurityToken);
            var TokenDto = new ClientTokenDto
            {
                AccessToken = AccessToken,
                AccessTokenExpiration = AccessTokenExpiration,
            };
            return TokenDto;
        }
    }
}
