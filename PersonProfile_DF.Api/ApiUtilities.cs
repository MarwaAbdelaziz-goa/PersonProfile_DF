using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api
{
    public static class ApiUtilities
    {
        const string emptyStringRepresentation = "E";

        public static string GenerateCacheKey(string keyPrefix, QueryStringParameters qt)
        {
            if(string.IsNullOrEmpty(keyPrefix))
            {
                throw new Exception("cacheKeyPrefix cannot be empty.");
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"{keyPrefix}_");

            if (qt != null)
            {
                sb.Append($"PN_{qt.PageNumber}_");
                sb.Append($"PS_{qt.PageSize}_");
                sb.Append($"RF_{(string.IsNullOrEmpty(qt.RequestedFields) ? emptyStringRepresentation : qt.RequestedFields)}_");
                sb.Append($"SC_{(string.IsNullOrEmpty(qt.SortColumns) ? emptyStringRepresentation : qt.SortColumns)}");
            }
            else
            {
                sb.Append(emptyStringRepresentation);
            }

            return sb.ToString();
        }

        public static string GenerateCacheKey<T>(string keyPrefix, QueryStringParameters qt, T anyOtherObject)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateCacheKey(keyPrefix, qt));

            if(anyOtherObject != null)
            {
                sb.Append("_");

                var propertyInfoList = new List<PropertyInfo>();
                var propertyInfos = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);

                if (propertyInfos != null && propertyInfos.Count() > 0)
                {
                    int cnt = 0;
                    foreach (var singlePropertyInfo in propertyInfos)
                    {
                        cnt++;

                        object valueOfThisProperty = singlePropertyInfo.GetValue(anyOtherObject);

                        sb.Append(singlePropertyInfo.Name + "_" + (valueOfThisProperty == null ? emptyStringRepresentation : valueOfThisProperty.ToString()));

                        if(cnt != propertyInfos.Count())
                        {
                            sb.Append("_");
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}

