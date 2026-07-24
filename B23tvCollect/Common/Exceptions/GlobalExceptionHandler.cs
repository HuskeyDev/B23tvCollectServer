using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace B23tvCollect.Common.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails();
            if (exception is BusinessException businessException)
            {
                var errCode = businessException.ErrCode;
                problemDetails.Title = businessException.Message;
                problemDetails.Detail = businessException.Message;
                problemDetails.Extensions["serviceStatus"] = errCode;

                if (errCode == 600 ||
                    errCode == 601 ||
                    errCode == 602)
                {
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else if (errCode == 604)
                {
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else if (errCode == 603)
                {
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                }
            }
            else
            {
                var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                Log.Error(exception, "发生时间:{Time}", time);
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "内部错误" + time;
                problemDetails.Detail = "内部错误" + time;
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

    }
}
