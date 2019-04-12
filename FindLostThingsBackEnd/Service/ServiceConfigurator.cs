using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindLostThingsBackEnd.Services
{
    public interface IFindLostThingsService { }
    public interface IFindLostThingsDbOperator { }
   
    public static class ServiceConfigurator
    {
        public static IServiceCollection AddAllServices<TServ>(this IServiceCollection services)
        {
            AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assm => assm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(TServ))))
                        .Where(x => x.IsClass)
                        .ForEachService(x => services.AddScoped(x));
            return services;
        }

        public static IEnumerable<T> ForEachService<T>(this IEnumerable<T> serv, Action<T> action) where T:Type
        {
            foreach (var item in serv)
            {
                action(item);
            }
            return serv;
        }

        public static void ConfigureKeepCaseOfMetadataInJsonResult(MvcJsonOptions options)
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        }
    }
}
