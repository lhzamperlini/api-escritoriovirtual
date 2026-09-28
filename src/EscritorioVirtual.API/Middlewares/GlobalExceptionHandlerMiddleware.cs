using System.Net;
using System.Text.Json;
using EscritorioVirtual.Application.Common.Exceptions;

namespace EscritorioVirtual.API.Middlewares;

public class GlobalExceptionHandlerMiddleware(
	RequestDelegate next,
	ILogger<GlobalExceptionHandlerMiddleware> logger,
	IHostEnvironment environment)
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	private static readonly JsonSerializerOptions JsonOptionsProduction = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = false
	};

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		if (context.Response.HasStarted)
		{
			logger.LogWarning("A resposta já foi iniciada, não é possível modificar o status code.");
			return;
		}

		var response = context.Response;
		response.ContentType = "application/json";

		var errorResponse = exception switch
		{
			NotFoundException notFoundEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.NotFound,
				Message = notFoundEx.Message,
				Details = environment.IsDevelopment() ? notFoundEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			BadRequestException badRequestEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.BadRequest,
				Message = badRequestEx.Message,
				Details = environment.IsDevelopment() ? badRequestEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			ValidationException validationEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.BadRequest,
				Message = validationEx.Message,
				Errors = validationEx.Errors,
				Details = environment.IsDevelopment() ? validationEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			CascadeDeleteException cascadeEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.BadRequest,
				Message = cascadeEx.Message,
				Details = environment.IsDevelopment() ? cascadeEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			UnauthorizedException unauthorizedEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.Unauthorized,
				Message = unauthorizedEx.Message,
				Details = environment.IsDevelopment() ? unauthorizedEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			ForbiddenException forbiddenEx => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.Forbidden,
				Message = forbiddenEx.Message,
				Details = environment.IsDevelopment() ? forbiddenEx.StackTrace : null,
				TraceId = context.TraceIdentifier
			},
			_ => new ErrorResponse
			{
				StatusCode = (int)HttpStatusCode.InternalServerError,
				Message = environment.IsDevelopment()
					? exception.Message
					: "Ocorreu um erro interno no servidor.",
				Details = environment.IsDevelopment() ? exception.StackTrace : null,
				TraceId = context.TraceIdentifier
			}
		};

		response.StatusCode = errorResponse.StatusCode;

		var options = environment.IsDevelopment() ? JsonOptions : JsonOptionsProduction;

		var json = JsonSerializer.Serialize(errorResponse, options);
		await response.WriteAsync(json);
	}
}
