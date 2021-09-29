using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.DTOs;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomExceptionHandle
    {
        public static void UseCustomExceptionHandle(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config => {
                config.Run(async context=> {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;
                        ErrorDto errorDto = null;

                        if (ex is CustomException)
                        {
                            errorDto = new ErrorDto(ex.Message, true); //Fluent validation hatası
                        }
                        else
                        {
                            errorDto = new ErrorDto(ex.Message, false); //Uygulama hatası
                        }
                        var response = Response<NoDataDto>.Fail(errorDto,500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                }); //Run = Sonlandırıcı middleware, Use = devam edebilen middleware
            });
        }
    }
}
