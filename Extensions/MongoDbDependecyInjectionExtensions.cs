using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Yoda.MongoDb.Extensions
{

    public static class MongoDbDependecyInjectionExtensions
    {
        public static IServiceCollection AddMongoDbFactory<TFactory, TOptions>(this IServiceCollection serviceCollection) where TFactory : MongoDbAbstractFactory where TOptions : MongoDbConnectionOptions
        {
            serviceCollection
             .AddMemoryCache()
             .AddLogging();

            serviceCollection.AddSingleton<TOptions, TOptions>();
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMongoDbFactory), typeof(TFactory), ServiceLifetime.Singleton));
            serviceCollection.AddSingleton<TFactory>(p => (TFactory)p.GetRequiredService<IMongoDbFactory>());

            return serviceCollection;
        }

    }
}