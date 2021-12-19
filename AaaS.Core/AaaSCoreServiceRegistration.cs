using AaaS.Common;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.HostedServices;
using AaaS.Core.Managers;
using AaaS.Core.Repositories;
using AaaS.Dal.Ado;
using AaaS.Dal.Ado.Telemetry;
using AaaS.Dal.Interface;
using AaaS.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core
{
    public static class AaaSCoreServiceRegistration
    {
        public static IServiceCollection ApplicationServiceRegistration(this IServiceCollection services)
        {
            services.AddTransient<IActionDao<BaseAction>, MSSQLActionDao<BaseAction>>();
            services.AddTransient<IDetectorDao<BaseDetector, BaseAction>, MSSQLDetectorDao<BaseDetector, BaseAction>>();

            services.AddTransient<ILogDao, MSSQLLogDao>();
            services.AddTransient<IMetricDao, MSSQLMetricDao>();
            services.AddTransient<ITimeMeasurementDao, MSSQLTimeMeasurementDao>();
            services.AddTransient<IClientDao, MSSQLClientDao>();

            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IMetricRepository, MetricRepository>();
            services.AddTransient<ITelemetryRepository<TimeMeasurement>, TimeMeasurementRepository>();

            services.AddSingleton<IActionManager, ActionManager>();
            services.AddSingleton<IDetectorManager, DetectorManager>();
            services.AddHostedService(sp => sp.GetService<HeartbeatService>());
            return services;
        }
    }
}
