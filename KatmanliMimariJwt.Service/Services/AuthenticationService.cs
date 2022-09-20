using KatmanliMimariJwt.Core.Configurations;
using KatmanliMimariJwt.Core.DTOs;
using KatmanliMimariJwt.Core.Models;
using KatmanliMimariJwt.Core.Repositories;
using KatmanliMimariJwt.Core.Services;
using KatmanliMimariJwt.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClients, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateToken(LoginDto login)
        {
            if (login == null) throw new ArgumentException(nameof(login));
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null) return Response<TokenDto>.Fail("Email or password is wrong.", 400, true);
            if (!await _userManager.CheckPasswordAsync(user, login.Password)) return Response<TokenDto>.Fail("Email or password is wrong.", 400, true);
            var token = await _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken == null) await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiraton = token.RefreshTokenExpiration });
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiraton = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token,200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLogin)
        {
            var clients = _clients.SingleOrDefault(x => x.ClientId == clientLogin.ClientId && x.ClientSecret == clientLogin.ClientSecret);
            if (clients == null) return Response<ClientTokenDto>.Fail("Client id or Client secret not found", 404, true);
            var token = _tokenService.TokenByClient(clients);
            return Response<ClientTokenDto>.Success(token, 200);

        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null) return Response<TokenDto>.Fail("Refresh token not found.", 404, true);
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user == null) return Response<TokenDto>.Fail("User id not found", 404, true);
            var token = await _tokenService.CreateToken(user);
            existRefreshToken.Code = token.RefreshToken;
            existRefreshToken.Expiraton = token.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null) Response<NoDataDto>.Fail("Refresh token not found.", 404, true);
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}
