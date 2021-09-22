using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PersonProfile_DF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Business.Data.Context;
using PersonProfile_DF.Business.Contracts;
using PersonProfile_DF.Business.Validators;

namespace PersonProfile_DF.Business.Implementations
{
	internal class PersonProcessor : BaseProcessor, IPersonProcessor
	{
		internal PersonProcessor() : base()
		{
		}

		public async Task<Person> GetPersonAsync(string userToken, int personId)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var person = await  DbProcessorEF<Person>.FindSingleByConditionsAsync(dbContext, false, x => x.PersonId == personId);

				return person;
			}
		}

		public async Task<Tuple<IEnumerable<Person>, int>> GetAllPeopleAsync(string userToken, string sortBy, int? pageNumber, int? pageSize)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var people = await DbProcessorEF<Person>.FindManyByConditionsAsync(dbContext, false, null, sortBy, pageNumber, pageSize);

				return people;
			}
		}

		public async Task<Person> SearchPersonAsync(string userToken, Expression<Func<Person, bool>> whereConditions, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPerson(isIncludeNonCollectionProperties, isIncludeCollectionProperties);

				var person = await DbProcessorEF<Person>.FindSingleByConditionsAsync(dbContext, false, whereConditions, true, includes);

				return person;
			}
		}

		public async Task<Tuple<IEnumerable<Person>, int>> SearchPeopleAsync(string userToken, Expression<Func<Person, bool>> whereConditions, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPerson(isIncludeNonCollectionProperties, isIncludeCollectionProperties);

				var people = await DbProcessorEF<Person>.FindManyByConditionsAsync(dbContext, false, whereConditions, sortBy, pageNumber, pageSize, true, includes);

				return people;
			}
		}

		public async Task<Tuple<IEnumerable<Phone>, int>> GetPhonesAsync(string userToken, int personId, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var phones = await DbProcessorEF<Phone>.FindManyByConditionsAsync(dbContext, false, x => x.PersonId == personId, sortBy, pageNumber, pageSize);

				return phones;
			}
		}

		public async Task<int> AddPersonAsync(string userToken, Person person)
		{
			App.ValidateUserToken(userToken);
			
			PersonValidator.Validate(person, true, true);

			using (var dbContext = new PersonProfile_DFDbContext())
			{

				dbContext.People.Add(person);
				await dbContext.SaveChangesAsync();
				return person.PersonId;
			}
		}

		public async Task UpdatePersonAsync(string userToken, Person person, bool isUpdateCoreOnly)
		{
			App.ValidateUserToken(userToken);
			
			PersonValidator.Validate(person, false, true);

			using (var dbContext = new PersonProfile_DFDbContext())
			{
				// Loading existing entity to compare
				var includes = CalculateIncludablePropertiesForPerson(!isUpdateCoreOnly, !isUpdateCoreOnly);

				var existingPerson = await DbProcessorEF<Person>.FindSingleByConditionsAsync(dbContext, true, x => x.PersonId == person.PersonId, true, includes);

				if(existingPerson != null)
				{
					dbContext.Entry(existingPerson).CurrentValues.SetValues(person);

					// Saving Transaction
					await dbContext.SaveChangesAsync();
				}
				else
				{
					throw new Exceptions.CustomBusinessException("Person not found in database.");
				}
			}
		}

		public async Task DeletePersonAsync(string userToken, int personId)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPerson(true, true);

				var existingPerson = await DbProcessorEF<Person>.FindSingleByConditionsAsync(dbContext, true, x => x.PersonId == personId, true, includes);

				if (existingPerson != null)
				{

					dbContext.People.Remove(existingPerson);
					dbContext.Entry(existingPerson).State = EntityState.Deleted;
					await dbContext.SaveChangesAsync();
				}
				else
				{
					throw new Exceptions.CustomBusinessException("Person not found in database.");
				}
			}
		}

		
		private static Func<IQueryable<Person>, IIncludableQueryable<Person, object>> CalculateIncludablePropertiesForPerson(bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			Func<IQueryable<Person>, IIncludableQueryable<Person, object>> includes = null;

			if (isIncludeNonCollectionProperties && isIncludeCollectionProperties)
			{				
				includes = source => source
					.Include(p => p.Phones);
			}
			else if (isIncludeNonCollectionProperties)
			{
				// Nothing to include
			}
			else if (isIncludeCollectionProperties)
			{				
				includes = source => source
					.Include(p => p.Phones);
			}

			return includes;
		}
	}
}

