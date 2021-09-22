using System;
using FluentValidation;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.Validators
{
	public class CreatePersonValidator: AbstractValidator<CreatePerson>
	{
		public CreatePersonValidator()
		{
			RuleFor(x => x.Name).NotNull();
			RuleFor(x => x.EmailAddress).NotNull();
			RuleFor(x => x.MailingAddress).NotNull();
			RuleFor(x => x.PhoneNumber).NotNull();
			RuleFor(x => x.LastUpdateDateTimeStamp).NotNull();
		}
	}

	public class UpdatePersonValidator: AbstractValidator<UpdatePerson>
	{
		public UpdatePersonValidator()
		{
			RuleFor(x => x.PersonId).NotNull();
			RuleFor(x => x.Name).NotNull();
			RuleFor(x => x.EmailAddress).NotNull();
			RuleFor(x => x.MailingAddress).NotNull();
			RuleFor(x => x.PhoneNumber).NotNull();
			RuleFor(x => x.LastUpdateDateTimeStamp).NotNull();
		}
	}
}

