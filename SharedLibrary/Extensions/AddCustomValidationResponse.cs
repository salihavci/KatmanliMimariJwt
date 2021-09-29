using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => {
                options.InvalidModelStateResponseFactory = context =>
                {
                    //Modeldeki errorların errormessage'leri al
                    var errors = context.ModelState.Values.Where(m => m.Errors.Count > 0).SelectMany(x=> x.Errors).Select(x=> x.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(),true);
                    var response = Response<NoContentResult>.Fail(errorDto, 400);
                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}
