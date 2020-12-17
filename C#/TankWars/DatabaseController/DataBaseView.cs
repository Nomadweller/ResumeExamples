using MySql.Data.MySqlClient;
using NetworkUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World;

namespace DatabaseController
{
    public class DataBaseView
    {


        public DataBaseView()
        {
            Networking.StartServer(HandleHTTPConnection, 80);
            Console.WriteLine("Database Server Started");
        }

        private static void HandleHTTPConnection(SocketState state)
        {
            state.OnNetworkAction = ServeHttpRequest;
            Networking.GetData(state);
        }

        private static void ServeHttpRequest(SocketState state)
        {
            StringBuilder sb = new StringBuilder();
            string data = state.GetData();
            state.ClearData();
            if (String.IsNullOrEmpty(data))
            {
                Networking.GetData(state);
                return;
            }

            if (data.Contains("GET"))
            {
               Networking.SendAndClose(state.TheSocket, DatabaseModel.GetHomePage(0) +  DatabaseModel.GetAllGames(DatabaseModel.GetGamesFromDB()));
            }

        }

    }
}
