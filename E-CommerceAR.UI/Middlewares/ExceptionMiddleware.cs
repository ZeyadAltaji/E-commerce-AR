using E_CommerceAR.UI.Errors;
using System.Net;

namespace E_CommerceAR.UI.Middlewares
{
    public class ExceptionMiddleware
    {
        //authorized
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next,
                                    ILogger<ExceptionMiddleware> logger,
                                    IHostEnvironment env)
        {
            this.env = env;
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                ErrorsAPIs response;
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                String message;
                var exceptionType = ex.GetType();

                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = "You are not authorized";
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Some unknown error occoured";
                }

                if (env.IsDevelopment())
                {
                    response = new ErrorsAPIs((int)statusCode, ex.Message, ex.StackTrace.ToString());
                }
                else
                {
                    response = new ErrorsAPIs((int)statusCode, message);
                }

                logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
        }
    }
}
