namespace NatsServer
{
    //Class representing a parsed message
    //MessageType: Eg pub,sub,unsub etc
    //Parts: The components of the message, split into an array
    public class NatsMessage
    {
        public NatsMessageType MessageType {get; set;}
        public string[] Parts {get; set;}

        public NatsMessage(NatsMessageType messageType, string[] parts)
        {
            MessageType = messageType;
            Parts = parts;
        }
    }
}


