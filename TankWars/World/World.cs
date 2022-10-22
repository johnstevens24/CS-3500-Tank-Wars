// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System;
using System.Collections.Generic;

namespace TankWars
{
    public class world
    {
        // A Dictionary that keeps track of walls in the game.
        public Dictionary<int, Wall> Walls { get; } = new Dictionary<int, Wall>();

        // A Dictionary that keeps track of tanks in the game.
        public Dictionary<int, Tank> Tanks { get; set; } = new Dictionary<int, Tank>();

        // A Dictionary that keeps track of Beams in the game.
        public Dictionary<int, Beam> Beams { get; set; } = new Dictionary<int, Beam>();

        // A Dictionary that keeps track of projectiles in the game.
        public Dictionary<int, Projectile> Projectiles { get; set; } = new Dictionary<int, Projectile>();

        // A Dictionary that keeps track of powerups in the game.
        public Dictionary<int, Powerup> Powerups { get; set; } = new Dictionary<int, Powerup>();

        // A Dictionary that keeps track of dead tanks in the game.
        public Dictionary<int, TankDeath> TankDeaths { get; set; } = new Dictionary<int, TankDeath>();

        // A Dictionary that keeps track of the control commands received.
        public Dictionary<int, ControlCommand> CtrlCmds = new Dictionary<int, ControlCommand>();

        // The length of the world, which is a square.
        public int worldSize { get; set; }

        // The ID of a projectile in the world.
        // Allows the projectile to have a unique ID by incrementing each time a projectile is created.
        private int projectileID { get; set; }

        // The ID of a beam in the world.
        // Allows the beam to have a unique ID by incrementing each time a beam is created.
        private int beamID { get; set; }
        public int FramesPerShot { get; set; } // the number of frames that must pass before a client can shoot again
        private int PowerupFrameCounter { get; set; } //the number of frames that must pass before a powerup can respawn. Randomly generated
        private int PowerupIDCounter { get; set; } // Allows the powerup to have a unique ID by incrementing each time a powerup is created.
        public int RespawnRate { get; set; } //the number of frames that must pass before a tank can respawn

        /// <summary>
        /// Default Constructor for a world object.
        /// </summary>
        public world()
        {
        }


        /// <summary>
        /// Constructs a World object.
        /// </summary>
        /// <param name="size"></param>
        public world(int size)
        {
            worldSize = size;
            projectileID = 0;
            beamID = 0;
            PowerupFrameCounter = 0;
            PowerupIDCounter = 0;
        }

        //==========================FROM HERE DOWN IS EXCLUSIVELY SERVER METHODS=========================

