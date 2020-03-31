using LoginAPI.Helpers;
using SessionKeyManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LoginAPI.Grpc;

namespace LoginAPI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //Dependency Injection

            var jwtConfig = Configuration.GetSection("Jwt").Get<JwtConfig>();
            var dbConfig = Configuration.GetSection("Database").Get<DBConfig>();
            services.AddSingleton(jwtConfig);
            services.AddSingleton(dbConfig);
            services.AddAuthentication().AddJwtAuthenticationWithKeyAndIssuer(jwtConfig.Key,jwtConfig.Issuer);
            services.AddAuthorization();


            services.AddSingleton<EmailHelper>();

            services.AddSingleton<ISessionKeyManager, JWTSessionKeyManager>();
            services.AddTransient<IDataAccess, DatabaseDataAccess>();
            services.AddTransient<IDatabaseHelper, MySQLHelper>();
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<LoginService>();
            });
        }
    }
}
