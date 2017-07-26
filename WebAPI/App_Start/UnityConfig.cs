using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using System.Web.Http;
using BusinessServices;
using DataModel.UnitOfWork;
using Resolver;

namespace WebAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = BuildUnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            System.Web.Mvc.DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();        
            //container.RegisterType<IProductServices, ProductServices>().RegisterType<UnitOfWork>(new HierarchicalLifetimeManager());
            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {

            //Component initialization via MEF
            ComponentLoader.LoadContainer(container, ".\\bin", "WebApi.dll");
            ComponentLoader.LoadContainer(container, ".\\bin", "BusinessServices.dll");
            //ComponentLoader.LoadContainer(container, ".\\bin", "DataModel.dll");

        }

    }
}