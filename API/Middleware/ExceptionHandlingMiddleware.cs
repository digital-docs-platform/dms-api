using API.DTO.Response;
using Application.Exceptions;
using Application.Logging;
using System.ComponentModel.DataAnnotations;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IErrorLogger _errorLogger;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            //_errorLogger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Call the next delegate/middleware in the pipeline.
            try
            {
                await _next(context);
            }
            //catch (ValidationException ex)
            //{
            //    context.Response.StatusCode = 422;

            //    var errors = ex.Errors.Select(x => new
            //    {
            //        x.ErrorMessage,
            //        x.PropertyName
            //    });

            //    await context.Response.WriteAsJsonAsync(errors);
            //}
            //catch (UnauthorizedUseCaseExecutionException ex)
            //{
            //    context.Response.StatusCode = 401;
            //}
            //catch (UnauthorizedAccessException ex)
            //{
            //    context.Response.StatusCode = 401;
            //}
            //catch (EntityNotFoundException ex)
            //{
            //    context.Response.StatusCode = 404;
            //    await context.Response.WriteAsJsonAsync(new
            //    {
            //        message = ex.Message
            //    });
            //}
            catch(UnauthorizedException ex)
            {
                context.Response.StatusCode = 401;

                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                Guid errorId = Guid.NewGuid();
                //AppError error = new AppError
                //{
                //    Exception = ex,
                //    ErrorId = errorId,
                //    Email = "test@gmail.com"
                //};
                //_errorLogger.Log(error);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Message = $"There was an error, please contact support with this error code: {errorId}.",
                    Data = new { errorId, traceId = context.TraceIdentifier, message = ex.Message }
                });
            }
        }
    }
}
