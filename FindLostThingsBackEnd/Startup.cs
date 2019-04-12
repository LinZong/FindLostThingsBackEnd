using FindLostThingsBackEnd.Persistence.DAO.Context;
using FindLostThingsBackEnd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FindLostThingsBackEnd.Middleware;
using ChargeScheduler.Services.User.UIDWorker;
using FindLostThingsBackEnd.Service.Tencent;

namespace FindLostThingsBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> log)
        {
            Configuration = configuration;
            logger = log;
        }

        private ILogger<Startup> logger { get; set; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            bool ReadProd = bool.Parse(Configuration["IsReadProdDb"]);
            var DbConnString = ReadProd ? Configuration["ConnectionStrings:MySQLConnectionStringProd"] : Configuration["ConnectionStrings:MySQLConnectionString"];
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(ServiceConfigurator.ConfigureKeepCaseOfMetadataInJsonResult);

            logger.LogInformation($"Used db connection string : {DbConnString}");
            services.AddDbContext<LostContext>(opt => opt.UseMySQL(DbConnString));

            services.Configure<SnowflakeConfigurationModel>(Configuration.GetSection("SnowflakeConfiguration"));
            services.AddTencentCos(x =>
            {
                x.SecretId = Configuration["TencentCos:SecretId"];
                x.SecretKey = Configuration["TencentCos:SecretKey"];
                x.AllowPrefix = "*";
                x.BucketName = "nemesiss";
                x.AppID = "1255798866";
                x.DurationSeconds = 1800;
                x.Region = "ap-guangzhou";
            });
            services.AddSingleton<IdWorker>();
            services.AddAllServices<IFindLostThingsService>();
            services.AddAllServices<IFindLostThingsDbOperator>();
            services.AddAuthorization();
            services.AddAuthentication("AuthorizeACTK")
                .AddScheme<AuthorizeACTKSchemeOptions, AuthorizeACTKHandler>("AuthorizeACTK", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

        }
    }
}
