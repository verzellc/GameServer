using System.Net;
using System.Net.Sockets;
using System.Text;

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

try
{   
    while (true)
    {
        Socket handler = await listener.AcceptAsync();
        ThreadPool.QueueUserWorkItem(state => HandleConnection(handler));
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

async void HandleConnection(Socket handler)
{
    try 
    {
        Console.WriteLine($"Client connected: {handler.RemoteEndPoint}.");
        while (true)
        {
            var buffer = new byte[1_024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, received);

            Console.WriteLine(
                $"Socket server received message: \"{receivedMessage}\"");
            if (String.IsNullOrEmpty(receivedMessage))
            {
                Console.WriteLine($"Disconnect from client: {handler.RemoteEndPoint}.");
                handler.Shutdown(SocketShutdown.Both);
                break;
            }

            var ackMessage = "<|ACK|>";
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await handler.SendAsync(echoBytes, 0);
            Console.WriteLine(
                $"Socket server sent acknowledgment: \"{ackMessage}\"");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}