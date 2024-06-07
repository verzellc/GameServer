using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpMessageHandler;

var hostName = Dns.GetHostName();
IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
// This is the IP address of the local machine
IPAddress localIpAddress = localhost.AddressList[0];
IPEndPoint ipEndPoint = new(localIpAddress , 13000);
using Socket client = new(
ipEndPoint.AddressFamily, 
SocketType.Stream, 
ProtocolType.Tcp);

try 
{
    await client.ConnectAsync(ipEndPoint);
    Console.WriteLine($"Server connected: {client.RemoteEndPoint}.");

    var receiveTask = Task.Run(() => TcpMessageProcessor.ReceiveData(client, "Client"));
    var sendTask = Task.Run(() => ClientSendData());
    await Task.WhenAll(receiveTask, sendTask);
}
catch (Exception e) 
{
    Console.WriteLine("Error..... " + e.StackTrace);
}
finally
{
    client.Shutdown(SocketShutdown.Both);
    client.Close();
}


async Task ClientSendData()
{
    while (true)
    {
        string message = TcpMessageProcessor.ReadMessage();
        await TcpMessageProcessor.SendData(client, message, "Client");
        
        if (message.Equals("close", StringComparison.InvariantCultureIgnoreCase))
        {
            break;
        }
    }
}