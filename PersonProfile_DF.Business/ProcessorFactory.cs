using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Contracts;
using PersonProfile_DF.Business.Implementations;

namespace PersonProfile_DF.Business
{
	public static class ProcessorFactory
	{
		public static IPersonProcessor CreatePersonProcessor()
		{
			return new PersonProcessor();
		}

		public static IPhoneProcessor CreatePhoneProcessor()
		{
			return new PhoneProcessor();
		}

		public static ILookupProcessor CreateLookupProcessor()
		{
			return new LookupProcessor();
		}

		public static IAuthenticateProcessor CreateAuthenticateProcessor()
		{
			return new AuthenticateProcessor();
		}

	}
}

