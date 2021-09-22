using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace PersonProfile_DF.Api.Middlewares
{
    public class ResponseCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiProjectConfig _apiProjectConfig;

        public ResponseCacheMiddleware(RequestDelegate next, IOptions<ApiProjectConfig> apiProjectConfig)
        {
            _next = next;
            _apiProjectConfig = apiProjectConfig.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(_apiProjectConfig.ClientSideCacheExpirationInSeconds),
                Private = true, // a.k.a Location = "Client"
                Public = false // a.k.a Location = "Any"
            };

            var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();
            if (responseCachingFeature != null)
            {
                responseCachingFeature.VaryByQueryKeys = new[] { "*" };
            }

            await _next(context);
        }
    }
}

