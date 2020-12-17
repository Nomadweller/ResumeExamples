using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
using TankWars;
using World;
using DatabaseController;

namespace Server
{
    public class ServerCtrl
    {
        private int worldSize;
        private int shotDelay;
        private int FPS;
        private int respawnTime;
        private int playerHP;
        private double playerSpeed;
        private int playerSize;
        private int wallSize;
        private int powerupCount;
        private int maxPowerupSpawnTime;
        private SocketState serverSocket;
        private Dictionary<long, SocketState> playerSockets;
        private Dictionary<long, Tank> players;
        private TheWorld world;
        private bool wallsSent;
        private int projectileCount;
        private int beamCount;
        private int powerUpTracker;
        private int powerUpID;
        private int projectileDelay;
        private bool working;
        private int timeSinceLastPowerUp;
        private int randPowerupTime;
        private Random rand;
        private double turretAimx;
        private double turretAimy;

        DataBaseView dataBaseView;



        public ServerCtrl(string filePath)
        {
            players = new Dictionary<long, Tank>();
            playerSockets = new Dictionary<long, SocketState>();
            wallsSent = false;
            SetDefaults();
            rand = new Random();
            randPowerupTime = rand.Next(0, maxPowerupSpawnTime);
            projectileCount = 0;
            beamCount = 0;
            powerUpTracker = 0;
            powerUpID = 0;
            projectileDelay = 0;
            turretAimx = 0;
            turretAimy = 0;
            working = false;
            world = new TheWorld(worldSize, 0);


            Load(filePath);
            Networking.StartServer(NewPlayer, 11000);
            Console.WriteLine("Server started");

            dataBaseView = new DataBaseView();


            Thread thread = new Thread(() => SendData());
            thread.Start();
        }

        private void NewPlayer(SocketState state)
        {
            string worldSizeString = worldSize.ToString();
            string playerIDString = state.ID.ToString();
            if (state.ErrorOccured)
                return;
            lock (playerSockets)
            {
                playerSockets[state.ID] = state;
            }
            Networking.Send(state.TheSocket, playerIDString + "\n");
            Networking.Send(state.TheSocket, worldSizeString + "\n");

            state.OnNetworkAction = ProcessMessage;
            Networking.GetData(state);
        }

