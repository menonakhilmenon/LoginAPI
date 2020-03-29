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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //Dependency Injection

            var jwtConfig = Configuration.GetSection("Jwt").Get<JwtConfig>();

            services.AddSingleton(jwtConfig);
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
