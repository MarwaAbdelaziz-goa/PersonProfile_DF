using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Reflection;

namespace PersonProfile_DF.Utilities
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ExceptBy<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other,  Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                   equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(T))
                   select item;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source?.GroupBy(keySelector).Select(grp => grp.First());
        }

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // create a list to hold ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // Create a list with PropertyInfo objects on TSource. For performance reasons, reflection is used once and then reusing it
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // the field are separated by ",", so we split it.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    // Use reflection to get the property on the source object
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }

                    // add propertyInfo to list 
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach (TSource sourceObject in source)
            {
                // create an ExpandoObject that will hold the selected properties & values
                var dataShapedObject = new ExpandoObject();

                // Get the value of each property we have to return.
                foreach (var propertyInfo in propertyInfoList)
                {
                    // GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                // add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            // return the list
            return expandoObjectList;
        }
    }    


}

