using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheLeague.Providers;
using TheLeague.Providers.Interfaces;

namespace TheLeague {
    public class Startup {
        private static Timer _timer;
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _timer = new Timer(this.CheckForUpdates, null, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, Timeout.Infinite);
        }

        private void CheckForUpdates(object state) {
            FplDataProvider.GetAllData();
            _timer.Change((int)TimeSpan.FromMinutes(60).TotalMilliseconds, Timeout.Infinite);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services.AddMvc();
            services.AddTransient<IMongoFplDataRequestProvider, MongoFplDataRequestProvider>();
            services.AddTransient<IMongoLineupProvider, MongoLineupProvider>();
            services.AddTransient<IMongoManagerProvider, MongoManagerProvider>();
            services.AddTransient<IMongoResultProvider, MongoResultProvider>();
            services.AddTransient<IMongoPlayerProvider, MongoPlayerProvider>();
            services.AddTransient<IMongoTeamProvider, MongoTeamProvider>();
            services.AddTransient<IMongoTransferProvider, MongoTransferProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
