using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonProfile_DF.Api.Mappings;
using PersonProfile_DF.Business;
using PersonProfile_DF.Business.Contracts;
using PersonProfile_DF.Api.Exceptions;
using PersonProfile_DF.Api.Models;
using PersonProfile_DF.Api.Validators;
using PersonProfile_DF.Api.Caching;

namespace PersonProfile_DF.Api
{
	public static class ConfigureExtensions
	{
		public static void ServiceConfiguration(this IServiceCollection services)
		{
			services.AddScoped<IPersonProcessor>((x) => { return ProcessorFactory.CreatePersonProcessor(); });
			services.AddScoped<IPhoneProcessor>((x) => { return ProcessorFactory.CreatePhoneProcessor(); });
			services.AddScoped<IAuthenticateProcessor>((x) => { return ProcessorFactory.CreateAuthenticateProcessor(); });
			services.AddScoped<ILookupProcessor>((x) => { return ProcessorFactory.CreateLookupProcessor(); });
		}

		public static void VersioningConfiguration(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;
			});
		}

		public static void AutoMapperConfiguration(this IServiceCollection services)
		{
			IMapper mapper = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<ModelToResourceModelMappingProfile>();
				cfg.AddProfile<ResourceModelToModelMappingProfile>();
			}).CreateMapper();

			services.AddSingleton(mapper);
		}

		public static void ControllerConfiguration(this IServiceCollection services)
		{
			services.AddControllers(opt =>
			{
				// Removing formatter that turns nulls into 204 - No Content responses; otherwise, it will break Angular's Http response JSON parsing
				opt.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.HttpNoContentOutputFormatter>();
			})
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreatePersonValidator>())
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UpdatePersonValidator>())
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreatePhoneValidator>())
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UpdatePhoneValidator>());
		}

		public static void SwaggerConfiguration(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonProfile_DF.Api", Version = "v1" });

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme."
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{{
					new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}}
					, new string[] {}
				}});
			});
		}

		public static void CacheConfiguration(this IServiceCollection services, ApiProjectConfig configSettings)
		{
			// Adding ResponseCache for client-side caching
			services.AddResponseCaching(options  =>
			{
				options.UseCaseSensitivePaths = false;
				options.MaximumBodySize = 1024;
			});

			// Adding IMemoryCache for server-side caching {in-memory-cache on server}
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddMemoryCache();
		}

		public static void MvcConfiguration(this IServiceCollection services, ApiProjectConfig configSettings)
		{
			services.AddMvc(options =>
			{
				options.CacheProfiles.Add("Default30", new CacheProfile()
				{
					Duration = configSettings.ClientSideCacheExpirationInSeconds,
					VaryByQueryKeys = new[] { "*" }
				});
			});
		}

		public static void AuthenticationConfiguration(this IServiceCollection services, ApiProjectConfig configSettings)
		{
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = true;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configSettings.JwtIssuer,
					ValidAudience = configSettings.JwtAudience,
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configSettings.JwtSecret)),
					ClockSkew = TimeSpan.FromMinutes(1)
				};
			});
		}

	}
}

