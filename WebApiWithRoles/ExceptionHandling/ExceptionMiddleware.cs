﻿using System.Net;

namespace WebApiWithRoles.ExceptionHandling;

public class ExceptionMiddleware(RequestDelegate requestDelegate)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await requestDelegate(httpContext);
        }
        catch (Exception)
        {
            // Log error
            await HandleExceptionAsync(httpContext);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var message = exception switch
        {
            AccessViolationException => "Access violation exception",
            _ => "Internal server error"
        };

        await httpContext.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = "Internal server error",
        }.ToString());
    }
}