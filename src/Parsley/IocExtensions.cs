using Microsoft.Extensions.DependencyInjection;

namespace parsley
{
    public static class IocExtensions
    {
        public static IServiceCollection UseParsley(this IServiceCollection services, char delimeter = ',')
        {
            services.AddTransient(typeof(IParser), c => new Parser(delimeter));
            return services;
        }
    }
}