using System;
using System.Collections.Generic;

namespace PersonProfile_DF.Api.Models
{
	public class PhoneSearchParameters
	{
		public int? PhoneId { get; set; }

		public int? PersonId { get; set; }

		public string PhoneNumber { get; set; }

	}

	public class PhoneCore
	{
		public int PhoneId { get; set; }

		public string PhoneNumber { get; set; }

	}

	public class PhoneSearchResult
	{
		public int PhoneId { get; set; }

		public string PhoneNumber { get; set; }

		public string Person_Name { get; set; }
	}

	public class PhoneDtl
	{
		public int PhoneId { get; set; }

		public string PhoneNumber { get; set; }

		public PersonCore Person { get; set; }
	}

	public class CreatePhone
	{
		public int PersonId { get; set; }

		public string PhoneNumber { get; set; }

	}

	public class UpdatePhone
	{
		public int PhoneId { get; set; }

		public int PersonId { get; set; }

		public string PhoneNumber { get; set; }

	}

}

