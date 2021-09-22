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

namespace PersonProfile_DF.Api.Middlewares
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiProjectConfig _apiProjectConfig;

        public TracingMiddleware(RequestDelegate next, IOptions<ApiProjectConfig> apiProjectConfig)
        {
            _next = next;
            _apiProjectConfig = apiProjectConfig.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            Utilities.Logger.LogTraceApiTraffic log = new();
            bool isErrorPresent = false;

            try
            {
                using (MemoryStream requestBodyStream = new MemoryStream())
                {
                    using (MemoryStream responseBodyStream = new MemoryStream())
                    {
                        Stream originalRequestBody = context.Request.Body;
                        context.Request.EnableBuffering();
                        Stream originalResponseBody = context.Response.Body;

                        try
                        {
                            var endpoint = context.GetEndpoint();
                            if(endpoint != null && endpoint.Metadata != null)
                            {
                                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                                if(controllerActionDescriptor != null)
                                {
                                    log.ControllerName = controllerActionDescriptor.ControllerName;
                                    log.ActionName = controllerActionDescriptor.ActionName;
                                }                                
                            }
                            
                            if(string.IsNullOrEmpty(log.ControllerName))
                            {
                                log.ControllerName = "NA";
                            }

                            if (string.IsNullOrEmpty(log.ActionName))
                            {
                                log.ActionName = "NA";
                            }

                            log.MethodName = context.Request.Method;
                            log.QueryString = context.Request.QueryString.ToString();
                            log.RequestUrl = context.Request.Scheme
                                            + "://" + context.Request.Host.Host
                                            + (context.Request.Host.Port == null ? "" : $":{context.Request.Host.Port}")
                                            + context.Request.Path;
                            log.DateTimeStampForRequest = DateTime.Now;
                            log.RequestHeaders = string.Join(",", context.Request.Headers.Select(he => he.Key + ":[" + he.Value + "]").ToList());

                            await context.Request.Body.CopyToAsync(requestBodyStream);
                            requestBodyStream.Seek(0, SeekOrigin.Begin);

                            log.RequestBody = new StreamReader(requestBodyStream).ReadToEnd();

                            requestBodyStream.Seek(0, SeekOrigin.Begin);
                            context.Request.Body = requestBodyStream;
                            context.Response.Body = responseBodyStream;

                            await _next(context);

                            responseBodyStream.Seek(0, SeekOrigin.Begin);
                            log.ResponseBody = new StreamReader(responseBodyStream).ReadToEnd();
                            log.ResponseHeaders = string.Join(",", context.Response.Headers.Select(he => he.Key + ":[" + he.Value + "]").ToList());

                            responseBodyStream.Seek(0, SeekOrigin.Begin);

                            await responseBodyStream.CopyToAsync(originalResponseBody);
                        }
                        finally
                        {
                            log.DateTimeStampForResponse = DateTime.Now;
                            log.ResponseHeaders = string.Join(",", context.Response.Headers.Select(he => he.Key + ":[" + he.Value + "]").ToList());

                            context.Request.Body = originalRequestBody;
                            context.Response.Body = originalResponseBody;
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                isErrorPresent = true;
                log.ErrorIfAny = exp.ToString();
                
                throw;
            }
            finally
            {
                log.IsSuccessStatusCode = !isErrorPresent;
                log.UserInformation = context != null && context.User != null && context.User.Identity != null && !string.IsNullOrEmpty(context.User.Identity.Name) ? context.User.Identity.Name : null;
                Utilities.Logger.CaptureApiTrace(_apiProjectConfig.ApiTraceDestination, _apiProjectConfig.TextLogDirectory, _apiProjectConfig.ConnectionString, log);
            }
        }
    }
}

