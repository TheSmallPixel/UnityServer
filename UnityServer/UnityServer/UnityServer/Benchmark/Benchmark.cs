using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Benchmark
{
    public abstract class Benchmark
    {
        public static byte[] _messageToSend;
        public static long _totalErrors;
        public static long _totalBytes;
        public static long _totalMessages;
        protected Stopwatch _stopwatch = new Stopwatch();
        protected string _address = "127.0.0.1";
        protected int _port = 11111;
        protected int _clients;
        protected int _messages;
        protected int _size;
        protected int _seconds;

        public Benchmark(int clients,int message,int size,int seconds)
        {
            _clients = clients;
            _messages = message;
            _size = size;
            _seconds = seconds;
            _messageToSend = new byte[size];
        }
        public abstract void StartServer();
        public abstract void StopServer();

        public abstract void CreateClients();
        public abstract void ConnectClients();
        public abstract void DisconnetcClients();
        public abstract void TestClients();
        
        public void Benchmacrk()
        {
            Task.Run(() => StartServer());
            CreateClients();
            ConnectClients();
            _stopwatch.Start();
            TestClients();
            _stopwatch.Stop();
            DisconnetcClients();
            StopServer();
            Result();

        }
        public void Result()
        {
            Console.WriteLine($"Errors: {_totalErrors}");

            Console.WriteLine();

            _totalMessages = _totalBytes / _size;

            Console.WriteLine($"Total data: {_totalBytes / 1024f / 1024f}");
            Console.WriteLine($"Total messages: {_totalMessages}");
            Console.WriteLine($"Data throughput: {(long)((_totalBytes / 1024f / 1024f) / _stopwatch.Elapsed.TotalSeconds)}/s");
            if (_totalMessages > 0)
            {
                Console.WriteLine($"Message latency: {_stopwatch.Elapsed.TotalMilliseconds / _totalMessages}");
                Console.WriteLine($"Message throughput: {(long)(_totalMessages / _stopwatch.Elapsed.TotalSeconds)} msg/s");
            }
        }
    }
}
