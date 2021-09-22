using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonProfile_DF.Website.Models;

namespace PersonProfile_DF.Api.Controllers
{	
    [Authorize]
    public abstract class AuthorizedController : ControllerBase
    {
        public ApplicationUser CurrentUser
        {
            get
            {
                return new ApplicationUser(this.User as ClaimsPrincipal);
            }
        }
    }
}

