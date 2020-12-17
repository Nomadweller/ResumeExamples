using System;
using Newtonsoft.Json;
using TankWars;

namespace World
{
    /// <summary>
    /// Class representing collectable powerups.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        //Unique powerup ID
        [JsonProperty(PropertyName = "power")]
        private int ID;
        //The location of the powerup
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;
        //Boolean representing if the powerup exists
        [JsonProperty(PropertyName = "died")]
        private bool died = true;

        /// <summary>
        /// Getter to return the unique powerupID.
        /// </summary>
        /// <returns>Unique Powerup ID</returns>
        public int GetID()
        {
            return ID;
        }
        public void SetID(int ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// Getter to return location of powerup.
        /// </summary>
        /// <returns>Powerup Location</returns>
        public Vector2D GetLocation()
        {
            return location;
        }
        public void SetLocation(Vector2D location)
        {
            this.location = location;
        }
        /// <summary>
        /// Getter to return if powerup exists.
        /// </summary>
        /// <returns>Powerup's existance.</returns>
        public bool IsDead()
        {
            return died;
        }
        public void SwitchDead()
        {
            if (died)
                died = false;
            else
                died = true;
        }
    }
}