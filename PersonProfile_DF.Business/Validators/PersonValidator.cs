using System;
using System.Text;
using System.Collections.Generic;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Utilities;

namespace PersonProfile_DF.Business.Validators
{
	public static class PersonValidator
	{
		public static void Validate(Person entity, bool isNew, bool isValidateForeignKeysEnabled)
		{
			bool hasValidationErrors = false;
			List<string> validationErrors = new List<string>();

			try
			{
				if(isNew == false && entity.PersonId == default(int))
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid PersonId");
				}

				if(String.IsNullOrEmpty(entity.Name) == true || entity.Name.Length > 70)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: Name . It is either null or have more than the allowed characters of: 70.");
				}

				if(String.IsNullOrEmpty(entity.Name) == false && entity.Name.Length > 70)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: Name . It is more than the allowed characters of: 70.");
				}

				if(String.IsNullOrEmpty(entity.EmailAddress) == true || entity.EmailAddress.Length > 80)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: EmailAddress . It is either null or have more than the allowed characters of: 80.");
				}

				if(String.IsNullOrEmpty(entity.EmailAddress) == false && entity.EmailAddress.Length > 80)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: EmailAddress . It is more than the allowed characters of: 80.");
				}

				if(String.IsNullOrEmpty(entity.MailingAddress) == true || entity.MailingAddress.Length > 200)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: MailingAddress . It is either null or have more than the allowed characters of: 200.");
				}

				if(String.IsNullOrEmpty(entity.MailingAddress) == false && entity.MailingAddress.Length > 200)
				{
					hasValidationErrors = true;
					validationErrors.Add("Invalid value for: MailingAddress . It is more than the allowed characters of: 200.");
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

				if(entity.LastUpdateDateTimeStamp == default(DateTime))
				{
					hasValidationErrors = true;
					validationErrors.Add("LastUpdateDateTimeStamp cannot be null.");
				}

			}
			catch (Exception)
			{
				hasValidationErrors = true;
				validationErrors.Add("Db validation failed for Person.");
			}

			if (hasValidationErrors == true)
			{
				throw new Exceptions.CustomBusinessValidationFailedException(string.Join(",", validationErrors));
			}
		}
	}
}