        /// <summary>
        /// Detects the incoming messages of new players when they connect then calls Get Data in order to create an event loop to continue listening for 
        /// new incoming players
        /// </summary>
        /// <param name="state">Connection of incoming player.</param>
        private void ProcessMessage(SocketState state)
        {
            string data;
            long ID = state.ID;
            
            string[] movingCommands;

            if (state.ErrorOccured)
            {

                PlayerDisconnect(state);
                Networking.SendAndClose(state.TheSocket, "error");
                //Networking.GetData(state);
                return;
            }

            data = state.GetData();
            state.ClearData();
            if (String.IsNullOrEmpty(data))
            {
                Networking.GetData(state);
                return;
            }


            //checks to see if user is sending a user command or name
            if (data.StartsWith("{\"moving\""))
            {
                movingCommands = data.Split(',');
                foreach (string s in movingCommands)
                {
                    string[] command = s.Split(':');
                    foreach (string t in command)
                    {
                        string tempString = t.Replace("\"", String.Empty);
                        string tempString2 = tempString.Replace("{", String.Empty);
                        string stringToCompare = tempString2.Replace("}", String.Empty);
                        switch (stringToCompare)
                        {
                            case "none":
                                break;
                            case "up":
                                MoveTank(new Vector2D(0, playerSpeed * -1), new Vector2D(0, -1), players[ID]);
                                break;
                            case "down":
                                MoveTank(new Vector2D(0, playerSpeed), new Vector2D(0, 1), players[ID]);
                                break;
                            case "left":
                                MoveTank(new Vector2D(playerSpeed * -1, 0), new Vector2D(-1, 0), players[ID]);
                                break;
                            case "right":
                                MoveTank(new Vector2D(playerSpeed, 0), new Vector2D(1, 0), players[ID]);
                                break;
                            case "main":
                                FireProjectile(players[ID]);
                                break;
                            case "alt":
                                FireBeam(players[ID]);
                                break;
                            case "x":
                                if (double.TryParse(command[2], out double anglex))
                                    turretAimx = anglex;
                                break;
                            case "y":
                                if (command[1].Length > 4)
                                {
                                    if (double.TryParse(command[1].Substring(0, command[1].Length - 5), out double angley))
                                        turretAimy = angley;
                                    players[ID].SetAim(new Vector2D(turretAimx, turretAimy));
                                }
                                break;
                        }
                    }
                }
            }
            else if (!players.ContainsKey(ID))
            {
                Console.WriteLine("Player " + state.ID.ToString() + " Connected.");
                CreateTank(data, ID);
                Sendwalls(state);
                Networking.GetData(state);
                return;
            }
            Networking.GetData(state);
        }
        private void SendData()
        {
            while (true)
            {
                List<SocketState> playerSocketCopy = new List<SocketState>(playerSockets.Values);
                StringBuilder sb = new StringBuilder();
                List<long> listToRemove = new List<long>();
                var prev = DateTime.Now;

                //For every socket connected to this server send a frame
                foreach (SocketState state in playerSocketCopy)
                {

                    foreach (Tank t in world.GetTanks())
                    {
                        t.projectileDelay += 1;
                    }
                    if (powerUpTracker < powerupCount)
                        timeSinceLastPowerUp += FPS;
                    if (timeSinceLastPowerUp > randPowerupTime)
                    {
                        SpawnPowerup();
                        randPowerupTime = rand.Next(0, maxPowerupSpawnTime);
                        timeSinceLastPowerUp = 0;
                    }

                    //Seems innefficient to do this for every tank but.. TODO
                    lock (world)
                    {
                        foreach (Tank t in world.GetTanks())
                        {
                            sb.Append(JsonConvert.SerializeObject(t) + "\n");
                            if (t.IsDead())
                                RespawnTank(t);
                            if (t.IsDisconnected())
                                listToRemove.Add(t.GetID());
                        }

                        foreach (long ID in listToRemove)
                        {
                            world.RemoveTankByID((int)ID);
                        }
                        listToRemove.Clear();

                        foreach (Projectile p in world.GetProjectiles())
                        {
                            MoveProjectile(p);
                            sb.Append(JsonConvert.SerializeObject(p) + "\n");
                            if (p.IsDead())
                                listToRemove.Add(p.GetID());
                        }
                        foreach (long ID in listToRemove)
                        {
                            world.RemoveProjectile(ID);
                        }

                        listToRemove.Clear();
                        foreach (Beam b in world.GetBeams())
                        {
                            sb.Append(JsonConvert.SerializeObject(b) + "\n");
                            listToRemove.Add(b.GetID());
                        }
                        foreach (long ID in listToRemove)
                        {
                            world.RemoveBeam(ID);
                        }
                        listToRemove.Clear();
                        foreach (Powerup up in world.GetPowerups())
                        {
                            sb.Append(JsonConvert.SerializeObject(up) + "\n");
                            if (up.IsDead())
                                listToRemove.Add(up.GetID());
                        }
                        if (!working)
                        {
                            foreach (long ID in listToRemove)
                            {

                                world.RemovePowerup(ID);
                                powerUpTracker--;
                            }
                        }
                    }
                    listToRemove.Clear();
                    List<SocketState> mySockets = new List<SocketState>(playerSockets.Values);
                    foreach (SocketState player in mySockets)
                    {
                        Networking.Send(player.TheSocket, sb.ToString());
                    }
                    
                    //Console.WriteLine(sb.ToString());
                    sb.Clear();
                }
                //managing the framerate for when this loop repeats
                var now = DateTime.Now;
                if (now <= prev.AddMilliseconds(FPS))
                {
                    TimeSpan timeSpan = prev.AddMilliseconds(FPS) - now;
                    System.Threading.Thread.Sleep((int)timeSpan.TotalMilliseconds);
                }
                prev = DateTime.Now;
            }
        }

        //Send the walls to the player on initial connect
        private void Sendwalls(SocketState state)
        {
            StringBuilder sb = new StringBuilder();
            lock (world)
            {
                foreach (Wall w in world.GetWalls())
                {
                    sb.Append(JsonConvert.SerializeObject(w) + "\n");

                }
            }
            Networking.Send(state.TheSocket, sb.ToString());
            //Console.WriteLine(sb.ToString());
            sb.Clear();
        }

