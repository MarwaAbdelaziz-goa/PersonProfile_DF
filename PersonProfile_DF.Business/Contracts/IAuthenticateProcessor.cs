using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Contracts
{
	public interface IAuthenticateProcessor
	{
		Task<LoginResponse> SignInAsync(LoginRequest loginRequest);
		Task SignOutAsync();
	}
}

