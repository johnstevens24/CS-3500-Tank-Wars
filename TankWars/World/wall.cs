// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using Newtonsoft.Json;
using System;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty]
        public int wall { get; set; } //wall ID number
        [JsonProperty]
        public Vector2D p1 { get; private set; } //endpoint 1
        [JsonProperty]
        public Vector2D p2 { get; private set; } //endpoint 2 
        private const double Thickness = 50; // The size of a wall.
        double top, left, right, bottom; // The borders of the wall.
        double expansion; // How much from the center of the wall a boundary should be.

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Wall()
        {

        }

        /// <summary>
        /// Constructs a wall object.
        /// </summary>
        /// <param name="p1"> The starting point of the wall. </param>
        /// <param name="p2"> The ending point of the wall. </param>
        /// <param name="ID"> The unique ID of the wall. </param>
        public Wall(Vector2D p1, Vector2D p2, int ID)
        {
            wall = ID;
            this.p1 = p1;
            this.p2 = p2;

            expansion = Thickness / 2;
            left = Math.Min(p1.GetX(), p2.GetX());
            right = Math.Max(p1.GetX(), p2.GetX());
            bottom = Math.Min(p1.GetY(), p2.GetY());
            top = Math.Max(p1.GetY(), p2.GetY());
        }

        /// <summary>
        /// Determines whether a tank has collided with a wall.
        /// Returns true if there was a collision and false otherwise.
        /// </summary>
        /// <param name="tankLoc"> Location of the tank. </param>
        /// <returns></returns>
        public bool CollidesTank(Vector2D tankLoc)
        {
            expansion = Thickness / 2 + Tank.Size / 2;
            return (left - expansion < tankLoc.GetX() && right + expansion > tankLoc.GetX() && bottom - expansion < tankLoc.GetY() && top + expansion > tankLoc.GetY());
        }

        /// <summary>
        /// Determines whether a projectile has collided with a wall.
        /// Returns true if there was a collision and false otherwise.
        /// </summary>
        /// <param name="projLoc"> Loctaion of a projectile. </param>
        /// <returns></returns>
        public bool CollidesProjectile (Vector2D projLoc)
        {
            return (left - 20 < projLoc.GetX() && right + 20 > projLoc.GetX() && bottom - 20 < projLoc.GetY() && top + 20 > projLoc.GetY());
        }
    }
}
