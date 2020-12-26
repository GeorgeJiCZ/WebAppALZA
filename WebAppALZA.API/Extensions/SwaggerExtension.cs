using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace WebAppALZA.API.Extensions
{
    public static class SwaggerExtension
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API - products",
                    Description = "ASP.NET Core 3.1 REST Web API (C# 8) - get and update product(s)",                    
                    Contact = new OpenApiContact() { Name = "Jiri Vyskocil", Email = "jivyskocil@gmail.com" }
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "API - products",
                    Description = "ASP.NET Core 3.1 REST Web API (C# 8) - get and update product(s) and product pagination",                    
                    Contact = new OpenApiContact() { Name = "Jiri Vyskocil", Email = "jivyskocil@gmail.com" }
                });
                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Version = "v3",
                    Title = "API - products",
                    Description = "ASP.NET Core 3.1 REST Web API (C# 8) - get and update product(s) and product pagination",
                    Contact = new OpenApiContact() { Name = "Jiri Vyskocil", Email = "jivyskocil@gmail.com" }
                });
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // nefunguje
                //c.DocumentFilter<JsonPatchDocumentFilter>();
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "API v3");
            });
        }
    }

    public class JsonPatchDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //Remove irrelevent schemas
            var schemas = swaggerDoc.Components.Schemas.ToList();
            foreach (var item in schemas)
            {
                if (item.Key.StartsWith("OperationOf") || item.Key.StartsWith("JsonPatchDocumentOf"))
                    swaggerDoc.Components.Schemas.Remove(item.Key);
            }

            //Add accurate PatchDocument schema
            swaggerDoc.Components.Schemas.Add("Operation", new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    {"op", new OpenApiSchema{ Type = "string" } },
                    {"value", new OpenApiSchema{ Type = "object", Nullable = true } },
                    {"path", new OpenApiSchema{ Type = "string" } }
                }
            });

            swaggerDoc.Components.Schemas.Add("JsonPatchDocument", new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Operation" }
                },
                Description = "Array of operations to perform"
            });

            //Fix up the patch references
            foreach (var path in swaggerDoc.Paths.SelectMany(p => p.Value.Operations).Where(p => p.Key == OperationType.Patch))
            {
                foreach (var item in path.Value.RequestBody.Content.Where(c => c.Key != "application/json-patch+json"))
                    path.Value.RequestBody.Content.Remove(item.Key);

                var response = path.Value.RequestBody.Content.Single(c => c.Key == "application/json-patch+json");
                response.Value.Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "JsonPatchDocument" }
                };
            }
        }
    }
}
