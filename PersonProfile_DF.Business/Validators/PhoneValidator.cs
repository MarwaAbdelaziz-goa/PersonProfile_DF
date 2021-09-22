using System;
using System.Text;
using System.Collections.Generic;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Utilities;

namespace PersonProfile_DF.Business.Validators
{
	public static class PhoneValidator
	{
		public static void Validate(Phone entity, bool isNew, bool isValidateForeignKeysEnabled)
		{
			bool hasValidationErrors = false;
			List<string> validationErrors = new List<string>();

			try
			{
				if(isNew == false && entity.PhoneId == default(int))
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid PhoneId");
				}

				if(isValidateForeignKeysEnabled == true && entity.PersonId == default(int))
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid PersonId");
				}

				if(String.IsNullOrEmpty(entity.PhoneNumber) == true || entity.PhoneNumber.Length > 50)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: PhoneNumber . It is either null or have more than the allowed characters of: 50.");
				}

				if(String.IsNullOrEmpty(entity.PhoneNumber) == false && entity.PhoneNumber.Length > 50)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: PhoneNumber . It is more than the allowed characters of: 50.");
				}

			}
			catch (Exception)
			{
				hasValidationErrors = true;
				validationErrors.Add("Db validation failed for Phone.");
			}

			if (hasValidationErrors == true)
			{
				throw new Exceptions.CustomBusinessValidationFailedException(string.Join(",", validationErrors));
			}
		}
	}
}

