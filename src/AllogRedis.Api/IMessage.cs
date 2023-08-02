namespace AllogRedis.Api;

public interface IMessage
{
    DateTime Date { get; }
    string Message { get; }
    string Author { get; }
}
