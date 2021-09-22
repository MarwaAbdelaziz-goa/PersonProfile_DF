using System;
using System.Collections.Generic;

namespace PersonProfile_DF.Business.Data.Models
{
	public class Phone
	{
		public int PhoneId { get; set; } // Primary Key
		public int PersonId { get; set; } // Foreign Key
		public string PhoneNumber { get; set; }

		public Person Person { get; set; } // One-To-Many (One-Side) [FK_Phone_Person] 

	}
}

