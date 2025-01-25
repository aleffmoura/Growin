namespace Growin.Api.Filters;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        var oDataParams = new string[]
         {
            "$count",
            "$expand",
            "$filter",
            "$orderBy",
            "$search",
            "$select",
            "$skip",
            "$top"
         };

        foreach (var item in oDataParams)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = item,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}