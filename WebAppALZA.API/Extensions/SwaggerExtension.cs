using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
                    Title = "Alza API - products",
                    Description = "ASP.NET Core 3.1 REST Web API (C# 8) - get and update product(s)",                    
                    Contact = new OpenApiContact() { Name = "Jiri Vyskocil", Email = "jivyskocil@gmail.com" }
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "Alza API - products",
                    Description = "ASP.NET Core 3.1 REST Web API (C# 8) - get and update product(s) and product pagination",                    
                    Contact = new OpenApiContact() { Name = "Jiri Vyskocil", Email = "jivyskocil@gmail.com" }
                });
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());                 
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alza API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Alza API v2");                 
            });
        }
    }
}
