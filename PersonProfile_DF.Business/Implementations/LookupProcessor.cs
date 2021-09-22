using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Business.Data.Context;
using PersonProfile_DF.Business.Contracts;

namespace PersonProfile_DF.Business.Implementations
{
	internal class LookupProcessor : BaseProcessor, ILookupProcessor
	{
		internal LookupProcessor() : base()
		{
		}

		public async Task<IEnumerable<Person>> GetAllPeopleAsync()
		{
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				return await  dbContext.People.AsNoTracking().ToListAsync();
			}
		}

		public async Task<IEnumerable<Phone>> GetAllPhonesAsync()
		{
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				return await  dbContext.Phones.AsNoTracking().ToListAsync();
			}
		}

	}
}

