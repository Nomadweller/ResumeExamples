using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankWars;
using World;
using System.Windows.Forms;

namespace Controller
{
    /// <summary>
    /// A game controller to handle events for user input and messages sent to the client from the server, as well as connecting to that client initially.
    /// </summary>
    public class GameController
    {
        //Creating events to handle certain things that need to happen on and from the view when the client becomes connected and whenever data is received
        public delegate void MessagesArrivedHandler(TheWorld w);
        public delegate string ConnectedHandler();

        public event ConnectedHandler Connected;
        public event MessagesArrivedHandler MessagesArrived;

        //flag to figure out when the client can start sending data.
        private bool receivedWalls;
        //general items controller needs to know about to function.
        private TheWorld world;
        private SocketState mySS;
        //Vector needed to steer tank.
        private Vector2D mouseLocation;
        //commands to be sent to the server
        private string moving = "\"moving\":\"none\"";
        private string firing = "\"fire\":\"none\"";
        private string turretDir;
        private string LeftOver = "";

        /// <summary>
        /// Method used to connect the game to a server to be played on using port 11000
        /// </summary>
        /// <param name="hostname">The domain the game will try to access</param>
        public void ConnectToServer(string hostname)
        {
            Networking.ConnectToServer(OnConnect, hostname, 11000);
        }
        /// <summary>
        /// Getter to return the world the game creates.
        /// </summary>
        /// <returns>The world the game creates</returns>
        public TheWorld GetWorld()
        {
            return world;
        }
        /// <summary>
        /// Method to evaluate commands received from view and move the tank.
        /// </summary>
        /// <param name="e">Key pressed by user.</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                PlayerMove("up");
            else if (e.KeyCode == Keys.A)
                PlayerMove("left");
            else if (e.KeyCode == Keys.S)
                PlayerMove("down");
            else if (e.KeyCode == Keys.D)
                PlayerMove("right"); ;
        }


        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
                PlayerMove("up");
            else if (e.KeyChar == 'a')
                PlayerMove("left");
            else if (e.KeyChar == 's')
                PlayerMove("down");
            else if (e.KeyChar == 'd')
                PlayerMove("right"); ;
        }

        /// <summary>
        /// Method to stop movement
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyUp(KeyEventArgs e)
        {

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.A ||
                e.KeyCode == Keys.S || e.KeyCode == Keys.D)
            {
                moving = "\"moving\":\"none\"";
            }
        }

        /// <summary>
        /// Method to stop shooting
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseUp(MouseEventArgs e)
        {
            firing = "\"fire\":\"none\"";
        }




        /// <summary>
        /// Method to evaluate Mouse position and rotate turret.
        /// </summary>
        /// <param name="e">Cursor on computer.</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (world == null)
            {
                return;
            }
            mouseLocation = new Vector2D(e.X, e.Y);
            PlayerAim(mouseLocation, new Vector2D(425, 425));

        }
        /// <summary>
        /// method to evaluate mouse click and fire attacks.
        /// </summary>
        /// <param name="e">Cursor on computer.</param>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PlayerFire("main");
            }

            if (e.Button == MouseButtons.Right)
            {
                PlayerFire("alt");
            }
        }
        /// <summary>
        /// Method used by view once each frame in order to send data to a server.
        /// </summary>
        public void TalkToserver()
        {
            
            if (!mySS.TheSocket.Connected)
            {
                
                return;
            }
            if (string.IsNullOrEmpty(moving) || string.IsNullOrEmpty(firing) || string.IsNullOrEmpty(turretDir))
            {
                Console.WriteLine("here");
                return;
            }

            Networking.Send(mySS.TheSocket, "{" + moving + "," + firing + "," + turretDir + "}\n");

        }
        /// <summary>
        /// Method that handles all the things needed when a client conencts to a server and then begins a perpetual event loop to receive data back from that server to use.
        /// </summary>
        /// <param name="socketToConnect">Socketstate the game is connected on</param>
        private void OnConnect(SocketState socketToConnect)
        {
            //set global socket to be used outside of method
            mySS = socketToConnect;
            //sets up player's name and sends it to the server
            string name = Connected();
            Networking.Send(mySS.TheSocket, name + "\n");
            //starts event loop 
            Networking.GetData(mySS);
            mySS.OnNetworkAction = OnReceive;
        }
        /// <summary>
        /// Method used to begin parsing messages received from server and subsequently clear the messages, in order to receive more messages from the server to handle via an event loop.
        /// </summary>
        /// <param name="socketToConnect">Socketstate that the game is connected on.</param>
        private void OnReceive(SocketState socketToConnect)
        {
            ParseMessage(socketToConnect.GetData());
            socketToConnect.ClearData();
            socketToConnect.OnNetworkAction = OnReceive;
            Networking.GetData(socketToConnect);
        }
        /// <summary>
        /// Method to parse through each piece of information being sent to the client.
        /// </summary>
        /// <param name="data">string sent to client by server</param>
        private void ParseMessage(string data)
        {
            object newItem;

            data = String.Concat(LeftOver, data);
            LeftOver = "";
            //handling of partial messages.
            if (!data.EndsWith(@"\n"))
            {
                int index = data.LastIndexOf('\n');
                LeftOver = data.Substring(index + 1);
                data = data.Substring(0, index);
            }
            string[] parsedMessages = data.Split('\n');
            //catches first few lines are code that are not json strings which cannot be serialized.
            if (int.TryParse(parsedMessages[0], out int result))
            {
                parsedMessages = SetUpWorld(parsedMessages);
            }
            foreach (string jsonString in parsedMessages)
            {
                if (!String.IsNullOrEmpty(jsonString))
                {
                    if (world == null)
                    {
                        throw new Exception("Something went wrong setting up the world");
                    }
                    
                    //Figures out the type of the JSON string in order to deserialize properly
                    int start = jsonString.IndexOf('"');
                    int end = jsonString.Substring(start + 1).IndexOf('"');
                    string type = jsonString.Substring(start + 1, end);

                    JObject obj = JObject.Parse(jsonString);
                    JToken token = obj[type];

                    newItem = FromJson(type, jsonString);
                    if (newItem != null)
                    {
                        lock (world)
                        {
                            //adds object to the game world to be drawn on next frame
                            world.AddObject((int)token, newItem);
                        }
                    }
                }
            }
            //fires events in the view that need to happen when messages are done arriving.
            MessagesArrived(world);
        }

        /// <summary>
        /// Method used to deserialize a JSON string from server and identify the type of that object.
        /// </summary>
        /// <param name="type">The type of object located in the Jsonstring.</param>
        /// <param name="jsonString">The Json string itself that needs to be serialized.</param>
        /// <returns>A newly deserialize object</returns>
        private object FromJson(string type, string jsonString)
        {

            switch (type)
            {
                case "tank":
                    Tank newTank = JsonConvert.DeserializeObject<Tank>(jsonString);
                    if (receivedWalls == false)
                        receivedWalls = true;
                    return newTank;
                case "wall":
                    Wall newWall = JsonConvert.DeserializeObject<Wall>(jsonString);
                    return newWall;
                case "proj":
                    Projectile newProjectile = JsonConvert.DeserializeObject<Projectile>(jsonString);
                    return newProjectile;
                case "power":
                    Powerup newPower = JsonConvert.DeserializeObject<Powerup>(jsonString);
                    return newPower;
                case "beam":
                    Beam newBeam = JsonConvert.DeserializeObject<Beam>(jsonString);
                    return newBeam;
                default: return null;
            }
        }
        /// <summary>
        /// Method to handle the first messages received by the server before it starts sending JSON and setup the drawing panel w/ world size and player according to this information.
        /// </summary>
        /// <param name="message">Message being sent from the server.</param>
        /// <returns>String array of additional messages not handled by this method</returns>
        private string[] SetUpWorld(string[] message)
        {
            if (int.TryParse(message[0], out int playerID) && int.TryParse(message[1], out int worldSize))
            {
                world = new TheWorld(worldSize, playerID);
            }
            else
            {
                throw new Exception("Something went wrong setting up the world");
            }
            return message.Skip(2).ToArray();
        }


        /// <summary>
        /// Method used to setup string to send to the server to move the player.
        /// </summary>
        /// <param name="s">String representing direction the player moved</param>
        private void PlayerMove(string s)
        {
            /*if (string.IsNullOrEmpty(s))
            {
                return;
            }
            if (mySS == null || !mySS.TheSocket.Connected)
            {
                return;
            }*/

            switch (s)
            {
                case "up":
                    moving = "\"moving\":\"up\"";
                    break;
                case "left":
                    moving = "\"moving\":\"left\"";
                    break;
                case "down":
                    moving = "\"moving\":\"down\"";
                    break;
                case "right":
                    moving = "\"moving\":\"right\"";
                    break;
            }
        }
        /// <summary>
        /// When player fires registers the string to be sent to the server.
        /// </summary>
        /// <param name="fireMode">String representing type of attack</param>
        private void PlayerFire(string fireMode)
        {
            firing = "\"fire\":\"" + fireMode + "\"";
        }


        /// <summary>
        /// Gets the players aim and applies it to a string to be sent to the server when needed.
        /// </summary>
        /// <param name="mouseLoc">The current position of the cursor.</param>
        /// <param name="playerLoc">The center of the screen where the player is located</param>
        private void PlayerAim(Vector2D mouseLoc, Vector2D playerLoc)
        {
            Vector2D turretAim = mouseLoc - playerLoc;
            turretAim.Normalize();
            double x = turretAim.GetX();
            double y = turretAim.GetY();
            turretDir = "\"tdir\":{\"x\":" + x + ",\"y\":" + y + "}";
        }
    }
}