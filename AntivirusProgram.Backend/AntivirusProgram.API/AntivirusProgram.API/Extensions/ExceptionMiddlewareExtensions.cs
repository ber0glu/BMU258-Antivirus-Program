using AntivirusProgram.Entities.ErrorModel;
using AntivirusProgram.Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace AntivirusProgram.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature is not null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            ConflictException => StatusCodes.Status409Conflict, //409 çakışma
                            NotFoundException => StatusCodes.Status404NotFound, //404 bululanamadı
                            NotAVirusException => StatusCodes.Status400BadRequest, //400 virus değil
                            _ => StatusCodes.Status500InternalServerError
                        };


                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
