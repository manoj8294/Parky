using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkyAPI.Data;
using ParkyAPI.ParkyMappers;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NLog;
using ParkyAPI.Infrastructure;
using ParkyAPI.Infrastructure.Interface;
using ParkyAPI.Middlewares;

namespace ParkyAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILog, LogNLog>();
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                 {
                     x.RequireHttpsMetadata = false;
                     x.SaveToken = true;
                     x.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(key),
                         ValidateIssuer = false,
                         ValidateAudience = false
                     };
                 });
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("ParkyOpenApiSpecNP",
            //        new Microsoft.OpenApi.Models.OpenApiInfo()
            //        {
            //            Title = "Parky Api",
            //            Version = "1",
            //            Description = "Test Parky API",
            //            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            //            {
            //                Email = "manoj8294@gmail.com",
            //                Name = "Manoj Singh Tomar",
            //                Url = new Uri("https://www.linkedin.com/in/manoj-singh-tomar-100700105/")
            //            },
            //            License = new Microsoft.OpenApi.Models.OpenApiLicense
            //            {
            //                Name = "MIT License",
            //                Url = new Uri("https://opensource.org/licenses/MIT")
            //            }
            //        });
            //options.SwaggerDoc("ParkyOpenApiSpecTrails",
            //    new Microsoft.OpenApi.Models.OpenApiInfo()
            //    {
            //        Title = "Parky Api Trails",
            //        Version = "1",
            //        Description = "Test Parky API Trails",
            //        Contact = new Microsoft.OpenApi.Models.OpenApiContact
            //        {
            //            Email = "manoj8294@gmail.com",
            //            Name = "Manoj Singh Tomar",
            //            Url = new Uri("https://www.linkedin.com/in/manoj-singh-tomar-100700105/")
            //        },
            //        License = new Microsoft.OpenApi.Models.OpenApiLicense
            //        {
            //            Name = "MIT License",
            //            Url = new Uri("https://opensource.org/licenses/MIT")
            //        }
            //    });
            //    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlCommnetsFilePath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            //    options.IncludeXmlComments(xmlCommnetsFilePath);
            //});
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var desc in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"./swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
                options.RoutePrefix = "";    // To make swagger UI page as default
            });

            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecNP/swagger.json", "Parky API");
            //    //options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecTrails/swagger.json", "Parky API Trails");
            //    options.RoutePrefix = "";    // To make swagger UI page as default
            //});
            app.UseRouting();
            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
