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
                      //  .AllowCredentials()
                        );

                options.DefaultPolicyName = "AllowAllOrigins";
            });

            services.AddOData();

            services.AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(20)
                .AddModel("odata", GetEdmModel(), builder => builder.AddService<ODataBatchHandler, DefaultODataBatchHandler>(Microsoft.OData.ServiceLifetime.Singleton))
                );

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

            //app.UseMvc(routes =>
            //{
            //    routes.Select().Expand().Filter().OrderBy().MaxTop(null).Count().EnableContinueOnErrorHeader();
            //    routes.MapODataServiceRoute("odata", "odata", GetEdmModel(), new DefaultODataBatchHandler());
            //    routes.EnableDependencyInjection();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

                if (db.Database.EnsureCreated())
                {
                    if (!db.Suppliers.Any())
                    {
                        db.Suppliers.AddRange(new List<Supplier> {
                        new Supplier { Id = 1, Name = "Stonka", Products =  new List<Product> {CreateNewProduct(1,"Cola", 130, "drink",1), CreateNewProduct(2,"Fanta", 140, "drink", 1) } },
                        new Supplier { Id = 2, Name = "Biedronka", Products =  new List<Product> {CreateNewProduct(3,"Pepsi", 130, "drink",2), CreateNewProduct(4,"Sprajt", 40, "drink", 2) } },

                         });
                        db.SaveChanges();
                    }
                }
            }


        }

        IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Product>("Products"); 
            odataBuilder.EntitySet<Supplier>("Suppliers");

            odataBuilder.Namespace = "ProductService";
            odataBuilder.EntityType<Product>().Action("Rate").Parameter<int>("Rating");
            odataBuilder.EntityType<Product>().Collection.Function("MostExpensive").Returns<double>();

            return odataBuilder.GetEdmModel();
        }

        public static Product CreateNewProduct(int id,string name, decimal price, string categ, int SupplierId)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Price = price,
                Category = categ,
                SupplierId = SupplierId
            };
        }

    }
}
