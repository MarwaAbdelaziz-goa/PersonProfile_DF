using System;
using FluentValidation;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.Validators
{
	public class CreatePhoneValidator: AbstractValidator<CreatePhone>
	{
		public CreatePhoneValidator()
		{
			RuleFor(x => x.PersonId).NotNull();
			RuleFor(x => x.PhoneNumber).NotNull();
		}
	}

	public class UpdatePhoneValidator: AbstractValidator<UpdatePhone>
	{
		public UpdatePhoneValidator()
		{
			RuleFor(x => x.PersonId).NotNull();
			RuleFor(x => x.PhoneId).NotNull();
			RuleFor(x => x.PhoneNumber).NotNull();
		}
	}
}

