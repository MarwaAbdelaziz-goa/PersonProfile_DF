using System;
using System.Collections.Generic;
using System.Linq;
namespace PersonProfile_DF.Api
{
	public class ApiProjectConfig
	{
		public string ConnectionString { get; set; }
		public string ErrorLogDestination { get; set; }
		public bool IsApiTraceEnabled { get; set; }
		public string ApiTraceDestination { get; set; }
		public bool IsDbQueryTraceEnabled { get; set; }
		public string DbQueryTraceDestination { get; set; }
		public string TextLogDirectory { get; set; }
		public string JwtIssuer { get; set; }
		public string JwtAudience { get; set; }
		public string JwtSecret { get; set; }
		public int JwtTokenExpirationTimeInSeconds { get; set; }
		public int ServerSideCacheExpirationInSeconds { get; set; }
		public int ClientSideCacheExpirationInSeconds { get; set; }
	}
}

