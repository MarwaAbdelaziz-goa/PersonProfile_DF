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
	internal class PhoneProcessor : BaseProcessor, IPhoneProcessor
	{
		internal PhoneProcessor() : base()
		{
		}

		public async Task<Phone> GetPhoneAsync(string userToken, int phoneId)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var phone = await  DbProcessorEF<Phone>.FindSingleByConditionsAsync(dbContext, false, x => x.PhoneId == phoneId);

				return phone;
			}
		}

		public async Task<Tuple<IEnumerable<Phone>, int>> GetAllPhonesAsync(string userToken, string sortBy, int? pageNumber, int? pageSize)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var phones = await DbProcessorEF<Phone>.FindManyByConditionsAsync(dbContext, false, null, sortBy, pageNumber, pageSize);

				return phones;
			}
		}

		public async Task<Phone> SearchPhoneAsync(string userToken, Expression<Func<Phone, bool>> whereConditions, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPhone(isIncludeNonCollectionProperties, isIncludeCollectionProperties);

				var phone = await DbProcessorEF<Phone>.FindSingleByConditionsAsync(dbContext, false, whereConditions, true, includes);

				return phone;
			}
		}

		public async Task<Tuple<IEnumerable<Phone>, int>> SearchPhonesAsync(string userToken, Expression<Func<Phone, bool>> whereConditions, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPhone(isIncludeNonCollectionProperties, isIncludeCollectionProperties);

				var phones = await DbProcessorEF<Phone>.FindManyByConditionsAsync(dbContext, false, whereConditions, sortBy, pageNumber, pageSize, true, includes);

				return phones;
			}
		}

		public async Task<int> AddPhoneAsync(string userToken, Phone phone)
		{
			App.ValidateUserToken(userToken);
			
			PhoneValidator.Validate(phone, true, true);

			using (var dbContext = new PersonProfile_DFDbContext())
			{

				dbContext.Phones.Add(phone);
				await dbContext.SaveChangesAsync();
				return phone.PhoneId;
			}
		}

		public async Task UpdatePhoneAsync(string userToken, Phone phone, bool isUpdateCoreOnly)
		{
			App.ValidateUserToken(userToken);
			
			PhoneValidator.Validate(phone, false, true);

			using (var dbContext = new PersonProfile_DFDbContext())
			{
				// Loading existing entity to compare
				var includes = CalculateIncludablePropertiesForPhone(!isUpdateCoreOnly, !isUpdateCoreOnly);

				var existingPhone = await DbProcessorEF<Phone>.FindSingleByConditionsAsync(dbContext, true, x => x.PhoneId == phone.PhoneId, true, includes);

				if(existingPhone != null)
				{
					dbContext.Entry(existingPhone).CurrentValues.SetValues(phone);

					// Saving Transaction
					await dbContext.SaveChangesAsync();
				}
				else
				{
					throw new Exceptions.CustomBusinessException("Phone not found in database.");
				}
			}
		}

		public async Task DeletePhoneAsync(string userToken, int phoneId)
		{
			App.ValidateUserToken(userToken);
			
			using (var dbContext = new PersonProfile_DFDbContext())
			{
				var includes = CalculateIncludablePropertiesForPhone(true, true);

				var existingPhone = await DbProcessorEF<Phone>.FindSingleByConditionsAsync(dbContext, true, x => x.PhoneId == phoneId, true, includes);

				if (existingPhone != null)
				{

					dbContext.Phones.Remove(existingPhone);
					dbContext.Entry(existingPhone).State = EntityState.Deleted;
					await dbContext.SaveChangesAsync();
				}
				else
				{
					throw new Exceptions.CustomBusinessException("Phone not found in database.");
				}
			}
		}

		
		private static Func<IQueryable<Phone>, IIncludableQueryable<Phone, object>> CalculateIncludablePropertiesForPhone(bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties)
		{
			Func<IQueryable<Phone>, IIncludableQueryable<Phone, object>> includes = null;

			if (isIncludeNonCollectionProperties && isIncludeCollectionProperties)
			{				
				includes = source => source
					.Include(p => p.Person);
			}
			else if (isIncludeNonCollectionProperties)
			{				
				includes = source => source
					.Include(p => p.Person);
			}
			else if (isIncludeCollectionProperties)
			{
				// Nothing to include
			}

			return includes;
		}
	}
}

