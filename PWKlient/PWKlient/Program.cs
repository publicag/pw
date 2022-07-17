using PWKlient.Config;
using PWKlient.Connection;
using PWKlient.Model;
using System;
using System.Threading.Tasks;

namespace PWKlient
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Podaj nazwę użytkownika: ");
            var name = Console.ReadLine();
            var user = new User
            {
                Name = name
            };

            Connector connector = new Connector(user);

            await connector.ConnectToServer();
            await connector.LoginToServer();

            FilesModel filesModel = null;

            while (filesModel == null)
            {
                Console.WriteLine("Pliki:");
                var json = Console.ReadLine();
                try
                {
                    filesModel = FilesModel.Deserialize(json);
                }
                catch (Exception)
                {
                    Console.WriteLine("Błędny plik.");
                }
            }

            await connector.SendFile(filesModel.Files);
        }

    }
}
