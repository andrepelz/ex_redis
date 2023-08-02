using System.Security.Authentication;
using AllogRedis.Api;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetDefaultEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host("jackal-01.rmq.cloudamqp.com", 5671, "zrdaqmiq", h =>
        {
            h.Username("zrdaqmiq");
            h.Password("P_-0fNC98Kiz5kOBrL5Tkz9fod4O6AY6");

            h.UseSsl(s =>
            {
                s.Protocol = SslProtocols.Tls12;
            });
        });

        // busFactoryConfigurator.ReceiveEndpoint("Message", e => { });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