        /// <summary>
        /// Goes through ControlCommands received from clients and processes their requests to move, 
        /// fire, and change their turret direction.
        /// </summary>
        public void Update()
        {
            lock (CtrlCmds)
            {
                foreach (KeyValuePair<int, ControlCommand> ctrlCmd in CtrlCmds)
                {
                    // controlcommands share the same ID as their corresponding tank.
                    Tank tank = Tanks[ctrlCmd.Key];
                    tank.tdir = ctrlCmd.Value.tdir;

                    // Determines if client would like to fire a projectile, beam, or nothing.
                    switch (ctrlCmd.Value.fire)
                    {
                        // main means projectile
                        case "main":
                            // Only allows a projectile to be created every 45 frames. Resets projectile frame counter
                            // to zero so that the client will have to wait another 45 frames before firing again.
                            if (tank.projFrames >= FramesPerShot)
                            {
                                Projectile p = new Projectile(projectileID, tank);
                                projectileID++;
                                Projectiles.Add(p.proj, p);
                                tank.projFrames = 0;
                            }
                            // Increments projectile frame counter since a frame has passed.
                            else
                            {
                                tank.projFrames++;
                            }
                            break;

                        // alt means beam
                        case "alt":
                            if (tank.powerupCount > 0)
                            {
                                Beam b = new Beam(beamID, tank);
                                beamID++;
                                Beams.Add(b.beam, b);
                                tank.powerupCount--;
                            }
                            break;

                        default:
                            // Increments projectile frame counter since a frame has passed.
                            tank.projFrames++;
                            break;
                    }

                    // Handles the movement request of the client tank.
                    switch (ctrlCmd.Value.moving)
                    {
                        case "up":
                            tank.Velocity = new Vector2D(0, -1);
                            tank.bdir = new Vector2D(0, -1);
                            break;

                        case "down":
                            tank.Velocity = new Vector2D(0, 1);
                            tank.bdir = new Vector2D(0, 1);
                            break;

                        case "left":
                            tank.Velocity = new Vector2D(-1, 0);
                            tank.bdir = new Vector2D(-1, 0);
                            break;

                        case "right":
                            tank.Velocity = new Vector2D(1, 0);
                            tank.bdir = new Vector2D(1, 0);
                            break;

                        default:
                            tank.Velocity = new Vector2D(0, 0);
                            break;
                    }
                    tank.Velocity *= Tank.EnginePower;
                }

                // Empties the ControlCommands dictionary so that only new messages will be included on the next call to this update method.
                CtrlCmds.Clear();
            }

            // Updates the projectile's location if it does not hit a wall
            foreach (Projectile proj in Projectiles.Values)
            {
                Vector2D newLoc = proj.loc + proj.velocity;
                bool collision = false;

                foreach (Wall wall in Walls.Values)
                {
                    // Does not allow the projectile's location to be updated if there was a collision.
                    if (wall.CollidesProjectile(newLoc))
                    {
                        collision = true;
                        proj.died = true;
                        break;
                    }
                    // Kills the projectile if it goes out of the world
                    if (newLoc.GetX() < -worldSize / 2 || newLoc.GetX() > worldSize / 2 || newLoc.GetY() < -worldSize / 2 || newLoc.GetY() > worldSize / 2)
                    {
                        collision = true;
                        proj.died = true;
                        break;
                    }
                    // If there was no collision, the projectile's position is allowed to be updated.
                    if (!collision)
                    {
                        proj.loc = newLoc;
                    }
                }

                foreach (Tank tank in Tanks.Values)
                {
                    //prevents dead (invisible) tanks from stopping projectiles
                    if (tank.hp != 0)
                        if (tank.CollidesWithTank(proj.loc) && tank.tank != proj.owner && proj.died == false)

                            {
                                tank.hp--;
                            if (tank.hp == 0)
                            {
                                tank.died = true;
                                Tanks[proj.owner].score++;
                            }
                            proj.died = true;
                        }
                }

            }

            // Kills Tanks if a beam hits them.
            foreach (Beam b in Beams.Values)
            {
                foreach (Tank tank in Tanks.Values)
                {
                    if (Beam.Intersects(b.org, b.dir, tank.loc, (double)Tank.Size / 2) && tank.tank != b.owner)
                    {
                        tank.died = true;
                        tank.hp = 0;
                        Tanks[b.owner].score++;
                    }
                }
            }

            // Updates the tank's location if it does not hit a wall
            foreach (Tank tank in Tanks.Values)
            {
                // Doesn't do anything if the tank is not moving
                if (tank.Velocity.Length() == 0)
                    continue;

                Vector2D newLoc = tank.loc + tank.Velocity;
                bool collision = false;

                // Checks to see if a tank should wraparound if it hit the border of the world
                // if tank hits left border, new location is right border - 3
                if (newLoc.GetX() < -worldSize / 2)
                {
                    newLoc = new Vector2D(-tank.loc.GetX() - 3, tank.loc.GetY());
                }
                // if tank hits right border, new location is left border + 3
                else if (newLoc.GetX() > worldSize / 2)
                {
                    newLoc = new Vector2D(-tank.loc.GetX() + 3, tank.loc.GetY());
                }
                // if tank hits top border, new location is bottom border - 3
                else if (newLoc.GetY() < -worldSize / 2)
                {
                    newLoc = new Vector2D(tank.loc.GetX(), -tank.loc.GetY() - 3);
                }
                // if tank hits bottom border, new location is top border + 3
                else if (newLoc.GetY() > worldSize / 2)
                {
                    newLoc = new Vector2D(tank.loc.GetX(), -tank.loc.GetY() + 3);
                }

                // Does not allow the tank's location to be updated if there was a collision.
                foreach (Wall wall in Walls.Values)
                {
                    if (wall.CollidesTank(newLoc))
                    {
                        collision = true;
                        tank.Velocity = new Vector2D(0, 0);
                        break;
                    }
                }

                // If there was no collision, the tank's position is allowed to be updated.
                if (!collision)
                {
                    tank.loc = newLoc;
                }

                // Checks to see if the tank collides with any powerups this frame
                foreach (Powerup power in Powerups.Values)
                {
                    if (tank.CollidesWithTank(power.loc))
                    {
                        power.died = true;
                        tank.powerupCount++;
                    }
                }
            }

            // There should never be more than two powerups in existence
            if (Powerups.Count < 2)
            {
                PowerupFrameCounter--;
            }
            if (PowerupFrameCounter <= 0)
            {
                Powerups.Add(PowerupIDCounter, new Powerup(PowerupIDCounter, GetRandomSpawnPoint()));
                PowerupIDCounter++;
                PowerupFrameCounter = RandomPowerupDelay();
            }

        }

