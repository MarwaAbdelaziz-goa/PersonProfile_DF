using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Contracts
{
	public interface IPersonProcessor
	{
		Task<Person> GetPersonAsync(string userToken, int personId);

		Task<Tuple<IEnumerable<Person>, int>> GetAllPeopleAsync(string userToken, string sortBy, int? pageNumber, int? pageSize);

		Task<Person> SearchPersonAsync(string userToken, Expression<Func<Person, bool>> whereConditions, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties);

		Task<Tuple<IEnumerable<Person>, int>> SearchPeopleAsync(string userToken, Expression<Func<Person, bool>> whereConditions, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties);

		Task<Tuple<IEnumerable<Phone>, int>> GetPhonesAsync(string userToken, int personId, string sortBy, int? pageNumber, int? pageSize, bool isIncludeNonCollectionProperties, bool isIncludeCollectionProperties);

		Task<int> AddPersonAsync(string userToken, Person person);

		Task UpdatePersonAsync(string userToken, Person person, bool isUpdateCoreOnly);

		Task DeletePersonAsync(string userToken, int personId);

	}
}

