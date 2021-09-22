using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.Mappings
{
	public class ResourceModelToModelMappingProfile : Profile
	{

		public ResourceModelToModelMappingProfile()
		{
			#region Person

			CreateMap<PersonDtl, Person>();

			CreateMap<CreatePerson, Person>();

			CreateMap<UpdatePerson, Person>();

			#endregion

			#region Phone

			CreateMap<PhoneDtl, Phone>();

			CreateMap<CreatePhone, Phone>();

			CreateMap<UpdatePhone, Phone>();

			#endregion

		}
	}
}

