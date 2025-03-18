using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace ChatApp
{
    internal class Program
    {
        private static TcpListener server;
        private static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();
        private static object objectLock = new object();

        static void Main(string[] args)
        {
            int port = 5000; 
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"Chat server started on port {port}...");

            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        private static void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            lock (objectLock)
            {
                foreach (var client in clients.Keys)
                {
                    try
                    {
                        client.GetStream().Write(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        
                    }
                }
            }
        }

        private static void BroadcastUserList()
        {
            string userList = "/users " + string.Join(",", clients.Values);
            BroadcastMessage(userList);
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            string username = "";
            string clientIP = "Unknown IP";
            string connectTime = DateTime.Now.ToString("HH:mm yyyy/MM/dd");

            try
            {
                
                if (!client.Connected)
                {
                    Console.WriteLine("Client disconnected before sending username.");
                    return;
                }

               
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected before sending username.");
                    return;
                }

                username = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                if (client.Client?.RemoteEndPoint != null)
                {
                    clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                }
                lock (objectLock)
                {
                    clients.Add(client, $"{username} ({clientIP})");
                    Console.WriteLine($"[{connectTime}] {username} connected from {clientIP}.");
                }

                BroadcastUserList();
                BroadcastMessage($"{username} joined the chat from {clientIP}");

                
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    if (message.StartsWith("/typing"))
                    {
                        BroadcastMessage($"/typing {username}");
                    }
                    else
                    {
                        BroadcastMessage($"{username}: {message}");
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"Client {username} ({clientIP}) lost connection.");
            }
            catch (SocketException)
            {
                Console.WriteLine($"Network error with client {username} ({clientIP}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            finally
            {
                lock (objectLock)
                {
                    if (clients.ContainsKey(client))
                    {
                        string disconnectedUser = clients[client];
                        string disconnectTime = DateTime.Now.ToString("HH:mm yyyy/MM/dd");
                        clients.Remove(client);
                        Console.WriteLine($"[{disconnectTime}] {disconnectedUser} disconnected.");
                       // Console.WriteLine($"{disconnectedUser} disconnected.");
                        BroadcastUserList();
                        BroadcastMessage($"[{disconnectTime}] {disconnectedUser} left the chat.");
                    }
                }

                client.Close();
            }
        }
    }
}

