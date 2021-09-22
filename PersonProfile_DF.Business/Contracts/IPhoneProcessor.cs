using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Contracts
{
	public interface IPhoneProcessor
	{
		Task<Phone> GetPhoneAsync(string userToken, int phoneId);

		Task<Tuple<IEnumerable<Phone>, int>> GetAllPhonesAsync(string userToken, string sortBy, int? pageNumber, int? pageSize);

		Task<Phone> SearchPhoneAsync(string userToken, Expression<Func<Phone, bool>> whereConditions, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties);

		Task<Tuple<IEnumerable<Phone>, int>> SearchPhonesAsync(string userToken, Expression<Func<Phone, bool>> whereConditions, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties);

		Task<int> AddPhoneAsync(string userToken, Phone phone);

		Task UpdatePhoneAsync(string userToken, Phone phone, bool isUpdateCoreOnly);

		Task DeletePhoneAsync(string userToken, int phoneId);

	}
}

