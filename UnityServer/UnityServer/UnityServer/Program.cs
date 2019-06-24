using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TcpEchoServer;

namespace UnityServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestEchoServer test = new TestEchoServer(clients: 100000,message:40,size: 1,seconds: 10);
                test.Benchmacrk();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.ReadLine();
        }

    }
    public static class Extension
    {
        public static void WaitAll(this List<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
        }
    }
}
