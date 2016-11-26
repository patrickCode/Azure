using System;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Telemetry.Api.Analytics.EventHubs;
using Telemetry.Api.Analytics.Spec;
using Unity.WebApi;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Telemetry.Api
{
    public static class UnityConfig
    {
        public static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            container = RegisterDefaults(container);
            //RegisterConfigs(container);
            //RegisterComponents(container);
            return container;
        });
        public static void RegisterComponents()
        {
			var unityContainer = new UnityContainer();
            
            // register all your components with the _container here
            // it is NOT necessary to register your controllers
            
            // e.g. _container.RegisterType<ITestService, TestService>();
            unityContainer.RegisterType<IEventSender, EventHubSender>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(unityContainer);
        }

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        private static void RegisterConfigs(IUnityContainer unityContainer)
        {
            if (ConfigurationManager.GetSection("unity") != null)
            {
                unityContainer.LoadConfiguration();
            }
        }

        private static UnityContainer RegisterDefaults(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IEventSender, EventHubSender>();
            return (UnityContainer)unityContainer;
        }
    }
}