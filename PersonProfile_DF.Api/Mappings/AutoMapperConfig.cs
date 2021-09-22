using System;
using AutoMapper;
using PersonProfile_DF.Api.Mappings;

namespace PersonProfile_DF.Api
{
	public static class AutoMapperConfig
	{
		public static MapperConfiguration RegisterProfiles()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<ModelToResourceModelMappingProfile>();
				cfg.AddProfile<ResourceModelToModelMappingProfile>();
			});

			return config;
		}
	}
}

