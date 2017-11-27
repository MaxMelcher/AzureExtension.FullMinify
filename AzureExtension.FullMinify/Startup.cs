using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureExtension.FullMinify.Helper;
using AzureExtension.FullMinify.Log;
using AzureExtension.FullMinify.Minify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureExtension.FullMinify
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMinifier(Configuration);
        }
    }

    public static class RegisterMinification
    {
        public static void UseMinifier(this IApplicationBuilder app, IConfiguration configuration)
        {
            try
            {
                string path = @"D:\home\site\wwwroot\appdata\Azure.FullMinify";

                ConfigReader configReader = new ConfigReader();
                var config = configReader.Read(path);
   
                Minifier minifier = new Minifier(config);
                Task.Run(() => minifier.FullMinify()).ContinueWith(minifier.Watch);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Exception: {ex}");
            }
        }
    }
}
