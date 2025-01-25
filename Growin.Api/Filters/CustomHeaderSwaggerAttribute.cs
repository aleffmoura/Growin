namespace Growin.Api.Filters;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Numerics;

public class CustomHeaderSwaggerAttribute : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var oDataParamsInfo = context.MethodInfo.GetParameters()
            .Where(prm => prm.ParameterType.IsGenericType &&
                  prm.ParameterType.GetGenericTypeDefinition() == typeof(ODataQueryOptions<>))
            .ToList();

        if (!oDataParamsInfo.Any())
            return;

        var oDataParamsNames = oDataParamsInfo.Select(d => d.Name);
        var paramsRemove = operation.Parameters.Where(op => oDataParamsNames.Contains(op.Name)).ToList();
        paramsRemove.ForEach(item => operation.Parameters.Remove(item));

        List<(string name, string type)> oDataParams =
        [
            ("$count", nameof(Boolean)),
            ("$expand", "string"),
            ("$filter", "string"),
            ("$orderBy", "string"),
            ("$search", "string"),
            ("$select", "string"),
            ("$skip", "Integer"),
            ("$top", "Integer")
        ];

        foreach (var (name, type) in oDataParams)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = name,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = type }
            });
        }
    }
}