using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Req
{
    public class AuthReq
    {
        public string? Email { get; set; } 
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
    }
}
