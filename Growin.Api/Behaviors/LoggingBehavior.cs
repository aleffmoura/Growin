namespace Growin.Api.Behaviors;

using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using MediatR;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, Result<TResponse>>> logger)
         : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, Result<TResponse>>> _logger = logger;

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        _logger.BeginScope("Handler {handlerName}", typeof(TRequest).Name);

        var response = await next();

        _logger.LogInformation("Request: {requestData} has response: {responseData}", request, response);

        response.Else(fail =>
        {
            if (fail is UnhandledError internalError)
            {
                _logger.LogCritical("InternalException handled");
                _logger.LogCritical("message: {MessageException}, exception: {Exception}", internalError.Message, internalError.Exception);
                _logger.LogCritical("InnerException: {InnerException}", internalError.Exception?.InnerException);
            }
            else if (fail is BaseError baseError)
            {
                _logger.LogError("BusinessError handled");
                _logger.LogError("message: {MessageError}, error: {Error}", baseError.Message, baseError.Exception);
            }
        });

        return response;
    }
}