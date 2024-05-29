using System.Net;
using System.Net.Sockets;
using System.Text;


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
    while (true)
    {
        String? message = Console.ReadLine();
        if (String.IsNullOrEmpty(message))
        {
            continue;
        }

        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Socket client sent message: \"{message}\"");

        // Receive ack.
        var buffer = new byte[1_024];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);
        if (response == "<|ACK|>")
        {
            Console.WriteLine(
                $"Socket client received acknowledgment: \"{response}\"");
        }

        if (message.Equals("close", StringComparison.InvariantCultureIgnoreCase))
        {
            break;
        }
    }
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