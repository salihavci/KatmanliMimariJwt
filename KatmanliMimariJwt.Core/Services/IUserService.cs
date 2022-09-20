using KatmanliMimariJwt.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Core.Services
{
    public interface IUserService 
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUser);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
        Task<Response<NoContentResult>> CreateUserRoles(string username);
    }
}
