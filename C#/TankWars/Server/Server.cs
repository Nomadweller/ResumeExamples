using System;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            ServerCtrl controller = new ServerCtrl(@"..\..\..\Resources\Libraries\settings.xml");
            Console.WriteLine();
            Console.ReadLine();
            controller.SaveGame();
            Console.WriteLine("Game has been Saved WebServer is still running.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
