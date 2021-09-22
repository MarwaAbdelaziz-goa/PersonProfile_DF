using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace PersonProfile_DF.Website.Models
{
	
    public class ApplicationUser : ClaimsPrincipal
    {
        public ApplicationUser(ClaimsPrincipal principal) : base(principal)
        {
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.Identity != null && this.Identity.IsAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }

        public string UserToken
        {
            get
            {
                return this.FindFirst(ClaimTypes.UserData).Value;
            }
        }
    }
}

