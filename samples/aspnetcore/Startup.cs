using System;
using AgentFramework.AspNetCore;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Wallets;
using Jdenticon;
using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAgent.Messages;
using WebAgent.Protocols.BasicMessage;
using WebAgent.Utils;

namespace WebAgent
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

            services.AddLogging();

            var ownerName = Environment.GetEnvironmentVariable("AGENT_NAME") ?? NameGenerator.GetRandomName();
            // TODO:  Allow the configuration of a AGENT_URL, and fall back to this computed version.
            // TODO:  Fix this hack based on Jdenticon URL encoding
            var hash = "Ci8A" + Identicon.FromValue(ownerName, 100).Hash.ToBase64String().Replace('-', '~') + "--";

            // Register agent framework dependency services and handlers
            services.AddAgentFramework(builder =>
            {
                builder.AddBasicAgent<SimpleWebAgent>(c =>
                {
                    c.OwnerName = Environment.GetEnvironmentVariable("AGENT_NAME") ?? NameGenerator.GetRandomName();
                    c.EndpointUri = new Uri(Environment.GetEnvironmentVariable("ENDPOINT_HOST") ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
                    c.WalletConfiguration = new WalletConfiguration { Id = "WebAgentWallet" };
                    c.WalletCredentials = new WalletCredentials { Key = "MyWalletKey" };
                    c.OwnerImageUrl = c.EndpointUri + "identicons/" + hash;
                });
            });

            // Register custom handlers with DI pipeline
            services.AddSingleton<BasicMessageHandler>();
            services.AddSingleton<TrustPingMessageHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Register agent middleware
            app.UseAgentFramework();

            // fun identicons
            app.UseJdenticon();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
