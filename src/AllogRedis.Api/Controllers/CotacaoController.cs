using System.Text.Json;
using AllogRedis.Api.DbContexts;
using AllogRedis.Api.Entities;
using AllogRedis.Shared;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AllogRedis.Api;

[ApiController]
[Route("api/cotacao")]
public class CotacaoController : ControllerBase
{
    private readonly CotacaoContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly IPublishEndpoint _publishEndpoint;

    public CotacaoController(CotacaoContext context, IConnectionMultiplexer redis, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _redis = redis;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult <IEnumerable <Cotacao>>> GetCotacaoByDate(int year, int month, int day)
    {
        var cache = _redis.GetDatabase();

        var date = new DateOnly(year, month, day);

        // var resultFromCache = await cache.StringGetAsync($"cotacoes_{date}");
        var resultFromCache = await cache.StringGetAsync($"{date}");

        if(!resultFromCache.IsNull)
        {
            var cotacoes = JsonSerializer.Deserialize<IEnumerable<Cotacao>>(resultFromCache.ToString())!.ToList();

            await VerificarCotacaoDolar(cotacoes);

            return Ok(resultFromCache.ToString());
        }

        var result = _context.Cotacao.Where(c => c.Data == date).ToList();
        await VerificarCotacaoDolar(result);

        var cacheResultSet = JsonSerializer.Serialize<IEnumerable<Cotacao>>(result);
        await cache.StringSetAsync($"cotacoes_{date}", cacheResultSet);
        await cache.KeyExpireAsync($"cotacoes_{date}", TimeSpan.FromSeconds(120));


        return result.Any() ? Ok(result) : NotFound();
    }

    [HttpGet("{id}")]
    public ActionResult<Cotacao> GetCotacaoById(int id)
    {
        var result = _context.Cotacao.FirstOrDefault(c => c.CotacaoId == id);

        return result != null ? Ok(result) : NotFound();
    }

    private async Task VerificarCotacaoDolar(List<Cotacao> cotacoes)
    {
        var cotacaoDolar = cotacoes.FirstOrDefault(c => c.Sigla == "USD");

        if(cotacaoDolar != null && cotacaoDolar.Valor < 3)
        {
            await _publishEndpoint.Publish<IMessage>(new {
                Date = DateTime.Now,
                Message = "Cotacao do dolar esta abaixo de R$3,00!",
                Author = "RedisExampleAPI"
            });
        }
    }
}