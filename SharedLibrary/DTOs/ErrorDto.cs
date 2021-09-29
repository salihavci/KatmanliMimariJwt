using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTOs
{
    public class ErrorDto
    {
        public List<String> Errors { get; private set; } = new List<string>();
        public bool IsShow { get; private set; }
        
        public ErrorDto(string Error,bool isShow)
        {
            Errors.Add(Error);
            IsShow = isShow;
        }
        public ErrorDto(List<string> Errors,bool isShow)
        {
            this.Errors = Errors;
            IsShow = isShow;
        }
    }
}
