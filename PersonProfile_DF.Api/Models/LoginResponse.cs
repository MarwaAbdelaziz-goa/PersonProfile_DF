
using System;

namespace PersonProfile_DF.Api.Models
{
    public class LoginResponse
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}


