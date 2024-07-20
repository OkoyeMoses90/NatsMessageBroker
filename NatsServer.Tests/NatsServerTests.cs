using System;
using System.Net;
using Xunit;
using NatsServer;

namespace NatsServer.Tests
{
    public class NatsServerTests
    {
        private readonly NatsServer _natsServer;

        public NatsServerTests()
        {
            _natsServer = new NatsServer(IPAddress.Parse("127.0.0.1"), 4222);  // Null and 0 are placeholders since we are only testing the parser.
        }

        [Theory]
        [InlineData("CONNECT", NatsMessageType.Connect)]
        [InlineData("PUB foo bar", NatsMessageType.Pub)]
        [InlineData("SUB foo", NatsMessageType.Sub)]
        [InlineData("UNSUB foo", NatsMessageType.Unsub)]
        [InlineData("MSG foo bar", NatsMessageType.Msg)]
        [InlineData("PING", NatsMessageType.Ping)]
        [InlineData("PONG", NatsMessageType.Pong)]
        [InlineData("UNKNOWNCOMMAND", NatsMessageType.Unknown)]
        public void ParseMessage_ShouldReturnCorrectMessageType(string message, NatsMessageType expectedType)
        {
            // Act
            var result = _natsServer.ParseMessage(message);

            // Assert
            Assert.Equal(expectedType, result.MessageType);
        }

        [Fact]
        public void ParseMessage_ShouldSplitPartsCorrectly()
        {
            // Arrange
            var message = "PUB foo bar";
            var expectedParts = new[] { "PUB", "foo", "bar" };

            // Act
            var result = _natsServer.ParseMessage(message);

            // Assert
            Assert.Equal(expectedParts, result.Parts);
        }

        [Fact]
        public void ParseMessage_ShouldTrimWhitespace()
        {
            // Arrange
            var message = "  PUB foo bar  ";
            var expectedParts = new[] { "PUB", "foo", "bar" };

            // Act
            var result = _natsServer.ParseMessage(message);

            // Assert
            Assert.Equal(expectedParts, result.Parts);
        }
    }
}
