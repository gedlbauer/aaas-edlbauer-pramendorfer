using AaaS.Api.Auth;
using AaaS.Api.JsonConverters;
using AaaS.Api.Settings;
using AaaS.Common;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Managers;
using AaaS.Core.Repositories;
using AaaS.Dal.Ado;
using AaaS.Dal.Ado.Telemetry;
using AaaS.Dal.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api
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

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.Filters.Add(new ProducesAttribute("application/json"));
            })
                .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new TimeSpanToMillisecondsConverter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AaaS.Api", Version = "v1" });
                c.AddSecurityDefinition(ApiKeyConstants.HeaderName, new OpenApiSecurityScheme
                {
                    Description = $"Api key needed to access the endpoints. {ApiKeyConstants.HeaderName}: My_API_Key",
                    In = ParameterLocation.Header,
                    Name = ApiKeyConstants.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name =  ApiKeyConstants.HeaderName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id =  ApiKeyConstants.HeaderName
                            },
                        },
                        Array.Empty<string>()
                    }
                });
                c.MapType(typeof(TimeSpan), () => new OpenApiSchema
                {
                    Type = "number"
                });
            });
            services.AddSingleton(x => DefaultConnectionFactory.FromConfiguration(Configuration, "AaaSDbConnection"));
            services.AddTransient<IActionDao<BaseAction>, MSSQLActionDao<BaseAction>>();
            services.AddTransient<IDetectorDao<BaseDetector, BaseAction>, MSSQLDetectorDao<BaseDetector, BaseAction>>();

            services.AddTransient<ILogDao, MSSQLLogDao>();
            services.AddTransient<IMetricDao, MSSQLMetricDao>();
            services.AddTransient<ITimeMeasurementDao, MSSQLTimeMeasurementDao>();
            services.AddTransient<IClientDao, MSSQLClientDao>();

            services.AddSingleton<LogRepository>();
            services.AddSingleton<MetricRepository>();
            services.AddSingleton<TimeMeasurementRepository>();

            services.AddSingleton<ActionManager>();
            services.AddSingleton<DetectorManager>();
            services.AddAutoMapper(typeof(Startup));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                o.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            }).AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, o => { });
            services.AddAuthorization();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // DetectorManager wird hier injected, damit er direkt beim Applikationsstart initialisiert wird, und nicht erst beim ersten Controller-Aufruf
        //      |-> dadurch werden die Detectors gleich bei Appstart gestartet
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DetectorManager _)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AaaS.Api v1"));
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
