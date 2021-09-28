using KatmanliMimariJwt.Core.DTOs;
using KatmanliMimariJwt.Core.Models;
using KatmanliMimariJwt.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUser)
        {
            var user = new UserApp
            {
                Email = createUser.Email,
                UserName = createUser.UserName
            };
            var result = await _userManager.CreateAsync(user, createUser.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);

            }
            return Response<UserAppDto>.Success(ObjectMapper._mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return Response<UserAppDto>.Fail("Username not found", 404, true);
            return Response<UserAppDto>.Success(ObjectMapper._mapper.Map<UserAppDto>(user), 200);
        }
    }
}
