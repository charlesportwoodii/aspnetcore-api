using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

namespace App
{
    using App.DataContext;
    using App.Middleware.HMACSignatureAuth;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options => {
                options.RespectBrowserAcceptHeader = true;
            });

            services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();
            
            services.AddDistributedRedisCache(options => {
                options.Configuration = Configuration.GetSection("redis:Configuration").Value;
                options.InstanceName = Configuration.GetSection("redis:InstanceName").Value;
            });

            services.AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            HMACSignatureAuthOptions options = new HMACSignatureAuthOptions {
                cache = cache
            };
            app.UseHMACSignatureAuthentication(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default_route",
                    template: "api/v1/{controller}/{action}/{id?}"
                );
            });
        }
    }
}
