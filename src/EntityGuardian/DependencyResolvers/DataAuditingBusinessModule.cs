using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System.Reflection;

namespace EntityGuardian.DependencyResolvers
{
    public class DataAuditingBusinessModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                builder.RegisterAssemblyTypes(assembly)
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
