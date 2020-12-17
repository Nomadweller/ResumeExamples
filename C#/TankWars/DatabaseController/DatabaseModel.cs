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
    /// <summary>
    /// You should not need to modify this class.
    /// Use this class as a helper to generate HTTP + HTML responses.
    /// Your web server will first query the database to get the appropriate data,
    /// then pass that data to these helper methods to format it in HTML.
    /// </summary>
    public static class DatabaseModel
    {
        // HTTP and HTML headers/footers
        private const string httpOkHeader = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";
        private const string httpBadHeader = "HTTP/1.1 404 Not Found\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";
        private const string htmlHeader = "<!DOCTYPE html><html><head><title>TankWars</title></head><body>";
        private const string htmlFooter = "</body></html>";

        public const string connectionString = "server=atr.eng.utah.edu;" +
             "database=cs3500_u0990135;" +
             "uid=cs3500_u0990135;" +
             "password=thisisapassword";

        /// <summary>
        /// Returns an HTTP response indicating the request was bad
        /// </summary>
        /// <returns></returns>
        public static string Get404()
        {
            return httpBadHeader + WrapHtml("Bad http request");
        }

        public static void SaveGame(int duration)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    //command.CommandText = "select ID, duration, player from Games";
                    command.CommandText = "INSERT INTO `cs3500_u0990135`.`Games` (`Duration`) VALUES ('"+ duration + "');";
                    command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        public static void SaveTank(Tank t)
        {
            uint GameID = 0;
            uint playerID = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    //command.CommandText = "select ID, duration, player from Games";
                    command.CommandText = "INSERT INTO `cs3500_u0990135`.`Players` (`PlayerName`) VALUES ('"+  t.GetName() + "');";
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT * FROM cs3500_u0990135.Games;";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GameID = reader.GetUInt16("GameID");
                        }

                    }

                    command.CommandText = "SELECT * FROM cs3500_u0990135.Players;";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string playerName = reader.GetString("PlayerName");
                            if(playerName == t.GetName())
                            {
                                playerID = reader.GetUInt16("PlayerID");
                            }
                        }

                    }

                    command.CommandText = "INSERT INTO `cs3500_u0990135`.`GameSession` (`GameID`, `PlayerScore`, `PlayerID`) VALUES ('"+ GameID +"', '"+t.GetScore()+"', '" + playerID + "');";
                    command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static Dictionary<uint, GameModel> GetGamesFromDB()
        {
            Dictionary<uint, GameModel> retDict = new Dictionary<uint, GameModel>();


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    //command.CommandText = "select ID, duration, player from Games";
                    command.CommandText = "select * from Games NATURAL JOIN GameSession NATURAL JOIN Players;";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {


                            if (!retDict.ContainsKey(reader.GetUInt16("GameID")))
                            {
                                retDict.Add(reader.GetUInt16("GameID"), new GameModel(reader.GetUInt16("GameID"), 10));
                            }
                            retDict[reader.GetUInt16("GameID")].AddPlayer(reader.GetString("PlayerName"), reader.GetUInt16("PlayerScore"), 10);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
                return retDict;
            }

        }


        /// <summary>
        /// Returns an HTTP response containing HTML tables representing the given games
        /// Query your database to construct a dictionary of games to pass to this method
        /// </summary>
        /// <param name="games">Information about all games known</param>
        /// <returns></returns>
        public static string GetAllGames(Dictionary<uint, GameModel> games)
        {
            StringBuilder sb = new StringBuilder();

            foreach (uint gid in games.Keys)
            {
                sb.Append("Game " + gid + " (" + games[gid].Duration + " seconds)<br>");
                sb.Append("<table border=\"1\">");
                sb.Append("<tr><th>Name</th><th>Score</th><th>Accuracy</th></tr>");
                foreach (PlayerModel p in games[gid].GetPlayers())
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + p.Name + "</td>");
                    sb.Append("<td>" + p.Score + "</td>");
                    sb.Append("<td>" + p.Accuracy + "</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table><br><hr>");
            }

            return httpOkHeader + WrapHtml(sb.ToString());
        }

        /// <summary>
        /// Returns an HTTP response containing one HTML table representing the games
        /// that a certain player has played in
        /// Query your database for games played by the named player, then pass that name
        /// and the list of sessions to this method
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="games">The list of sessions the player has played</param>
        /// <returns></returns>
        public static string GetPlayerGames(string name, List<SessionModel> games)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Games for " + name + "<br>");
            sb.Append("<table border=\"1\">");
            sb.Append("<tr><th>GameID</th><th>Duration</th><th>Score</th><th>Accuracy</th></tr>");

            foreach (SessionModel s in games)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + s.GameID + "</td>");
                sb.Append("<td>" + s.Duration + "</td>");
                sb.Append("<td>" + s.Score + "</td>");
                sb.Append("<td>" + s.Accuracy + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table><br><hr>");

            return httpOkHeader + WrapHtml(sb.ToString());
        }

        /// <summary>
        /// Returns a simple HTTP greeting response
        /// </summary>
        /// <param name="numPlayers"></param>
        /// <returns></returns>
        public static string GetHomePage(int numPlayers)
        {
            return httpOkHeader + WrapHtml("Welcome to TankWars");
        }

        /// <summary>
        /// Helper for wraping a string in an HTML header and footer
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string WrapHtml(string content)
        {
            return htmlHeader + content + htmlFooter;
        }

    }

    /// <summary>
    /// A simple container class representing one player in one game
    /// </summary>
    public class PlayerModel
    {
        public readonly string Name;
        public readonly uint Score;
        public readonly uint Accuracy;
        public PlayerModel(string n, uint s, uint a)
        {
            Name = n;
            Score = s;
            Accuracy = a;
        }
    }


    /// <summary>
    /// A simple container class representing one game and its players
    /// </summary>
    public class GameModel
    {
        public readonly uint ID;
        public readonly uint Duration;
        private List<PlayerModel> players;

        public GameModel(uint id, uint d)
        {
            Duration = d;
            players = new List<PlayerModel>();
        }

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="score">The player's score</param>
        /// <param name="accuracy">The player's accuracy</param>
        public void AddPlayer(string name, uint score, uint accuracy)
        {
            players.Add(new PlayerModel(name, score, accuracy));
        }

        /// <summary>
        /// Returns the players in this game
        /// </summary>
        /// <returns></returns>
        public List<PlayerModel> GetPlayers()
        {
            return players;
        }

    }

    /// <summary>
    /// A simple container class representing the information about one player's session in one game
    /// </summary>
    public class SessionModel
    {
        public readonly uint GameID;
        public readonly uint Duration;
        public readonly uint Score;
        public readonly uint Accuracy;

        public SessionModel(uint gid, uint dur, uint score, uint acc)
        {
            GameID = gid;
            Duration = dur;
            Score = score;
            Accuracy = acc;
        }

    }

}
