//@Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        [JsonProperty]
        public string moving;
        [JsonProperty]
        public string fire;
        [JsonProperty]
        public Vector2D tdir;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ControlCommand()
        {

        }
    }
}
