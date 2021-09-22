using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace PersonProfile_DF.Business.Data.Context
{
	internal static class DbProcessorEF<TEntity> where TEntity : class
	{
		internal async static Task<TEntity> FindSingleByConditionsAsync (DbContext _dbContext, bool isTrackingEnabled, Expression<Func<TEntity, bool>> whereConditions, bool isMultipleQueriesEnabledForIncludes = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties = null)
		{			
			var recordsFound = await FindManyByConditionsAsync(_dbContext, isTrackingEnabled, whereConditions, null, null, null, isMultipleQueriesEnabledForIncludes, includeProperties);
			return recordsFound.Item1.FirstOrDefault();
		}

		internal static  async Task<Tuple<IEnumerable<TEntity>, int>> FindManyByConditionsAsync(
			DbContext _dbContext, bool isTrackingEnabled, Expression<Func<TEntity, bool>> whereConditions,
			string orderByStatements, int? pageNumber, int? pageSize,
			bool isMultipleQueriesEnabledForIncludes = false, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties = null)
		{			
			int totalNumberOfRecords = 0;
			IEnumerable<TEntity> records = new List<TEntity>();

			IQueryable<TEntity> query = _dbContext.Set<TEntity>();

			// Filtering by conditions
			if (whereConditions != null)
			{
				query = query.Where(whereConditions);
			}

			// Including related properties (if required)
			if (includeProperties != null)
			{
				if(isMultipleQueriesEnabledForIncludes)
				{
					query = query.AsSplitQuery();
				}

				query = includeProperties(query);
			}

			// Sorting
			if (!string.IsNullOrEmpty(orderByStatements))
			{
				query = query.OrderBySingleOrMultipleColumns<TEntity>(orderByStatements);
			}

			// Paging (if required)
			if (pageNumber != null && pageNumber > 0 && pageSize != null && pageSize > 0)
			{
				if (isTrackingEnabled)
				{
					records = query.Pagination<TEntity, object>((int)pageNumber, (int)pageSize, out totalNumberOfRecords);
				}
				else
				{
					records = query.AsNoTracking().Pagination<TEntity, object>((int)pageNumber, (int)pageSize, out totalNumberOfRecords);
				}
			}
			else
			{
				if (isTrackingEnabled)
				{
					records = await query.ToListAsync();
				}
				else
				{
					records = await query.AsNoTracking().ToListAsync();
				}

				totalNumberOfRecords = records.Count();
			}

			return Tuple.Create(records, totalNumberOfRecords);
		}
	}
}

