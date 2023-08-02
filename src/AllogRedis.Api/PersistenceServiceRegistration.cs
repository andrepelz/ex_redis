using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AllogRedis.Api;

public static class PersistenceServiceRegistration
{
    private const string HOST_STRING = "localhost";
    private const string DATABASE_NAME = "aula_32";
    private const string POSTGRES_USERNAME = "postgres";
    private const string POSTGRES_PASSWORD = "123456";

    private const string DATABASE_INIT_STRING = $"Host={HOST_STRING};Database={DATABASE_NAME};Username={POSTGRES_USERNAME};Password={POSTGRES_PASSWORD}";
    // private const string DATABASE_INIT_STRING = "Host=localhost;Database=Univali;Username=postgres;Password=123456";

    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContexts();
        services.AddRedis();

        return services;
    }

    private static void AddDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<CotacaoContext>(options =>
            options.UseNpgsql(DATABASE_INIT_STRING)
        );
    }

    private static void AddRedis(this IServiceCollection services)
    {
        var multiplexer = ConnectionMultiplexer.Connect("redis-15387.c308.sa-east-1-1.ec2.cloud.redislabs.com:15387,password=jgx9GyBzZ8PQV5CbKEkF0SWaJMLGWzHL");
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
    }
}
