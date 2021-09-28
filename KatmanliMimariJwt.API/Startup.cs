using KatmanliMimariJwt.Core.Configurations;
using KatmanliMimariJwt.Core.Models;
using KatmanliMimariJwt.Core.Repositories;
using KatmanliMimariJwt.Core.Services;
using KatmanliMimariJwt.Core.UnitOfWork;
using KatmanliMimariJwt.Data;
using KatmanliMimariJwt.Data.Repositories;
using KatmanliMimariJwt.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //-----------------------------Register Dependency Injection -----------------------------

            services.AddScoped<IAuthenticationService,AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); //2 Deðiþken aldýðý için virgül konuldu
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<AppDbContext>(opt=> {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),sqlOptions=> {
                    sqlOptions.MigrationsAssembly("KatmanliMimariJwt.Data");
                });
            });

            services.AddIdentity<UserApp,IdentityRole>(opts => {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //Token üretimi için default token provider çaðýrýldý
            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOptions"));
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
            //-----------------------------Register Dependency Injection End--------------------------

            //-----------------------------Token Authorization Injection -----------------------------

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //Eðer üye - bayi þeklinde ayýrsaydýk bunu direk string olarak yazabilirdik.
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //Ýki þemayý baðlamak için yazýlan kod
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opts=> {
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymetricSecurityKey(tokenOptions.SecurityKey),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //Expiration doðrulama payý (2 sunucuda zaman farký varsa) Default 5 dakika verir

                };
            });

            //-----------------------------Token Authorization Injection End--------------------------

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KatmanliMimariJwt.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KatmanliMimariJwt.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
