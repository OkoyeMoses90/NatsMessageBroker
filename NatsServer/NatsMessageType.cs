namespace NatsServer
{
    //Categorize messages for processing
    public enum NatsMessageType
    {
        Connect,
        Pub,
        Sub,
        Unsub,
        Msg,
        Ping,
        Pong,
        Unknown
    }
}


