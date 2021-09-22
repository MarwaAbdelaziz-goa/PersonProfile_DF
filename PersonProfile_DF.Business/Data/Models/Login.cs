using System;

namespace PersonProfile_DF.Business.Data.Models
{	
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool IsAuthenticationSuccess { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AuthenticatedToken { get; set; }
    }
}

