using Autofac;
using MediatR;
using System.Reflection;

namespace Sample.Api
{
    public class AutofacBusinessModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .AsImplementedInterfaces()
                .AsClosedTypesOf(typeof(IRequestHandler<,>));
        }
    }
}
