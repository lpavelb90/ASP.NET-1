using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using System.Diagnostics;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationContext>(opt =>
                //opt.UseSqlite("Data Source=db\\promocodefactory.db")
                opt.UseNpgsql(_configuration.GetConnectionString("PromoCodeFactoryDb"))
                .LogTo(message => Debug.WriteLine(message)));

            services.AddTransient(typeof(IRepository<,>), typeof(ApplicationEfRepository<,>));
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //EnsureDbCreated(app);
            DbMigrate(app);

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void DbMigrate(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                ctx.Database.Migrate();

                ctx.SeedData<Role>(FakeDataFactory.Roles);
                ctx.SeedData<Employee>(FakeDataFactory.Employees);
                ctx.SeedData<Customer>(FakeDataFactory.Customers);
                ctx.SeedData<Preference>(FakeDataFactory.Preferences);
            }
        }

        public void EnsureDbCreated(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                ctx.EnsureDBCreatedWithData<Role>(FakeDataFactory.Roles, true);
                ctx.EnsureDBCreatedWithData<Employee>(FakeDataFactory.Employees);
                ctx.EnsureDBCreatedWithData<Customer>(FakeDataFactory.Customers);
                ctx.EnsureDBCreatedWithData<Preference>(FakeDataFactory.Preferences);
            }
        }
    }
}