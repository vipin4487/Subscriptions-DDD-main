using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureServiceBus.EventBus;
using AzureServiceBus.EventBusServiceBus;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Subscriptions.Before.Data;
using Subscriptions.Before.Domain;
using Subscriptions.Before.Infrastructure;
using Subscriptions.Before.IntegrationEvents;
using Subscriptions.Before.Services;

namespace Subscriptions.Before
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //public IServiceCollection _services { get; set; }
       // public ILifetimeScope AutofacContainer { get; private set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMediatR(typeof(Startup));
            services.AddScoped<DomainEventDispatcher>();
            services.AddDbContext<SubscriptionContext>((serviceProvider, options) =>
               options
                   .AddInterceptors(serviceProvider.GetService<DomainEventDispatcher>())
                   .UseSqlServer(Configuration.GetConnectionString("SubscriptionDatabase")));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISubscriptionAmountCalculator, SubscriptionAmountCalculator>();

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                var serviceBusConnectionString = "Endpoint=sb://eshoptestdemoapp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=tdhI8AWEBMMoKuMjNJGHCiHvFT09CJ/vCNFQe180f6g=";
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
                serviceBusConnection.EntityPath = "Basket";
                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });

            AddEventBus(services);
            //_services = services;
            //var container = new ContainerBuilder();
            //container.Populate(services);

            //return new AutofacServiceProvider(container.Build());
            //var iLifetimeScope = services.AddSingleton<ILifetimeScope>();//  services.GetRequiredService<ILifetimeScope>();
            //var iLifetimeScope = services.Resolve<ILifetimeScope>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<ProductPriceChangedIntegrationEventHandler>();
            //builder.RegisterInstance(new LoggerFactory())
            //   .As<ILoggerFactory>();

            //builder.RegisterGeneric(typeof(Logger<>))
            //       .As(typeof(ILogger<>))
            //       .SingleInstance();

            //builder.Register<IServiceBusPersisterConnection>(sp =>
            //{
            //    var logger = sp.Resolve<ILogger<DefaultServiceBusPersisterConnection>>();
            //    var serviceBusConnectionString = "Endpoint=sb://eshoptestdemoapp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=tdhI8AWEBMMoKuMjNJGHCiHvFT09CJ/vCNFQe180f6g=";
            //    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
            //    serviceBusConnection.EntityPath = "Basket";
            //    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            //});

            //builder.Register<IEventBus>(sp =>
            //{
            //    var serviceBusPersisterConnection = sp.Resolve<IServiceBusPersisterConnection>();
            //    var iLifetimeScope = sp.Resolve<ILifetimeScope>();
            //    var logger = sp.Resolve<ILogger<EventBusServiceBus>>();
            //    var eventBusSubcriptionsManager = sp.Resolve<IEventBusSubscriptionsManager>();

            //    var subscriptionClientName = "BasketClient1";
            //    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
            //        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            //});
            //builder.RegisterType<InMemoryEventBusSubscriptionsManager>().As<IEventBusSubscriptionsManager>().SingleInstance();
        }

        private void AddEventBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                //"SubscriptionClientName": "Basket",
                var subscriptionClientName = "BasketClient1";// Configuration["SubscriptionClientName"];
                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
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
            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            //this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            var eventBus =  app.ApplicationServices.GetRequiredService<IEventBus>();//app.ApplicationServices.GetAutofacRoot().Resolve<IEventBus>();
            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        }
    }
}