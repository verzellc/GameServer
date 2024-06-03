using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpLib
{
    public static class TcpHelper
    {
        public static async Task ReceiveData(Socket socket, string identifier)
        {
            try
            {
                while (true)
                {
                    var buffer = new byte[1_024];
                    var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, received);

                    Console.WriteLine($"{identifier} received message: \"{receivedMessage}\"");

                    if (string.IsNullOrEmpty(receivedMessage))
                    {
                        Console.WriteLine($"Disconnect from {identifier}: {socket.RemoteEndPoint}.");
                        socket.Shutdown(SocketShutdown.Both);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                socket.Close();
            }
        }

        public static async Task SendData(Socket socket, string identifier)
        {
            try
            {
                while (true)
                {
                    string? message = Console.ReadLine();
                    if (string.IsNullOrEmpty(message))
                    {
                        continue;
                    }

                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    _ = await socket.SendAsync(messageBytes, SocketFlags.None);
                    Console.WriteLine($"{identifier} sent message: \"{message}\"");
                    
                    if (message.Equals("close", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}

