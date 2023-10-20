using Autofac;
using DataAuditing.Options;
using EntityGuardian.DependencyResolvers;
using EntityGuardian.Enums;
using EntityGuardian.Extensions;
using Microsoft.OpenApi.Models;
using Sample.Api.ApplicationSpecific;
using Sample.Api.ApplicationSpecific.Contexts;

namespace Sample.Api
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(Startup).Assembly));

            services.AddDbContext<MemoryDbContext>();

            services.AddTransient<ITestRepository, TestRepository>();

            services.AddDataAuditing(configuration =>
            {
                var dataAuditingOptions = new DataAuditingConfiguration
                {
                    StorageType = StorageType.MsSQL,
                    DashboardUrl = "deneme"
                };

                configuration.UseSqlServerStorage(Configuration.GetConnectionString("SqlServerConnection"));
            });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api4 v1"));
            }

            app.UseDataAuditing<MemoryDbContext>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DataAuditingBusinessModule());
        }
    }
}