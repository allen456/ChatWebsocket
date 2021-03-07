using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

using ChatApp.Middleware;
using ChatApp.Data;

namespace ChatApp
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
            services.AddDbContext<ChatContext>(options => options.UseInMemoryDatabase(databaseName: "Chat"));
            services.AddControllersWithViews();
            services.AddWebSocketServerConnectionManager();
            services.AddHttpContextAccessor();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ChatContext>();
                ChatInitializer.Initialize(context);
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Chat/Error");
                app.UseForwardedHeaders();
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseWebSockets();
            app.UseWebSocketServer();

            app.UseRouting();
            app.UseStatusCodePagesWithRedirects("/Chat/Error?type={0}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=Index}/{id?}");
            });
        }
    }
}
