using Microsoft.AspNet.SignalR.Client;
using PWKlient.Config;
using PWKlient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PWKlient.Connection
{
    public class ServerConnection
    {
        public event Action TransferEnded;
        public event Action<string> FileProcessed;
        private IHubProxy hubProxy;
        private HubConnection connection;

        private string url = "http://localhost:8080/pw";
        public async Task ConnectAsync()
        {
            connection = new HubConnection(url);
            hubProxy = connection.CreateHubProxy("PWHub");
            hubProxy.On("Finished", () => TransferEnded?.Invoke());
            hubProxy.On<string>("FileProcessed", (n) => FileProcessed?.Invoke(n));

            await connection.Start();
        }

        public async Task<List<User>> LoginAsync(string name)
        {
            return await hubProxy.Invoke<List<User>>("Login", new object[] { name });
        }

        public async Task SendFileAsync(ICollection<TransferFile> transferFiles)
        {
            await hubProxy.Invoke("BroadcastFiles", transferFiles);
        }

    }
}
