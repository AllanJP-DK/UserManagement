using Microsoft.AspNetCore.Mvc.Controllers;

namespace UserManagement.Services
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;

        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService, ICurrentUserService currentUserService)
        {
            // Process the request first
            await _next(context);

            try
            {
                // Only log successful requests (2xx status codes)
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    var endpoint = context.GetEndpoint();
                    if (endpoint != null)
                    {
                        var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                        if (controllerActionDescriptor != null)
                        {
                            var controllerName = controllerActionDescriptor.ControllerName;
                            var actionName = controllerActionDescriptor.ActionName;
                            var httpMethod = context.Request.Method;

                            _logger.LogInformation(
                                $"Processing audit for {httpMethod} {controllerName}/{actionName}"
                            );

                            string tableName = GetTableNameFromController(controllerName);
                            string operation = GetOperationFromHttpMethod(httpMethod);

                            if (
                                !string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(operation)
                            )
                            {
                                var userId = currentUserService.GetCurrentUserId() ?? Guid.Empty;
                                _logger.LogInformation($"Logging audit: {tableName} {operation} by user {userId}");
                                await auditService.LogAuditAsync(tableName, operation, userId);
                            }
                            else
                            {
                                _logger.LogWarning($"Not logging audit: tableName={tableName}, operation={operation}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("ControllerActionDescriptor not found");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Endpoint not found");
                    }
                }
                else
                {
                    _logger.LogInformation($"Not logging non-successful status code: {context.Response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't rethrow - don't break the request pipeline
                _logger.LogError(ex, "Error in AuditLogMiddleware");
            }
        }

        private string GetTableNameFromController(string controllerName)
        {
            // Remove "Controller" suffix and convert to lowercase for table name
            if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                return controllerName.Substring(0, controllerName.Length - 10).ToLowerInvariant();
            }
            return controllerName.ToLowerInvariant();
        }

        private string GetOperationFromHttpMethod(string httpMethod)
        {
            switch (httpMethod.ToUpper())
            {
                case "POST":
                    return "INSERT";
                case "PUT":
                case "PATCH":
                    return "UPDATE";
                case "DELETE":
                    return "DELETE";
                default:
                    return null; // Don't log GET requests
            }
        }
    }

    // Extension method to use the middleware
    public static class AuditLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLogMiddleware>();
        }
    }
}