using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using OdataAPI.Controllers;
using OdataAPI.Data;
using Microsoft.OData;
using Microsoft.OData.ModelBuilder;
using System.Net.Http;
using System.Net;
using OdataAPI.ERPNext.Customer;

namespace OdataAPI
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
            services.AddDbContext<TestDbContext>(opt => opt.UseInMemoryDatabase("Test"));

            services.AddControllers(
                //mvcOptions => mvcOptions.EnableEndpointRouting = false
                );

            services.AddCors(options =>
            {
                options.AddPolicy(
                     "AllowAllOrigins",
                      builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        );

                options.DefaultPolicyName = "AllowAllOrigins";
            });

            services.AddOData();

            services.AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(20)
                .AddModel("odata", GetEdmModel(), builder => builder.AddService<ODataBatchHandler, DefaultODataBatchHandler>(Microsoft.OData.ServiceLifetime.Singleton))
                );

            services.AddHttpContextAccessor();

            services.AddHttpClient("erpnext", c =>
            {
                c.BaseAddress = new Uri("http://77.55.214.237/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("Authorization", "token 7d3713286bde8ff:0e52555369a84be");
            });
            //.AddTransientHttpErrorPolicy(p => p.RetryAsync(1)); ;

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseODataBatching();


            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();

      
            app.Use((context, next) =>
            {
                context.Response.Headers["OData-Version"] = "4.0";
                return next.Invoke();
            });

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.Namespace = "ErpNextService";
            odataBuilder.EntitySet<CustomerRoot>("Customer");


            return odataBuilder.GetEdmModel();
        }

  

    }
}
