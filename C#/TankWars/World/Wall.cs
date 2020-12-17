using System;
using Newtonsoft.Json;
using TankWars;

namespace World
{
    /// <summary>
    /// Class representing game walls.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        //unique wall ID
        [JsonProperty(PropertyName = "wall")]
        private int ID;
        //Wall starting location
        [JsonProperty(PropertyName = "p1")]
        private Vector2D start;
        //Wall ending location
        [JsonProperty(PropertyName = "p2")]
        private Vector2D end;

        //public Wall(int _ID, Vector2D _start, Vector2D _end)
        //{
        //    ID = _ID;

        //}

        /// <summary>
        /// Getter for the unique wall ID.
        /// </summary>
        /// <returns>The unique wall ID.</returns>
        public int GetID()
        {
            return ID;
        }
        public void SetID(int ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// Getter to return wall start
        /// </summary>
        /// <returns>Wall starting position</returns>
        public Vector2D GetStartLocation()
        {
            return start;
        }
        public void SetStartLocation(Vector2D start)
        {
            this.start = start;
        }
        /// <summary>
        /// Getter to return wall end
        /// </summary>
        /// <returns>Wall ending position</returns>
        public Vector2D GetEndLocation()
        {
            return end;
        }
        public void SetEndLocation(Vector2D end)
        {
            this.end = end;
        }
    }
}