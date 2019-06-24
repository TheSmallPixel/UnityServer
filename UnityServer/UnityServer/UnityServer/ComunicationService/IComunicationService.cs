using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace UnityServer.ComunicationService
{
    public abstract class IComunicationServiceClient
    {
        public abstract bool Connect(string ip, string port);

        public abstract bool Send(object data);

        public abstract void Recive();
    }
    public abstract class IComunicationServiceServer
    {
        public abstract bool Start(int port);

        public abstract bool Send(Socket socket, object data);

        public abstract void StartRecive();
    }
}
