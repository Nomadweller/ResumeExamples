using System;
using Newtonsoft.Json;
using TankWars;

namespace World
{
    /// <summary>
    /// Class represent player/AI controlled tanks
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        public int beamCount { get; set; }
        public int projectileDelay { get; set; }
        //unique ID of tank
        [JsonProperty(PropertyName = "tank")]
        private int ID;
        //location of tank
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;
        //direction tank is facing
        [JsonProperty(PropertyName = "bdir")]
        private Vector2D orientation;
        //direction turret is facing.
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming;
        //name of the player controlling tank
        [JsonProperty(PropertyName = "name")]
        private string name;
        //health of tank
        [JsonProperty(PropertyName = "hp")]
        private int hitPoints = 3;
        //amount of tanks this one has destroyed.
        [JsonProperty(PropertyName = "score")]
        private int score = 0;
        //Tank's existence
        [JsonProperty(PropertyName = "died")]
        private bool died = true;
        //Has tank disconnected
        [JsonProperty(PropertyName = "dc")]
        private bool disconnected = true;
        //Did tank join
        [JsonProperty(PropertyName = "join")]
        public bool joined = false;

        /// <summary>
        /// Getter to return tank's unique id.
        /// </summary>
        /// <returns>tank's unique id.</returns>
        public int GetID()
        {
            return ID;
        }
        public void SetID(int ID)
        {
            this.ID = ID;
        }
        /// <summary>
        /// Getter to return tank's location.
        /// </summary>
        /// <returns>tank's location.</returns>
        public Vector2D GetLocation()
        {
            return location;
        }
        public void SetLocation(Vector2D location)
        {
            this.location = location;
        }
        //
        /// <summary>
        /// Getter to return tank's orientation.
        /// </summary>
        /// <returns>tank's orientation.</returns>
        public Vector2D GetOrientation()
        {
            return orientation;
        }
        public void SetOrientation(Vector2D orientation)
        {
            this.orientation = orientation;
        }
        /// <summary>
        /// Getter to return turret's orientation.
        /// </summary>
        /// <returns>turret's orientation.</returns>
        public Vector2D GetAim()
        {
            if (aiming == null)
            {
                return new Vector2D(0, 0);
            }
            return aiming;
        }
        public void SetAim(Vector2D aiming)
        {
            this.aiming = aiming;
        }
        /// <summary>
        /// Getter to return tank's player's name.
        /// </summary>
        /// <returns>tank's player's name.</returns>
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            if (name.EndsWith("\n"))
            {
                name = name.Substring(0, name.Length - 1);
            }
            this.name = name;
        }
        /// <summary>
        /// Getter to return tank's health.
        /// </summary>
        /// <returns>tank's health.</returns>
        public int GetHealth()
        {
            return hitPoints;
        }
        public void SetHealth(int _hitPoints)
        {
            this.hitPoints = _hitPoints;
            if(hitPoints == 0)
            {
                died = true;
            }
        }
        /// <summary>
        /// Getter to return tank's score.
        /// </summary>
        /// <returns>tank's score.</returns>
        public int GetScore()
        {
            return score;
        }
        public void SetScore(int score)
        {
            this.score = score;
        }
        /// <summary>
        /// Getter to return tank's existence.
        /// </summary>
        /// <returns>tank's existence.</returns>
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
        /// Getter to return if player disconnects.
        /// </summary>
        /// <returns>Represents a disconnected player.</returns>
        public bool IsDisconnected()
        {
            return disconnected;
        }
        public void SwitchDisconnected()
        {
            if (disconnected)
                disconnected = false;
            else
                disconnected = true;
        }
        /// <summary>
        /// Getter to return if player connects.
        /// </summary>
        /// <returns>Represents a connected player.</returns>
        public bool IsConnected()
        {
            return joined;
        }
        public void SwitchConnected()
        {
            if (joined)
                joined = false;
            else
                joined = true;
        }

    }
}