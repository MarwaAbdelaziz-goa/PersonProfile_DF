using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PersonProfile_DF.Api.Exceptions;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiProjectConfig _apiProjectConfig;

        public ExceptionMiddleware(RequestDelegate next, IOptions<ApiProjectConfig> apiProjectConfig)
        {
            _next = next;
            _apiProjectConfig = apiProjectConfig.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exp)
            {
                Utilities.Logger.CaptureErrorLog(_apiProjectConfig.ErrorLogDestination, _apiProjectConfig.TextLogDirectory, _apiProjectConfig.ConnectionString, exp, Guid.NewGuid());
                await HandleExceptionAsync(context, exp);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            switch (exception)
            {
                case AppException e:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Error from API."
            }.ToString());
        }
    }
}

