using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Linq;
using System.Net.Mime;
using PersonProfile_DF.Business;
using PersonProfile_DF.Api.Exceptions;
using PersonProfile_DF.Api.Middlewares;

namespace PersonProfile_DF.Api
{
	public class Startup
	{
		private bool _isApiTraceEnabled = false;
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			var configurationSection = Configuration.GetSection("ApiProjectConfig").Get<ApiProjectConfig>();

			// Initialize AppConfiguration
			var configurationForProcessor = new AppConfiguration
			{
				ConnectionString = configurationSection.ConnectionString,
				ErrorLogDestination = configurationSection.ErrorLogDestination,
				IsDbQueryTraceEnabled = configurationSection.IsDbQueryTraceEnabled,
				DbQueryTraceDestination = configurationSection.DbQueryTraceDestination,
				TextLogDirectory = configurationSection.TextLogDirectory
			};
			App.Configure(configurationForProcessor);

			_isApiTraceEnabled = configurationSection.IsApiTraceEnabled;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var iConfigSection = Configuration.GetSection("ApiProjectConfig");
			var apiProjectConfig = iConfigSection.Get<ApiProjectConfig>();

			services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin() .AllowAnyMethod().AllowAnyHeader());});

			services.ControllerConfiguration();

			services.VersioningConfiguration();

			services.Configure<ApiProjectConfig>(options => iConfigSection.Bind(options));

			services.SwaggerConfiguration();

			services.AutoMapperConfiguration();

			services.CacheConfiguration(apiProjectConfig);

			services.MvcConfiguration(apiProjectConfig);

			services.AuthenticationConfiguration(apiProjectConfig);

			services.ServiceConfiguration();

			services.AddHealthChecks()
				.AddTypeActivatedCheck<HealthCheckDbMiddleware>("Database", HealthStatus.Unhealthy, new[] { "readiness" }, new object[] { apiProjectConfig.ConnectionString });

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonProfile_DF.Api v1"));
			}

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseResponseCaching();

			// Conditional execution of Caching Middleware: Cache shouldn't be used during HealthChecks
			app.UseWhen(context => !context.Request.Path.Value.Contains("/health") && !context.Request.Path.Value.Contains("favicon.ico"), appBuilder =>
			{
				appBuilder.UseMiddleware<ResponseCacheMiddleware>();
			});


			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseCors("CorsPolicy");

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			// Conditional execution of Tracing Middleware
			app.UseWhen(context => _isApiTraceEnabled && !context.Request.Path.Value.Contains("/health") && !context.Request.Path.Value.Contains("favicon.ico"), appBuilder =>
			{
				appBuilder.UseMiddleware<TracingMiddleware>();
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();

				// Health check endpoint for Liveness (API is running)
				endpoints.MapHealthChecks("/health/liveness", new HealthCheckOptions
				{
					AllowCachingResponses = false,
					Predicate = (_) => false
				});

				// Health check endpoint for Readiness (all the sub-components, environments, and any other external service is running)
				endpoints.MapHealthChecks("/health/readiness", new HealthCheckOptions
				{
					AllowCachingResponses = false,
					Predicate = (check) => check.Tags.Contains("readiness"),
					ResponseWriter = async(context, report) =>
					{
						var result = JsonSerializer.Serialize(
							new
							{
								status = report.Status.ToString(),
								checks = report.Entries.Select(entry => new
								{
									name = entry.Key,
									status = entry.Value.Status.ToString(),
									exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
									duration = entry.Value.Duration.ToString()
								})
							}
						);

						context.Response.ContentType = MediaTypeNames.Application.Json;
						await context.Response.WriteAsync(result);
					}
				});
			});
		}
	}
}

