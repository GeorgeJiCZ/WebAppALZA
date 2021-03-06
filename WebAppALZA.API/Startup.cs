using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WebAppALZA.API.Data;
using WebAppALZA.API.Extensions;
using WebAppALZA.API.Models;
using WebAppALZA.API.Repositories;

namespace WebAppALZA.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;                 
            } );

            string connection = Configuration.GetConnectionString("LocalDBConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            //app.UseCustomSwagger();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });             

            InitializeDBAsync(app);
                        
        }

        private void InitializeDBAsync(IApplicationBuilder app)
        {
            _ = Boolean.TryParse(Configuration.GetSection("InitializeDB").Value, out bool initDB);

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
            dbInitializer.Initialize();

            if (initDB) dbInitializer.SeedData();            
        }
    }
}
