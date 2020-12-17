using System;
using Newtonsoft.Json;
using TankWars;

namespace World
{
    /// <summary>
    /// Class representing projectiles fired by a player
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        //Projectile unique ID
        [JsonProperty(PropertyName = "proj")]
        private int ID;
        //Vector showing location of projectile
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;
        //Vector relative angle of projectile
        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;
        //Boolean representing if projectile exists.
        [JsonProperty(PropertyName = "died")]
        private bool died = true;
        //Tank ID that fired this projectile.
        [JsonProperty(PropertyName = "owner")]
        private int ownerID;

        /// <summary>
        /// Getter to return the unique projectile ID.
        /// </summary>
        /// <returns>The unique projectile ID.returns>
        public int GetID()
        {
            return ID;
        }

        public void SetID(int ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// Getter to return projectile location
        /// </summary>
        /// <returns>Location of projectile</returns>
        public Vector2D GetLocation()
        {
            return location;
        }
        public void SetLocation(Vector2D location)
        {
            this.location = location;
        }
        /// <summary>
        /// Getter to return vector relative angle of this projectile.
        /// </summary>
        /// <returns>vector relative angle of projectile.</returns>
        public Vector2D GetOrientation()
        {
            return orientation;
        }
        public void SetOrientation(Vector2D orientation)
        {
            this.orientation = orientation;
        }
        /// <summary>
        /// Getter to return whether the projectile exists or not.
        /// </summary>
        /// <returns>Projectile existance.</returns>
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
        /// <summary>
        /// Getter to return owner of projectile
        /// </summary>
        /// <returns>ID of the owner of this projectile</returns>
        public int GetOwnerID()
        {
            return ownerID;
        }
        public void SetOwnerID(int ownerId)
        {
            this.ownerID = ownerId;
        }
    }
}
