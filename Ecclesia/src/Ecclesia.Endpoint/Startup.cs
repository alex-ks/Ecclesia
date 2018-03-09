using Ecclesia.DataAccessLayer;
using Ecclesia.ExecutorClient;
using Ecclesia.Identity.Auth;
using Ecclesia.Identity.Models;
using Ecclesia.MessageQueue;
using Ecclesia.MessageQueue.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecclesia.Endpoint
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

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 1;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<EcclesiaContext>()
                .AddDefaultTokenProviders();

            var connectionString = Configuration.GetConnectionString("psql");
            services.AddTransient<EcclesiaContext, PsqlContext>(_ => 
                new PsqlContext(connectionString));

            var queueEntry = Configuration.GetSection("RabbitMQ");
            var queueParams = new RmqMessageQueueParams
            {
                HostName = queueEntry["hostname"],
                UserName = queueEntry["username"],
                Password = queueEntry["password"]
            };

            services.AddSingleton(queueParams);
            services.AddTransient(typeof(IMessageQueue<>), typeof(RmqMessageQueue<>));

            var executorUrl = Configuration.GetSection("Executor")["url"];
            services.AddTransient<IExecutor, ExecutorRestClient>(_ => new ExecutorRestClient(executorUrl));

            services.AddTransient<SessionManager>();

            services.AddSingleton<Poller>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = UsernameAuthOptions.DefaultScheme;
                    options.DefaultChallengeScheme = UsernameAuthOptions.DefaultScheme;
                })
                .AddUsernameScheme();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
