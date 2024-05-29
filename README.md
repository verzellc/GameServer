On first terminal:
cd /TcpServer
dotnet run 

On second terminal:
cd /TcpClient
dotnet run

You can also use third and fourth terminal to create more clients.
Client will listen to user input in the console and send the message to server.
When you send "close" message from client to server, the connection will be closed.
