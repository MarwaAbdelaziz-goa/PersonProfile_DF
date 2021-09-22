using System;
using System.Collections.Generic;

namespace PersonProfile_DF.Business
{
	public class App
	{
		internal static AppConfiguration Configuration { get; private set; }

		public static void Configure(AppConfiguration configuration)
		{
			Configuration = configuration;

			// Registering the global/centralized exceptional handler
			GlobalExceptionHandler globalExceptionHandler = new GlobalExceptionHandler();
			globalExceptionHandler.RegisterGlobalExceptionHandler();

		}

		internal static void ValidateUserToken(string userToken)
		{
			bool isValid = false;

			if(String.IsNullOrEmpty(userToken) == false)
			{
				List<string> tokenValues = null;
				Guid userGuid;
				string decryptedUserToken = EncryptDecryptHelper.Decrypt(userToken);
				isValid = TokenHelper.IsUserTokenValid(decryptedUserToken, out userGuid, out tokenValues);
			}

			if(!isValid)
			{
				throw new Exceptions.CustomBusinessException("Invalid/Expired user token. Please login.");
			}
		}
	}
}

