using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WebApplication2.Middlewares;
using WebApplication2.Models;
using WebApplication2.Services;
using static System.Net.WebRequestMethods;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Zarejestrowanie interfejsu w kontenerze zaleznosci
            services.AddScoped<IDbService, SqlServerDbService>();
            services.AddControllers(config =>
            {
                config.Filters.Add(typeof(CustomExceptionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbService dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ExceptionMiddleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            // LoggingMiddleware
            // Zadanie 2
            app.UseMiddleware<LoggingMiddleware>();

            // Zadanie 1
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podano indeksu");
                    return;
                }

                string index = context.Request.Headers["Index"].ToString();
                var stu = dbService.CheckIndexNumber(index);
                if (stu == null) 
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Nie istnieje student o podanym indeksie");
                    return;
                }
                await next();
            });
            
            app.UseRouting();  

            app.UseEndpoints(endpoints => // GetStudents()
            {
                endpoints.MapControllers();
            });
        }
    }
}
