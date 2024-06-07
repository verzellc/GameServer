using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpMessageHandler;

var hostName = Dns.GetHostName();
IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
// This is the IP address of the local machine
IPAddress localIpAddress = localhost.AddressList[0];
int port = 13000;
IPEndPoint ipEndPoint = new(localIpAddress , port);
using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);
listener.Bind(ipEndPoint);
listener.Listen(100);
Console.WriteLine($"Listening on port {port}.");

// Dictionary to store client handlers
ConcurrentDictionary<string, Socket> clientHandlers = new();

string[] clientKeys = [];

try
{   
    var sendTask = Task.Run(() => ServerSendData());

    while (true)
    {
        Socket handler = await listener.AcceptAsync();

        // Generate a unique key for the client
        string clientKey = clientHandlers.Count.ToString();
        clientHandlers[clientKey] = handler;
      
        _ = Task.Run(() => HandleConnection(handler, clientKey));
    }
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
finally
{
    listener.Shutdown(SocketShutdown.Both);
}

async Task HandleConnection(Socket handler, string clientKey)
{
    Console.WriteLine($"Client connected: {handler.RemoteEndPoint} with key {clientKey}.");
    PrintClientHandlers();
   
    var receiveTask = Task.Run(() => TcpMessageProcessor.ReceiveData(handler, "Server"));
    await receiveTask;
}

async Task ServerSendData()
{
    while (true)
    {
        
        string[] clientKeys = ReadClientKeys();
        string message = TcpMessageProcessor.ReadMessage();

    
        if (clientKeys.Contains("all", StringComparer.InvariantCultureIgnoreCase)) // Broadcast
        {
            var tasks = clientHandlers.Values.Select(clientHandler => TcpMessageProcessor.SendData(clientHandler, message, "Server"));
            await Task.WhenAll(tasks);
        }
        else
        {
            var tasks = new List<Task>();
            bool invalidKeyFound = false;
            foreach (var clientKey in clientKeys)
            {
                if (clientHandlers.TryGetValue(clientKey, out Socket? handler))
                {
                    tasks.Add(TcpMessageProcessor.SendData(handler, message, "Server"));
                }
                else
                {
                    Console.WriteLine($"Invalid client key: {clientKey}");
                    invalidKeyFound = true;
                    break;
                    
                }
            }
            if (invalidKeyFound)
            {
                clientKeys = ReadClientKeys();

            }
            await Task.WhenAll(tasks);
        }
    }
}

void PrintClientHandlers()
{
    Console.WriteLine("Current connections:");
    foreach (var entry in clientHandlers)
    {
        Console.WriteLine($"Key: {entry.Key}, Endpoint: {entry.Value.RemoteEndPoint}");
    }
}


string[] ReadClientKeys()
{
    Console.WriteLine("Enter client key(s) (comma-separated or 'all' for broadcast): ");
    string? clientKeysInput = Console.ReadLine();

    var clientKeys = clientKeysInput?.Split(',').Select(k => k.Trim()).ToArray();
    if (clientKeys == null || clientKeys.Length == 0)
    {
        return [];
    }
    PrintClientHandlers();

    return clientKeys;
}