using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.Mappings
{
	public class ModelToResourceModelMappingProfile : Profile
	{
		public ModelToResourceModelMappingProfile()
		{
			#region Person

			CreateMap<Person, PersonCore>();

			CreateMap<Person, PersonSearchResult>();

			CreateMap<Person, PersonDtl>();

			CreateMap<Person, CreatePerson>();

			CreateMap<Person, UpdatePerson>();

			#endregion

			#region Phone

			CreateMap<Phone, PhoneCore>();

			CreateMap<Phone, PhoneSearchResult>()
				.ForMember(dest => dest.Person_Name, opts => opts.MapFrom(src => src.Person.Name));

			CreateMap<Phone, PhoneDtl>();

			CreateMap<Phone, CreatePhone>();

			CreateMap<Phone, UpdatePhone>();

			#endregion

		}
	}
}

