using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using VacationRental.Api.Models;
using VacationRental.ApplicationServices;
using VacationRental.ApplicationServices.Services;
using VacationRental.DomainServices;
using VacationRental.Infraestructure;

namespace VacationRental.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //NOTE: Added to upgrade to .Net Core 3.1
            services.AddControllers();

            services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacation rental information", Version = "v1" }));

            //services.AddSingleton<IDictionary<int, RentalViewModel>>(new Dictionary<int, RentalViewModel>());
            //services.AddSingleton<IDictionary<int, BookingViewModel>>(new Dictionary<int, BookingViewModel>());
            services.AddSingleton(new VacationRentalContext());
            services.AddScoped<IBookingRep, BookingRep>();
            services.AddScoped<IRentalRep, RentalRep>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IRentalService, RentalService>();

            //Set Automapper configuration
            services.AddAutoMapper(c => c.AddProfile<VacationRentalMappingProfile>(), typeof(Startup));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //NOTE: Added to upgrade to .Net Core 3.1
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
              endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
        }
    }
}
