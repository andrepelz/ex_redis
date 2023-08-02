namespace AllogRedis.Api;

public class Cotacao
{
    public int CotacaoId { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string NomeMoeda { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public decimal Valor { get; set; }
}