        private void CreateTank(string data, long ID)
        {
            Tank newTank = new Tank();
            players[ID] = newTank;
            newTank.SetAim(new Vector2D(0, 0));
            newTank.SetHealth(playerHP);
            newTank.SetID((int)ID);
            newTank.SetLocation(FindEmptyLocation());
            newTank.SetName(data);
            newTank.SetOrientation(new Vector2D(0, 0));
            newTank.SetScore(0);
            newTank.SwitchConnected();
            newTank.SwitchDisconnected();
            lock (world)
            {
                world.AddObject((int)ID, newTank);
            }
        }
        private void SpawnPowerup()
        {
            if (powerUpTracker < powerupCount)
            {
                Powerup p = new Powerup();
                p.SetID(powerUpID);
                p.SetLocation(FindEmptyLocation());
                p.SwitchDead();
                lock (world)
                {
                    world.AddObject(powerUpID, p);
                }
                powerUpID++;
                powerUpTracker++;
            }
        }
        /// <summary>
        /// Method to move tanks through world on player command
        /// </summary>
        /// <param name="movement">The direction they move</param>
        /// <param name="playerFacing">The orientationg of the tank after movement</param>
        /// <param name="t">The tank being moved</param>
        private void MoveTank(Vector2D movement, Vector2D playerFacing, Tank t)
        {
            Vector2D startLocation = t.GetLocation();
            Vector2D endLocation = startLocation + movement;

            //wallCollision
            lock (world)
            {
                foreach (Wall w in world.GetWalls())
                {
                    if (CheckOverlap(w, endLocation.GetX(), endLocation.GetY()))
                        return;
                }
            }
            //worldWrap
            //too left
            if (endLocation.GetX() - (playerSize / 2) < -(worldSize / 2))
                endLocation = new Vector2D((worldSize / 2) - (playerSize / 2), endLocation.GetY());
            //too right
            else if (endLocation.GetX() + (playerSize / 2) > (worldSize / 2))
                endLocation = new Vector2D(-(worldSize / 2) + (playerSize / 2), endLocation.GetY());
            //too low
            else if (endLocation.GetY() - (playerSize / 2) < -(worldSize / 2))
                endLocation = new Vector2D(endLocation.GetX(), (worldSize / 2) - (playerSize / 2));
            //too high
            else if (endLocation.GetY() + (playerSize / 2) > (worldSize / 2))
                endLocation = new Vector2D(endLocation.GetX(), -(worldSize / 2) + (playerSize / 2));
            t.SetLocation(endLocation);
            t.SetOrientation(playerFacing);
            working = true;
            lock (world)
            {
                foreach (Powerup p in world.GetPowerups())
                {
                    if (PointIntersect(p.GetLocation(), t.GetLocation(), t.GetLocation(), playerSize + 25))
                    {
                        p.SwitchDead();
                        t.beamCount++;
                    }
                }
            }
            working = false;
        }

        private void FireProjectile(Tank t)
        {

            if (t.projectileDelay > shotDelay)
            {
                Projectile p = new Projectile();
                p.SetID(projectileCount);
                p.SetLocation(t.GetLocation());
                p.SetOrientation(t.GetAim());
                p.SetOwnerID(t.GetID());
                p.SwitchDead();
                lock (world)
                {
                    world.AddObject(projectileCount, p);
                }
                projectileCount++;
                t.projectileDelay = 0;
            }
        }
        private void FireBeam(Tank t)
        {
            if (t.beamCount > 0)
            {
                Beam b = new Beam();
                b.SetDirection(t.GetAim());
                b.SetID(beamCount);
                b.SetLocation(t.GetLocation());
                lock (world)
                {
                    b.SetOwner(t.GetID());
                    world.AddObject(beamCount, b);
                    foreach (Tank opponent in world.GetTanks())
                    {
                        if (opponent.GetID() == t.GetID())
                            continue;
                        if (Intersects(b.GetLocation(), b.GetDirection(), opponent.GetLocation(), playerSize / 2))
                        {
                            opponent.SwitchDead();
                            RespawnTank(opponent);
                            PointIncrement(t);
                        }
                    }
                }

                //checkforcollision
                //then delete
                beamCount++;
                t.beamCount--;
            }
        }
        private void PointIncrement(Tank t)
        {
            t.SetScore(t.GetScore() + 1);
        }
        private void RespawnTank(Tank t)
        {
            t.SetLocation(FindEmptyLocation());
            t.SetHealth(playerHP);
            t.SwitchDead();
        }

