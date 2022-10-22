// @Author Alyssa Johnson, John Stevens, and Daniel Kopta
// CS 3500, Fall 2021

using Newtonsoft.Json;
using System;

namespace TankWars
{
    //DON'T CHANGE ANY MEMBER VARIABLE NAMES

    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty]
        public int tank { get; set; } // tank ID value
        [JsonProperty]
        public String name { get; set; } // Player's name
        [JsonProperty]
        public Vector2D loc { get; set; } // location of tank
        [JsonProperty]
        public Vector2D bdir { get; set; } // Tank's orientation
        [JsonProperty]
        public Vector2D tdir { get; set; } // Turret orientation
        [JsonProperty]
        public int score { get; set; } // Player's score
        [JsonProperty]
        public int hp { get; set; } // Tanks health
        [JsonProperty]
        public bool died { get; set; } // indicates if the tank died on that frame
        [JsonProperty]
        public bool dc { get; set; } // indicates if the player disconnected on that frame
        [JsonProperty]
        public bool join { get; set; } // indicates if the player joined on this frame
        public Vector2D Velocity { get; internal set; } // The velocity of the tank.
        public const double EnginePower = 3; // The speed of the tank.
        public const int Size = 60; // The length of the tank
        public int projFrames { get; set; } // A counter for how many frames have passed since a projectile was fired.
        public int respawnCounter { get; set; } //Counts down the number of frames until the tank can respawn
        public int powerupCount { get; set; } //The number of powerups the tank currently has

        /// <summary>
        /// The default contructor for a tank.
        /// </summary>
        public Tank()
        {
        }

        /// <summary>
        /// Constructor used when the server creates a new tank
        /// </summary>
        /// <param name="ID">Tank's ID</param>
        /// <param name="name">Client's name</param>
        public Tank(int ID, String name)
        {
            tank = ID;
            this.name = name;
            this.loc = new Vector2D(0, 0);
            this.bdir = new Vector2D(0, 1);
            this.tdir = new Vector2D(0, 1);
            score = 0;
            hp = 3;
            died = false;
            dc = false;
            join = true;
            Velocity = new Vector2D(0, 0);
            projFrames = 0;
            respawnCounter = 0;
            powerupCount = 0;
        }

        /// <summary>
        /// Checks to see if a projectile collides with a tank. Used exclusively by the server
        /// </summary>
        /// <param name="projLoc">location of the projectile</param>
        /// <returns></returns>
        public bool CollidesWithTank(Vector2D projLoc)
        {
            double expansion = Size / 2;
            return ((projLoc.GetX() > loc.GetX()-expansion) && (projLoc.GetX() < loc.GetX() + expansion) && (projLoc.GetY() > loc.GetY() - expansion) && (projLoc.GetY() < loc.GetY() + expansion));
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty]
        public int beam; //ID number
        [JsonProperty]
        public Vector2D org; //origin of beam
        [JsonProperty]
        public Vector2D dir; //direction of beam
        [JsonProperty]
        public int owner; //ID of tank who fired the beam
        public int frameCounter = 30; //The duration of frames a beam should last as specified by us.

        /// <summary>
        /// Default constructor for a beam.
        /// </summary>
        public Beam()
        {
        }

        /// <summary>
        /// Contructs a Beam object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        public Beam(int id, Tank t)
        {
            beam = id;
            org = t.loc;
            dir = t.tdir;
            owner = t.tank;
        }

        /// <summary>
        /// @Author Daniel Kopta
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

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
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty]
        public int proj; //ID number
        [JsonProperty]
        public Vector2D loc; //location of projectile
        [JsonProperty]
        public Vector2D dir; //orientation of projectile
        [JsonProperty]
        public bool died; //represents if the projectile died on this frame (hit something/left world bounds)
        [JsonProperty]
        public int owner; //ID number of the tank who shot the projectile
        public Vector2D velocity { get; set; } // The velocity of the projectile
        public const double speed = 25; // The speed of the projectile
        public const int Size = 30; // The size of the projectile.

        /// <summary>
        /// Default constructor for a projectile.
        /// </summary>
        public Projectile()
        {
        }

        /// <summary>
        /// Constructs a Projectile object.
        /// </summary>
        /// <param name="id"> Unique ID of the projectile. </param>
        /// <param name="t"> The tank the projectile originates from. </param>
        public Projectile(int id, Tank t)
        {
            proj = id;
            loc = t.loc;
            dir = t.tdir;
            died = false;
            owner = t.tank;
            velocity = t.tdir * speed;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        [JsonProperty]
        public int power; // an int representing the powerup's unique ID.
        [JsonProperty]
        public Vector2D loc; // a Vector2D representing the location of the powerup.
        [JsonProperty]
        public bool died; // a bool indicating if the powerup "died" (was collected by a player) on this frame
        
        /// <summary>
        /// Constructs a powerup object.
        /// </summary>
        public Powerup()
        {
        }

        /// <summary>
        /// Constructs a powerup object. Used by the server
        /// </summary>
        public Powerup(int ID, Vector2D loc)
        {
            power = ID;
            this.loc = loc;
            died = false;
        }

        /// <summary>
        /// Checks to see if the location passed in collides with the current powerups location. Stops powerups from spawning on top of each other.
        /// </summary>
        /// <param name="powerLoc"></param>
        /// <returns></returns>
        public bool CollidesWithPowerups(Vector2D powerLoc)
        {
            double expansion = 15;
            return ((powerLoc.GetX() > loc.GetX() - expansion) && (powerLoc.GetX() < loc.GetX() + expansion) && (powerLoc.GetY() > loc.GetY() - expansion) && (powerLoc.GetY() < loc.GetY() + expansion));
        }

    }

    /// <summary>
    /// This class is exclusively used by our client to help manage drawing the death animations
    /// </summary>
    public class TankDeath
    {
        public int frameCounter { get; set; } // The number of frames the animation should last as specified by us.
        public int id { get; set; } // The ID of the Tank that died
        public double x { get; set; } // The x-coordinate of the tank
        public double y { get; set; } // The y-coordinate of the tank

        /// <summary>
        /// Constructs a deadTank.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TankDeath(int id, double x, double y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            frameCounter = 32;
        }
    }
}
