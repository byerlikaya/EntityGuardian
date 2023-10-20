using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace EntityGuardian.DependencyResolvers
{
    public class InterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<DataAuditing>(true).ToList();

            var methodAttributes = type.GetMethod(method.Name)!.GetCustomAttributes<DataAuditing>(true);

            classAttributes.AddRange(methodAttributes);

            return classAttributes.ToArray();
        }
    }
}
