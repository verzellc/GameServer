using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpLib;

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
        _ = Task.Run(() => HandleConnection(handler));
        // ThreadPool.QueueUserWorkItem(state => HandleConnection(handler));
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

async Task HandleConnection(Socket handler)
{
    Console.WriteLine($"Client connected: {handler.RemoteEndPoint}.");
    var receiveTask = Task.Run(() => TcpHelper.ReceiveData(handler, "Server"));
    var sendTask = Task.Run(() => TcpHelper.SendData(handler, "Server"));
    await Task.WhenAll(receiveTask, sendTask);
}