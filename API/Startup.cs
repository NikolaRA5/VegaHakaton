using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Context;
using Handlers;
using Handlers.FacultyHandlers;
using Infrastructure.Modules;
using Infrastructure.UnitOfWork;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Repositories.Rooms;
using Services.DatabasePopulator;
using Services.RandomHexaGenerator;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });

            services.AddDbContextPool<AppDbContext>(optionsBuilder =>
            {
                AppDbContextFactory.ConfigureOptionsBuilder(optionsBuilder);

                using (var context = new AppDbContext(optionsBuilder.Options as DbContextOptions<AppDbContext>))
                {
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                        new DatabasePopulatorService().Populate(context);
                    }
                }
            });
            var builder = new ContainerBuilder();

            builder.RegisterModule(new RepositoryModule()
            {
                RepositoryAssemblies = new List<Assembly>()
                {
                    typeof(RoomReadRepository).Assembly
                },
                Namespace = ""
            });

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<Generator>().As<IGenerator>();

            builder.RegisterMediatR(typeof(GetAllFacultiesHandler).Assembly);
            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
