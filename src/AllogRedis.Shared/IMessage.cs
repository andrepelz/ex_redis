namespace AllogRedis.Shared;

public interface IMessage
{
    DateTime Date { get; }
    string Message { get; }
    string Author { get; }
}
