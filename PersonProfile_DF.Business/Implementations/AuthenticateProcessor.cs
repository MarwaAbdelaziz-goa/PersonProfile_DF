using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonProfile_DF.Business;
using PersonProfile_DF.Business.Contracts;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Business.Data.Context;

namespace PersonProfile_DF.Business.Implementations
{
	internal class AuthenticateProcessor : BaseProcessor, IAuthenticateProcessor
	{
		internal AuthenticateProcessor() : base()
		{
		}

		public async Task<LoginResponse> SignInAsync(LoginRequest loginRequest)
		{
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				// Todo: Authenticate the user against the proper tables; here we're just temporarily using the hardcoded usernames/passwords

				if(loginRequest != null && (loginRequest.Username == "admin" && loginRequest.Password == "password") || (loginRequest.Username == "care" && loginRequest.Password == "password"))
				{
					var authenticatedToken = TokenHelper.GenerateUserToken(Guid.NewGuid(), new string[] { loginRequest.Username, loginRequest.Password });
					var encryptedToken = EncryptDecryptHelper.Encrypt(authenticatedToken);
					App.Configuration.ConsumerUserToken = encryptedToken;

					string name = null;
					string email = null;
					if(loginRequest.Username == "admin")
					{
						name = "Admin";
						email = "admin@admin.com";
					}
					else if(loginRequest.Username == "care")
					{
						name = "Customer Care";
						email= "customercare@customercare.com";
					}
					return new LoginResponse { IsAuthenticationSuccess = true, AuthenticatedToken = encryptedToken, Name = name, Email = email };
				}
				else
				{
					return new LoginResponse { IsAuthenticationSuccess = false, AuthenticatedToken = null };
				}
			}
		}
		public async Task SignOutAsync()
		{
			App.Configuration.ConsumerUserToken = null;
		}
	}
}

