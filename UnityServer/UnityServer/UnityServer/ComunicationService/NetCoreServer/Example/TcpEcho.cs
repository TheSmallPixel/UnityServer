using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetCoreServer;
using UnityServer.Benchmark;

namespace TcpEchoServer
{
    class EchoClient : NetCoreServer.TcpClient
    {
        public EchoClient(string address, int port, int messages) : base(address, port)
        {
            _messages = messages;
        }

        protected override void OnConnected()
        {
            for (long i = _messages; i > 0; --i)
                SendAsync(TestEchoServer._messageToSend);
        }

        protected override void OnSent(long sent, long pending)
        {
            _sent += sent;
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            _received += size;
            while (_received >= TestEchoServer._messageToSend.Length)
            {
                SendAsync(TestEchoServer._messageToSend);
                _received -= TestEchoServer._messageToSend.Length;
            }
            
            TestEchoServer._totalBytes += size;
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Client caught an error with code {error}");
            ++TestEchoServer._totalErrors;
        }

   
        
        private long _sent;
        private long _received;
        private long _messages;
    }
    class EchoSession : TcpSession
    {
        public EchoSession(TcpServer server) : base(server) { }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            SendAsync(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Session caught an error with code {error}");
        }
    }

    class EchoServer : TcpServer
    {
        public EchoServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new EchoSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }

    public class TestEchoServer : Benchmark
    {
        EchoClient[] echoClients;
        public TestEchoServer(int clients = 100, int message = 1000, int size = 32, int seconds = 32) : base(clients,message,size,seconds){
            echoClients = new EchoClient[clients];
        }
        private EchoServer server;
        public override void StartServer()
        {
            server = new EchoServer(IPAddress.Any, _port);
           // server.OptionNoDelay = true;
            server.OptionReuseAddress = true;
            server.OptionReusePort = true;
            
            Console.WriteLine("Server starting...");
            server.Start();
        }
        


        public override void CreateClients()
        {
            for (int i = 0; i < _clients; ++i)
            {
                echoClients[i] = new EchoClient(_address, _port, _messages);
                //client.OptionNoDelay = true;
            }
        }

        public override void ConnectClients()
        {
            for (int i = 0; i < _clients; ++i)
                echoClients[i].ConnectAsync();

            do
            {
                Thread.Yield();
                Console.WriteLine("Clients connected:{0}/{1}", echoClients.Where(x => x.IsConnected).Count(), echoClients.Count());
            } while (echoClients.Any(x => !x.IsConnected));
        }

        public override void DisconnetcClients()
        {
            Console.WriteLine("Clients connected:{0}/{1}",echoClients.Where(x=>x.IsConnected).Count(),echoClients.Count());
            Console.WriteLine("Clients disconnecting...");
            for (int i = 0; i < _clients; ++i)
            {
                if(echoClients[i].IsConnected)
                echoClients[i].Disconnect();
            }
            for (int i = 0; i < _clients; ++i)
                while (echoClients[i].IsConnected)
                    Thread.Yield();
            Console.WriteLine("All clients disconnected!");
        }

        public override void TestClients()
        {

            Console.Write("Benchmarking...");
            Thread.Sleep(_seconds * 1000);
            Console.WriteLine("Done!");
        }

        public override void StopServer()
        {
            server.Stop();
        }
    }
}
