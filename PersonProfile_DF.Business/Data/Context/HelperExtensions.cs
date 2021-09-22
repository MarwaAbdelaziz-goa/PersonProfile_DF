using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PersonProfile_DF.Business.Data.Context
{
    public static class HelperExtensions
    {
        public static IQueryable<TEntity> OrderBySingleOrMultipleColumns<TEntity>(this IQueryable<TEntity> source, string orderByQuery)
        {
            if (string.IsNullOrEmpty(orderByQuery))
            {
                return source;
            }

            // Replacing multiple spaces (including newlines, tabs) with single space
            orderByQuery = Regex.Replace(orderByQuery, @"\s+", " ");

            // Splitting multiple order by statements
            List<string> listOfAllOrderByStatements = orderByQuery.Split(',').ToList();

            if (listOfAllOrderByStatements == null || listOfAllOrderByStatements.Count() == 0)
            {
                return source;
            }

            int orderClauseCounter = 0;
            foreach (var singleOrderBy in listOfAllOrderByStatements)
            {
                // Again splitting by space to retrieve the orderByColumn and the direction (ascending or descending)
                List<string> queryWithSortDirection = singleOrderBy.Trim().Split(' ').ToList();

                if (queryWithSortDirection != null && queryWithSortDirection.Count >= 1 && queryWithSortDirection.Count < 3)
                {
                    string sortBy = queryWithSortDirection[0];
                    bool? isSortDirectionAscending = null;

                    if (queryWithSortDirection.Count() == 1)
                    {
                        isSortDirectionAscending = true;
                    }
                    else if (queryWithSortDirection.Count() == 2)
                    {
                        if (queryWithSortDirection[1].ToLower() == "asc")
                        {
                            isSortDirectionAscending = true;
                        }
                        else if (queryWithSortDirection[1].ToLower() == "desc")
                        {
                            isSortDirectionAscending = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(sortBy) && isSortDirectionAscending != null)
                    {
                        orderClauseCounter = orderClauseCounter + 1;

                        source = OrderBySingleColumn(source, sortBy, (bool)isSortDirectionAscending, orderClauseCounter > 1 ? true : false);
                    }
                }
            }

            return source;
        }

        public static IEnumerable<T> Pagination<T, TResult>(this IQueryable<T> orderedQuery, int pageNumber, int pageSize, out int totalRowsCountWithoutPaging)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new Exception("Invalid PageNumber and/or PageSize");
            }

            totalRowsCountWithoutPaging = orderedQuery.Count();
            int skip = ((int)pageNumber - 1) * (int)pageSize;
            int take = (int)pageSize;

            var results = orderedQuery.Skip(skip).Take(take).ToList();

            return results;
        }

        private static IQueryable<TEntity> OrderBySingleColumn<TEntity>(IQueryable<TEntity> source, string orderByProperty, bool isAscendingDirection, bool isSubsequentOrderingRequested)
        {
            Type typeOfEntity, typeOfProperty;
            var expression = GetExpressionForProperty<TEntity>(orderByProperty, out typeOfEntity, out typeOfProperty);

            MethodCallExpression resultExpression;

            if (isSubsequentOrderingRequested)
            {
                resultExpression = Expression.Call(typeof(Queryable), isAscendingDirection ? "ThenBy" : "ThenByDescending", new Type[] { typeOfEntity, typeOfProperty }, source.Expression, Expression.Quote(expression));
            }
            else
            {
                resultExpression = Expression.Call(typeof(Queryable), isAscendingDirection ? "OrderBy" : "OrderByDescending", new Type[] { typeOfEntity, typeOfProperty }, source.Expression, Expression.Quote(expression));
            }

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        private static LambdaExpression GetExpressionForProperty<TEntity>(string propertyName, out Type typeOfEntity, out Type typeOfProperty)
        {
            typeOfEntity = typeof(TEntity);
            var property = typeOfEntity.GetProperty(propertyName);

            if (property == null)
                throw new Exception($"Column ({propertyName}) not present in ({typeOfEntity.Name}) object.");

            typeOfProperty = property.PropertyType;

            var parameter = Expression.Parameter(typeOfEntity, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var expression = Expression.Lambda(propertyAccess, parameter);

            return expression;
        }
    }

}