        /// <summary>
        /// Checks to see what objects died during that frame and removes them from the world
        /// </summary>
        public void RemoveDeadObjects()
        {
            // Clears beams because they're only ever sent on the frame where they're created
            Beams.Clear();

            foreach (KeyValuePair<int, Projectile> p in Projectiles)
            {
                if (p.Value.died)
                    Projectiles.Remove(p.Key);
            }
            foreach (KeyValuePair<int, Tank> t in Tanks)
            {
                if (t.Value.died)
                {
                    t.Value.respawnCounter = RespawnRate;
                }
                if (t.Value.dc)
                {
                    Tanks.Remove(t.Key);
                }
            }
            foreach (KeyValuePair<int, Powerup> p in Powerups)
            {
                if (p.Value.died)
                {
                    Powerups.Remove(p.Key);
                }
            }
        }

        /// <summary>
        /// Generates a random number between 1 and 1650
        /// </summary>
        /// <returns></returns>
        private int RandomPowerupDelay()
        {
            Random r = new Random();
            return r.Next(1, 1650);
        }

        /// <summary>
        /// Generates a random spawnpoint for new tanks that doesn't collide with anything (wall, tanks, powerups)
        /// </summary>
        /// <returns></returns>
        public Vector2D GetRandomSpawnPoint()
        {
            Random r = new Random();
            Vector2D spawnPoint;
            bool collision = false;
            do
            {
                //reset it to false every time
                collision = false;
                //the +/- 50 is so that we can avoid a lot of points that we know will be in walls
                spawnPoint = new Vector2D(r.Next(-worldSize / 2 + 50, worldSize / 2 - 50), r.Next(-worldSize / 2 + 50, worldSize / 2 - 50));

                //prevents from spawning in walls
                foreach (Wall w in Walls.Values)
                {
                    if (w.CollidesTank(spawnPoint))
                    {
                        collision = true;
                        continue;
                    }

                }
                //prevents from spawning on top of other tanks
                foreach (Tank t in Tanks.Values)
                {
                    if (t.CollidesWithTank(spawnPoint))
                    {
                        collision = true;
                        continue;
                    }
                }

                //prevents from spawning on top of other powerups
                foreach (Powerup p in Powerups.Values)
                {
                    if (p.CollidesWithPowerups(spawnPoint))
                    {
                        collision = true;
                        continue;
                    }
                }

            }
            while (collision == true);

            return spawnPoint;
        }

    }
}
