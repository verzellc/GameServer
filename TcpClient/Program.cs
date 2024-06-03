using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpLib;

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

    var receiveTask = Task.Run(() => TcpHelper.ReceiveData(client, "Client"));
    var sendTask = Task.Run(() => TcpHelper.SendData(client, "Client"));
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