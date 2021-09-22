using System;

namespace PersonProfile_DF.Business
{
	public class AppConfiguration
	{
		internal string ConsumerUserToken { get; set; }
		public string ConnectionString { get; set; }
		public string ErrorLogDestination { get; set; }
		public bool IsDbQueryTraceEnabled { get; set; }
		public string DbQueryTraceDestination { get; set; }
		public string TextLogDirectory { get; set; }
	}
}

