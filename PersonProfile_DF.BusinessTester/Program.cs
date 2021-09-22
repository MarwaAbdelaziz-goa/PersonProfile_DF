using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PersonProfile_DF.BusinessTester
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				// =========================================
				// EXAMPLE OF HOW TO CALL THE BUSINESS LAYER
				// 1. Initialize the Business Layer one-time
				// 2. Call any function
				// =========================================

				//--------------------------------------------------------------------
				// STEP-1 (perform it once): Initialize Business Library configuration
				//--------------------------------------------------------------------
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build();

				PersonProfile_DF.Business.App.Configure(new PersonProfile_DF.Business.AppConfiguration
				{
					//ConsumerApplicationToken = PersonProfile_DF.Utilities.EncryptDecryptHelper.Encrypt("PersonProfile_DF", PersonProfile_DF.Utilities.Constants.EncryptDecryptKey),
					ConnectionString = configuration["ConnectionString"],
					ErrorLogDestination = configuration["ErrorLogDestination"],
					IsDbQueryTraceEnabled = Convert.ToBoolean(configuration["IsDbQueryTraceEnabled"]),
					DbQueryTraceDestination = configuration["DbQueryTraceDestination"]
				});

				//-----------------------------------------------------------------------------------------------
				// STEP-2 (authenticate user)
				//-----------------------------------------------------------------------------------------------
				PersonProfile_DF.Business.Contracts.IAuthenticateProcessor authenticateProcessor = PersonProfile_DF.Business.ProcessorFactory.CreateAuthenticateProcessor();
				var loginResponse = authenticateProcessor.SignInAsync(new Business.Data.Models.LoginRequest { Username = "admin", Password = "password" });

				//-----------------------------------------------------------------------------------------------
				// STEP-3 (call any function): Create the object of any 'Processor' class and then call any method
				//-----------------------------------------------------------------------------------------------
				PersonProfile_DF.Business.Contracts.IPersonProcessor personProcessor = PersonProfile_DF.Business.ProcessorFactory.CreatePersonProcessor();
				var records = personProcessor.GetAllPeopleAsync(loginResponse.Result.AuthenticatedToken, "PersonId", 1, 7);
			}
			catch(Exception exp)
			{
				string errMessage = exp.Message;
			}
		}
	}
}
