using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Contracts
{
	public interface ILookupProcessor
	{
		Task<IEnumerable<Person>> GetAllPeopleAsync();

		Task<IEnumerable<Phone>> GetAllPhonesAsync();

	}
}

