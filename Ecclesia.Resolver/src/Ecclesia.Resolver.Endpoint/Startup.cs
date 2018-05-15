using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Ecclesia.Resolver.Endpoint
{
    public class Startup
    {
        private const string ApiVersion = "v0.1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c => 
                c.SwaggerDoc(ApiVersion, new Info { Title = "Ecclesia.Resolver API", Version = ApiVersion }));

            var connectionString = Configuration.GetConnectionString("psql");
            services.AddTransient<ResolverContext, NpgsqlResolverContext>(_ => 
                new NpgsqlResolverContext(connectionString));
            
            services.AddScoped<AtomStorage>();
            services.AddScoped<Resolver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => 
                c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", $"Ecclesia.Resolver API {ApiVersion}"));

            app.UseMvc();
        }
    }
}
