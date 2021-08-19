using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Subscriptions.Data;
using Subscriptions.Domain.Services;
using Subscriptions.Infrastructure;
using Subscriptions.Services;

namespace Subscriptions
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
            services.AddControllers();
            services.AddMediatR(typeof(Startup));
            services.AddScoped<ISubscriptionAmountCalculator, SubscriptionAmountCalculator>();
            services.AddScoped<DomainEventDispatcher>();
            services.AddDbContext<SubscriptionContext>((serviceProvider, options) =>
            {
                options
                    .AddInterceptors(serviceProvider.GetService<DomainEventDispatcher>())
                    .UseSqlServer(Configuration.GetConnectionString("SubscriptionDatabase"));
            });
            services.AddScoped<IEmailSender, EmailSender>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}