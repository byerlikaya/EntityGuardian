using Microsoft.Extensions.DependencyInjection;
using System;

namespace EntityGuardian.Utilities
{
    internal static class ServiceTool
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void Create(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
