using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankWars;
using World;

namespace World
{

    /// <summary>
    /// Class representing entire game world.
    /// </summary>
    public class TheWorld
    {
        //dictionaries to tie objects to specific IDs.
        private Dictionary<int, Tank> Tanks;
        private Dictionary<int, Powerup> Powerups;
        private Dictionary<int, Beam> Beams;
        private Dictionary<int, Wall> Walls;
        private Dictionary<int, Projectile> Projectiles;
        //information the game needs to know about the size of the world and player ID and Location.
        private int _worldSize;
        private int _playerID;
        private Vector2D playerLocation;


        public TheWorld(int worldSize, int playerID)
        {
            _worldSize = worldSize;
            _playerID = playerID;
            Tanks = new Dictionary<int, Tank>();
            Powerups = new Dictionary<int, Powerup>();
            Beams = new Dictionary<int, Beam>();
            Walls = new Dictionary<int, Wall>();
            Projectiles = new Dictionary<int, Projectile>();
            playerLocation = new Vector2D(0, 0);
        }
        /// <summary>
        /// Getter to return the size of the game world.
        /// </summary>
        /// <returns>Game world size</returns>
        public int GetWorldSize()
        {
            return _worldSize;
        }
        /// <summary>
        /// Getter to return ID of the player in current client.
        /// </summary>
        /// <returns>ID of player tank.</returns>
        public int GetPlayerID()
        {
            return _playerID;
        }
        /// <summary>
        /// Method to add objects in dictionaries used to draw the game world.
        /// </summary>
        /// <param name="ID">unique ID of the object to be added.</param>
        /// <param name="o">object to be added</param>
        public void AddObject(int ID, Object o)
        {
            switch (o.GetType().ToString())
            {
                case "World.Wall":
                    Walls[ID] = (Wall)o;
                    break;
                case "World.Tank":
                    if (ID == _playerID)
                    {
                        playerLocation = ((Tank)o).GetLocation();
                    }
                    Tanks[ID] = (Tank)o;
                    //if (((Tank) o).IsDead() || ((Tank)o).GetHealth() < 1)
                    //{
                    //    Tanks.Remove(ID);
                    //}
                    break;
                case "World.Powerup":
                    Powerups[ID] = ((Powerup)o);
                    if (((Powerup)o).IsDead()) Powerups.Remove(ID);
                    break;
                case "World.Beam":
                    Beams[ID] = (Beam)o;
                    break;
                case "World.Projectile":
                    Projectiles[ID] = (Projectile)o;
                    if (((Projectile)o).IsDead()) Projectiles.Remove(ID);
                    break;
                default: return;
            }
        }
        /// <summary>
        /// Method to clear all dictionaries. Used to prevent oversized dictionaries.
        /// </summary>
        public void RemoveAll()
        {
            Beams.Clear();
            Powerups.Clear();
            Tanks.Clear();
            Projectiles.Clear();
        }

        /// <summary>
        /// Method to return all tanks that the world currently has.
        /// </summary>
        /// <returns>All tanks in the current world.</returns>
        public IEnumerable<object> GetTanks()
        {
            IEnumerable<object> query = Tanks.Values;
            return query;
        }
        /// <summary>
        /// Method to return all walls that the world currently has.
        /// </summary>
        /// <returns>All walls in the current world.</returns>
        public IEnumerable<object> GetWalls()
        {
            IEnumerable<object> query = Walls.Values;
            return query;
        }
        /// <summary>
        /// Method to return all powerups that the world currently has.
        /// </summary>
        /// <returns>All powerups in the current world.</returns>
        public IEnumerable<object> GetPowerups()
        {
            IEnumerable<object> query = Powerups.Values;
            return query;
        }
        /// <summary>
        /// Method to return all beams that the world currently has.
        /// </summary>
        /// <returns>All beams in the current world.</returns>
        public IEnumerable<Beam> GetBeams()
        {
            IEnumerable<Beam> query = Beams.Values;
            return query;
        }
        /// <summary>
        /// Method to return all projectiles that the world currently has.
        /// </summary>
        /// <returns>All projectiles in the current world.</returns>
        public IEnumerable<object> GetProjectiles()
        {
            IEnumerable<object> query = Projectiles.Values;
            return query;
        }
        /// <summary>
        /// Method to return the location of a specific tank.
        /// </summary>
        /// <returns>The location of a specific tank.</returns>
        public Vector2D GetTankLocation(int ID)
        {
            if (Tanks.ContainsKey(ID))
            {
                return Tanks[ID].GetLocation();
            }
            return playerLocation;
        }

        public void RemoveBeam(long ID)
        {
            Beams.Remove((int)ID);
        }
        public void RemoveProjectile(long ID)
        {
            Projectiles.Remove((int)ID);
        }
        public void RemovePowerup(long ID)
        {
            Powerups.Remove((int)ID);
        }
        public Tank GetTankByID(int ID)
        {
            return Tanks[ID];
        }
        public bool TankExists(int ID)
        {
            return Tanks.ContainsKey(ID);
        }
        public void RemoveTankByID(int ID)
        {
            if (!Tanks.ContainsKey(ID))
            {
                return;
            }
            Tanks.Remove(ID);
        }
    }

}
