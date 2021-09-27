using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(()=> {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MapProfile>();
                
            });
            return config.CreateMapper(); //IMapper return edecek
        }); //Lazy Loading işlemi
        public static IMapper _mapper => lazy.Value; 
    }
}
