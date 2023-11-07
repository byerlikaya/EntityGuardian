namespace EntityGuardian.DependencyResolvers;

public class InterceptorSelector : IInterceptorSelector
{
    public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
    {
        var classAttributes = type.GetCustomAttributes<EntityGuardian>(true).ToList();

        var methodAttributes = type.GetMethod(method.Name)!.GetCustomAttributes<EntityGuardian>(true);

        classAttributes.AddRange(methodAttributes);

        return classAttributes.ToArray();
    }
}