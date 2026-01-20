using Repair.Application.Common;
using Serilog;
using Serilog.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Repair.Api.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Result<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ArgumentException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        Log.Information(error, error?.Message);
                        break;
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message =  e.Message;
                        Log.Information(error, error?.Message);
                        break;
                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        Log.Information(error, error?.Message);
                        break;
                   
                    case UnauthorizedAccessException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        Log.Information(error, error?.Message);
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = "Unable to complete request, please try again.";
                        Log.Error(error, error?.Message);
                        break;
                }

                await response.WriteAsync(JsonSerializer.Serialize(responseModel));
            }
        }
    }
}
