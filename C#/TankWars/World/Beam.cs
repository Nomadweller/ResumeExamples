using System;
using Newtonsoft.Json;
using TankWars;

namespace World
{
    /// <summary>
    /// Class representing special attack beams.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        //Beam's unique ID
        [JsonProperty(PropertyName = "beam")]
        private int ID;
        //The vector that the beam originates on
        [JsonProperty(PropertyName = "org")]
        private Vector2D origin;
        //The vector relative angle the beam is shot at
        [JsonProperty(PropertyName = "dir")]
        private Vector2D direction;
        //The tank that shot the beam
        [JsonProperty(PropertyName = "owner")]
        private int owner;

        /// <summary>
        /// Getter for the unique beam ID.
        /// </summary>
        /// <returns>The unique beam ID.</returns>
        public int GetID()
        {
            return ID;
        }
        public void SetID(int ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// A getter for the origin of the beam.
        /// </summary>
        /// <returns>Beam origin.</returns>
        public Vector2D GetLocation()
        {
            return origin;
        }
        public void SetLocation(Vector2D origin)
        {
            this.origin = origin;
        }
        /// <summary>
        /// Getter for the vector relative angle.
        /// </summary>
        /// <returns>Vector to calculate angle.</returns>
        public Vector2D GetDirection()
        {
            return direction;
        }
        public void SetDirection(Vector2D direction)
        {
            this.direction = direction;
        }
        /// <summary>
        /// A getter for the owner of the beam.
        /// </summary>
        /// <returns>Beam owner.</returns>
        public int GetOwner()
        {
            return owner;
        }
        public void SetOwner(int owner)
        {
            this.owner = owner;
        }
    }
}