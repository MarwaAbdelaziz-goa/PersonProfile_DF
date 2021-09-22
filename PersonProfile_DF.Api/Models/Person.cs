using System;
using System.Collections.Generic;

namespace PersonProfile_DF.Api.Models
{
	public class PersonSearchParameters
	{
		public int? PersonId { get; set; }

		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime? LastUpdateDateTimeStamp { get; set; }

	}

	public class PersonCore
	{
		public int PersonId { get; set; }

		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime LastUpdateDateTimeStamp { get; set; }

	}

	public class PersonSearchResult
	{
		public int PersonId { get; set; }

		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime LastUpdateDateTimeStamp { get; set; }

	}

	public class PersonDtl
	{
		public int PersonId { get; set; }

		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime LastUpdateDateTimeStamp { get; set; }

	}

	public class CreatePerson
	{
		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime LastUpdateDateTimeStamp { get; set; }

	}

	public class UpdatePerson
	{
		public int PersonId { get; set; }

		public string Name { get; set; }

		public string EmailAddress { get; set; }

		public string MailingAddress { get; set; }

		public string PhoneNumber { get; set; }

		public DateTime LastUpdateDateTimeStamp { get; set; }

	}

}