        private void MoveProjectile(Projectile p)
        {
            Vector2D startLocation = p.GetLocation();
            Vector2D scaledDirection = p.GetOrientation() * 20;
            Vector2D endLocation = startLocation + scaledDirection;
            lock (world)
            {
                foreach (Wall w in world.GetWalls())
                {
                    if (PointIntersect(endLocation, w.GetStartLocation(), w.GetEndLocation(), wallSize))
                        p.SwitchDead();
                }
                foreach (Tank t in world.GetTanks())
                {
                    if (t.GetID() == p.GetOwnerID())
                        continue;
                    if (PointIntersect(endLocation, t.GetLocation(), t.GetLocation(), playerSize))
                    {
                        p.SwitchDead();
                        if (t.GetID() != p.GetOwnerID())
                        {
                            t.SetHealth(t.GetHealth() - 1);
                            if (t.GetHealth() == 0)
                            {
                                PointIncrement(world.GetTankByID(p.GetOwnerID()));
                            }
                        }
                    }
                }
            }
            p.SetLocation(endLocation);
        }
        private Vector2D FindEmptyLocation()
        {
            bool collisionDetection = true;
            Vector2D RetVal = new Vector2D(0, 0);
            Random rnd = new Random();
            double tempx;
            double tempy;
            //If a collision was detected start over 
            while (collisionDetection)
            {
                //Assume no collision has been detected
                collisionDetection = false;
                tempx = rnd.Next((-(worldSize / 2)) + (wallSize / 2), (worldSize / 2) - (wallSize / 2));
                tempy = rnd.Next((-(worldSize / 2)) + (wallSize / 2), (worldSize / 2) - (wallSize / 2));
                lock (world)
                {
                    foreach (Wall w in world.GetWalls())
                    {
                        if (CheckOverlap(w, tempx, tempy))
                        {
                            //If there was a collision set to true
                            collisionDetection = true;
                        }
                        RetVal = new Vector2D(tempx, tempy);
                    }
                }
            }
            return RetVal;
        }
        private bool CheckOverlap(Wall w, double tankX, double tankY)
        {
            double wallX1 = w.GetStartLocation().GetX();
            double wallY1 = w.GetStartLocation().GetY();
            double wallX2 = w.GetEndLocation().GetX();
            double wallY2 = w.GetEndLocation().GetY();

            double wallMinX = minMaxWallCheck(wallX1, wallX2).Item1;
            double wallMaxX = minMaxWallCheck(wallX1, wallX2).Item2;
            double wallMinY = minMaxWallCheck(wallY1, wallY2).Item1;
            double wallMaxY = minMaxWallCheck(wallY1, wallY2).Item2;

            double tankMinX = tankX - playerSize / 2;
            double tankMaxX = tankX + playerSize / 2;
            double tankMinY = tankY - playerSize / 2;
            double tankMaxY = tankY + playerSize / 2;


            //Check Rectangle collision
            return doOverlap(new Tuple<double, double>(tankMinX, tankMinY),
            new Tuple<double, double>(tankMaxX, tankMaxY),
            new Tuple<double, double>(wallMinX, wallMinY),
            new Tuple<double, double>(wallMaxX, wallMaxY));
        }
        private Tuple<double, double> minMaxWallCheck(double p1, double p2)
        {
            double minValue;
            double maxValue;
            if (p1 < p2)
            {

                minValue = p1 - wallSize / 2;
                maxValue = p2 + wallSize / 2;
            }
            else
            {
                minValue = p2 - wallSize / 2;
                maxValue = p1 + wallSize / 2;
            }
            return new Tuple<double, double>(minValue, maxValue);
        }
        // Returns true if two rectangles (leftTop1, rightBottom1) and (leftTop2, rightBottom2) overlap 
        private bool doOverlap(Tuple<double, double> leftTop1, Tuple<double, double> rightBottom1,
                                Tuple<double, double> leftTop2, Tuple<double, double> rightBottom2)
        {
            // If one rectangle is on left side of other  
            if (leftTop1.Item1 > rightBottom2.Item1 || leftTop2.Item1 > rightBottom1.Item1)
            {
                return false;
            }
            // If one rectangle is above other  
            if (leftTop1.Item2 > rightBottom2.Item2 || leftTop2.Item2 > rightBottom1.Item2)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }
        private bool PointIntersect(Vector2D originPoint, Vector2D bottomLeftRect, Vector2D topRightRect, int sizeOfObject)
        {
            double objectX = originPoint.GetX();
            double objectY = originPoint.GetY();
            double minX = bottomLeftRect.GetX() - sizeOfObject / 2;
            double minY = bottomLeftRect.GetY() - sizeOfObject / 2;
            double maxX = topRightRect.GetX() + sizeOfObject / 2;
            double maxY = topRightRect.GetY() + sizeOfObject / 2;
            if (minX > maxX)
            {
                double tempX = minX;
                minX = maxX;
                maxX = tempX;
            }
            if (minY > maxY)
            {
                double tempY = minY;
                minY = maxY;
                maxY = tempY;
            }
            if (objectX >= minX & objectX <= maxX & objectY >= minY & objectY <= maxY)
                return true;
            return false;
        }


