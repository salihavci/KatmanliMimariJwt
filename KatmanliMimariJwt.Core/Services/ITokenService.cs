using KatmanliMimariJwt.Core.Configurations;
using KatmanliMimariJwt.Core.DTOs;
using KatmanliMimariJwt.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Core.Services
{
    public interface ITokenService
    {
        Task<TokenDto> CreateToken(UserApp userApp);
        ClientTokenDto TokenByClient(Client client);
    }
}
