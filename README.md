# NATS Messaging Broker

A C# implementation of a NATS messaging broker, built to explore networking protocols, asynchronous programming, and concurrent message delivery.

The broker manages client connections, parses incoming commands, and distributes messages to clients subscribed to a topic.

## Features

- **Concurrent client connections:** Handles multiple connected clients.
- **Command parsing:** Processes client commands for subscriptions and message publishing.
- **Topic subscriptions:** Allows clients to subscribe to topics and receive published messages.
- **Thread-safe subscription management:** Coordinates access to shared subscription state.
- **Asynchronous message delivery:** Uses C# `async/await` to support message distribution across clients.

## How It Works

1. A client connects to the broker.
2. The client subscribes to a topic.
3. A publisher sends a message to that topic.
4. The broker identifies matching subscribers and delivers the message asynchronously.

## Technology

- C#
- .NET
- Asynchronous programming with `async/await`
- Visual Studio

## Getting Started

### Prerequisites

- The .NET SDK version required by the project.
- Visual Studio or another C# development environment.

### Setup

```bash
git clone <repository-url>
cd <repository-directory>
dotnet restore
dotnet build
dotnet run --project <broker-project-path>
```

Replace the placeholders with the repository URL, directory name, and broker project path. Configure the broker’s listening address and port according to the project’s configuration.

## Project Focus

This project explores the engineering challenges behind a publish-subscribe messaging system:

- Managing concurrent client connections.
- Parsing commands from incoming client traffic.
- Protecting shared subscription state.
- Delivering messages asynchronously to multiple subscribers.

## Scope

The project focuses on the messaging capabilities described above. Full compatibility with the official NATS server and its broader feature set is not established by this README.
