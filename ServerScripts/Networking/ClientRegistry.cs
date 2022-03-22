using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking
{
    public class ClientRegistry
    {
        private readonly ConcurrentQueue<ClientConnection> clients = new ConcurrentQueue<ClientConnection>();


        public void Connect(ClientConnection connection)
        {
            connection.SetStatus(ClientStatus.CONNECTING);
            this.clients.Enqueue(connection);
        }

        public ConcurrentQueue<ClientConnection> GetRegistry()
        {
            return clients;
        }
    }
}
