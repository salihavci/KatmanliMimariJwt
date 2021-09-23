using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Core.Configurations
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<String> Audiences { get; set; } //Erişmek istenilen apilerin listesini tutacak değişken
    }
}