        /// <summary>
        /// Sets defaults for world in case settings file does not contain them.
        /// </summary>
        private void SetDefaults()
        {
            worldSize = 1200;
            shotDelay = 80;
            FPS = 17;
            respawnTime = 300;
            playerHP = 3;
            playerSpeed = 2.9;
            playerSize = 60;
            wallSize = 50;
            powerupCount = 2;
            maxPowerupSpawnTime = 16500;
        }
        /// <summary>
        /// Loads a settings file to set server variables to be used for computation.
        /// </summary>
        /// <param name="filePath">The location of the settings file.</param>
        private void Load(string filePath)
        {
            string currentLine;
            int wallStart;
            int WallEnd;
            int tempx;
            int tempy;
            int wallID = 0;
            Vector2D wallLocation;
            //Wall walltoAdd = new Wall();
            try
            {
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            currentLine = reader.Name;
                            switch (currentLine)
                            {
                                case "UniverseSize":
                                    worldSize = reader.ReadElementContentAsInt();
                                    break;
                                case "MSPerFrame":
                                    FPS = reader.ReadElementContentAsInt();
                                    break;
                                case "FramesPerShot":
                                    shotDelay = reader.ReadElementContentAsInt();
                                    break;
                                case "RespawnRate":
                                    respawnTime = reader.ReadElementContentAsInt();
                                    break;
                                case "HitPoints":
                                    playerHP = reader.ReadElementContentAsInt();
                                    break;
                                case "TankSpeed":
                                    playerSpeed = reader.ReadElementContentAsDouble();
                                    break;
                                case "TankSize":
                                    playerSize = reader.ReadElementContentAsInt();
                                    break;
                                case "WallSize":
                                    wallSize = reader.ReadElementContentAsInt();
                                    break;
                                case "MaxPowerups":
                                    powerupCount = reader.ReadElementContentAsInt();
                                    break;
                                case "MaxPowerupDelay":
                                    maxPowerupSpawnTime = reader.ReadElementContentAsInt();
                                    break;
                                case "Wall":
                                    Wall walltoAdd = new Wall();
                                    reader.ReadToDescendant("p1");
                                    //set x for wall start
                                    reader.ReadToFollowing("x");
                                    tempx = int.Parse(reader.ReadInnerXml());
                                    //set y for wall start
                                    reader.ReadToFollowing("y");
                                    tempy = int.Parse(reader.ReadInnerXml());
                                    //set start location
                                    wallLocation = new Vector2D(tempx, tempy);
                                    walltoAdd.SetStartLocation(wallLocation);
                                    reader.ReadToNextSibling("p2");
                                    //set x for wall end
                                    reader.ReadToFollowing("x");
                                    tempx = int.Parse(reader.ReadInnerXml());
                                    //set y for wall end
                                    reader.ReadToFollowing("y");
                                    tempy = int.Parse(reader.ReadInnerXml());
                                    //set end location
                                    wallLocation = new Vector2D(tempx, tempy);
                                    walltoAdd.SetEndLocation(wallLocation);
                                    walltoAdd.SetID(wallID);
                                    lock (world)
                                    {
                                        world.AddObject(wallID, walltoAdd);
                                    }
                                    wallID++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Could not load file.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("Could not find file path.");
            }
            catch (XmlException)
            {
                throw new XmlException("File data not readable.");
            }
        }

        private void PlayerDisconnect(SocketState state)
        {
            
            lock (world)
            {
                Tank t = world.GetTankByID((int)state.ID);
                t.SwitchDisconnected();
                t.SwitchConnected();
                t.SetHealth(0);
                players.Remove(state.ID);
                playerSockets.Remove(state.ID);
                //world.GetTankByID((int)state.ID).SetHealth(0);
                //world.RemoveTankByID((int)state.ID);
                Console.WriteLine("Player " + state.ID + " has left the game.");
            }
        }

        public void SaveGame()
        {
            lock (world)
            {
                DatabaseModel.SaveGame(10);
                foreach(Tank t in world.GetTanks())
                {
                    DatabaseModel.SaveTank(t);
                }
            }
        }

    }
}
