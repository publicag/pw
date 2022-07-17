using PWKlient.Config;
using PWKlient.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWKlient.Connection
{
    public class Connector
    {
        private User _user;
        private ServerConnection _serverConnection;
        private bool _isConnected = false;

        public Connector(User user)
        {
            _user = user;
            _serverConnection = new ServerConnection();

            _serverConnection.TransferEnded += TransferEnded;
            _serverConnection.FileProcessed += FileProcessed;
        }

        public async Task<bool> ConnectToServer()
        {
            try
            {
                if (_isConnected == false)
                {
                    await _serverConnection.ConnectAsync();
                    _isConnected = true;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> LoginToServer()
        {
            try
            {
                List<User> users = new List<User>();
                users = await _serverConnection.LoginAsync(_user.Name);
                if (users != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception) { return false; }
        }

        public async Task<bool> SendFile(ICollection<TransferFile> transferFiles)
        {
            try
            {
                await _serverConnection.SendFileAsync(transferFiles);
                return true;
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void TransferEnded()
        {
            Console.WriteLine("Połączono.");
        }

        private void FileProcessed(string file)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(file);
        }
    }
}
