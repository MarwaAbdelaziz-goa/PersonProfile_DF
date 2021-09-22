using System;
using System.Collections.Generic;

namespace PersonProfile_DF.Business.Data.Models
{
	public class Person
	{
		public int PersonId { get; set; } // Primary Key
		public string Name { get; set; }
		public string EmailAddress { get; set; }
		public string MailingAddress { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime LastUpdateDateTimeStamp { get; set; }

		public ICollection<Phone> Phones { get; set; } // One-To-Many (Many-Side) [FK_Phone_Person] 

		public Person()
		{
			Phones = new HashSet<Phone>();
		}
	}
}

