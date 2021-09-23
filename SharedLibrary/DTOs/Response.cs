using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; } //Private set attribute'si constructor dışında bir fonksiyonda kullanılabilmesi için yazılıyor
        public int StatusCode { get; private set; }
        public ErrorDto Error { get; private set; }
        [JsonIgnore]
        public bool IsSuccessful { get; private set; }
        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful = true }; //Default vermemizin sebebi boş gelmesini istememizdir.
        }
        public static Response<T> Fail(ErrorDto error, int statusCode)
        {
            return new Response<T> { StatusCode = statusCode, Error = error, IsSuccessful = false };
        }
        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            return new Response<T> { StatusCode = statusCode, Error = new ErrorDto(errorMessage, isShow), IsSuccessful = false };
        }

    }
}
