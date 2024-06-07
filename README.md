This is a simple TCP server that accepts multiple TCP clients connection simultaneously that can run on your local computer for testing.

In the first terminal, run below commands to start the server:
```
cd TcpServer
dotnet run 
```

In the second terminal, run below command to start the client which will automatically connect to the server:
```
cd TcpClient
dotnet run
```

You can also use third and fourth terminal to create more clients.
Client will listen to user input in the console and send the message to server.
When you send "close" message from client to server, the connection will be closed.


Each client is asigned a key which are stored in a dictionary as a key-value pair (key -> client endpoint).

To send message to one or more clients from the server, first enter the key(s) of the client handlers (comma-separated), press ENTER, then enter the message. Enter "all" for broadcast.