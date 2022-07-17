using Microsoft.AspNet.SignalR;
using PWSerwer.Client;
using PWSerwer.Dispatcher;
using PWSerwer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PWSerwer.Hub
{
    public class PWHub : Hub<IClient>
    {
        private static ConcurrentDictionary<string, User> ServerClients = new ConcurrentDictionary<string, User>();
        private static ConcurrentDictionary<string, IClient> ClientsDictionary = new ConcurrentDictionary<string, IClient>();
        private DispatcherProcessor dispatcher = DispatcherProcessor.Instance;
        public List<User> Login(string name)
        {
            if (!ServerClients.ContainsKey(name))
            {
                List<User> users = new List<User>(ServerClients.Values);
                User newUser = new User { Name = name };
                var added = ServerClients.TryAdd(name, newUser);
                ClientsDictionary.TryAdd(name, Clients.Caller);
                if (!added) return null;
                Clients.CallerState.UserName = name;
                ProcessingFinished();
                return users;
            }
            return null;
        }

        public void BroadcastFiles(ICollection<TransferFile> files)
        {
            var name = Clients.CallerState.UserName;

            if (!string.IsNullOrEmpty(name))
            {
                dispatcher.AddToQueue(files, name);
            }

            dispatcher.Process(ClientsDictionary);
        }

        public void ProcessingFinished()
        {
            Clients.Caller.Finished();
        }
    }
}
