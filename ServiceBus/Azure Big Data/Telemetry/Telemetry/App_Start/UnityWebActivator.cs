using System.Web.Http;
using Telemetry.Api;
using Microsoft.Practices.Unity;
using Unity.WebApi;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebActivator), "Start")]
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebActivator), "Shutdown")]

namespace Telemetry.Api
{
    public static class UnityWebActivator
    {
        public static void Start()
        {
            //var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            //var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());
            UnityConfig.RegisterComponents();
        }

        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}