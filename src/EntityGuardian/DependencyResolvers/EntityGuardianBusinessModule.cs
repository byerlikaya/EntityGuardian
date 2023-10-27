using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System.Reflection;

namespace EntityGuardian.DependencyResolvers
{
    public class EntityGuardianBusinessModule : Autofac.Module
    {
        private Assembly Assembly { get; set; }

        public EntityGuardianBusinessModule(Assembly assembly) => Assembly = assembly;

        protected override void Load(ContainerBuilder builder)
        {
            if (Assembly != null)
            {
                builder.RegisterAssemblyTypes(Assembly)
                    .AsImplementedInterfaces()
                    .EnableInterfaceInterceptors(new ProxyGenerationOptions
                    {
                        Selector = new InterceptorSelector()
                    })
                    .SingleInstance()
                    .InstancePerDependency();
            }
        }
    }
}
