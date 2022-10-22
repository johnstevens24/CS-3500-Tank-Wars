// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System.Drawing;
using System.Windows.Forms;
using TankWars;
using System.Collections;

namespace View
{
    public class DrawingPanel : Panel
    {
        // The World that keeps track of tanks, walls, and other world features.
        private world theWorld;
        // Array of paths to all 8 tank images
        private Image[] tankImgs;
        // Array of paths to all 8 turret images
        private Image[] turretImgs;
        // Array of paths to all 8 shot images
        private Image[] shotImgs;
        // Array of paths to all 8 tank explosion images
        private Image[] explosionImgs;
        // Array of paths to 3 beam images
        private Color[] beamColors;
        // Powerup image
        private Image powerupImg;
        // Wall image
        private Image wallImg;
        // Background image
        private Image backgroundImg;
        // The player's x-coordinate location
        private double playerX;
        // The player;s y-coordinate location
        private double playerY;


        /// <summary>
        /// Constructor for a DrawingPanel.
        /// </summary>
        /// <param name="w"> The World object. </param>
        public DrawingPanel(world w)
        {
            DoubleBuffered = true;
            theWorld = w;
            playerX = 0;
            playerY = 0;
            LoadImages();
        }


        /// <summary>
        /// Loads all the images needed for the game.
        /// </summary>
        private void LoadImages()
        {
            tankImgs = new Image[] {
            Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\RedTank.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png")
            };

            turretImgs = new Image[] {
            Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\RedTurret.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTurret.png")
            };

            shotImgs = new Image[] {
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-blue.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-grey.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-white.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-brown.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-violet.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-red.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\shot-yellow.png")
            };

            explosionImgs = new Image[] {
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp1.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp2.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp3.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp4.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp5.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp6.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp7.png"),
            Image.FromFile("..\\..\\..\\Resources\\Images\\exp8.png")
            };

            beamColors = new Color[] { Color.DodgerBlue, Color.Purple,
                Color.Green, Color.LightGreen, Color.Orange, Color.Violet,
                Color.Red, Color.Yellow };

            powerupImg = Image.FromFile("..\\..\\..\\Resources\\Images\\PowerUp.png");
            wallImg = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");
            backgroundImg = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");
        }


        /// <summary>
        /// Sets the x-coordinate of the player.
        /// </summary>
        /// <param name="x"></param>
        public void SetX(double x)
        {
            playerX = x;
        }


        /// <summary>
        /// Sets the y-coordinate of the player.
        /// </summary>
        /// <param name="y"></param>
        public void SetY(double y)
        {
            playerY = y;
        }


        /// <summary>
        /// A delegate for DrawObjectWithTransform
        /// Methods matching this delegate can draw whatever they want using e  
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            int width = 50;
            int height = 50;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)

            e.Graphics.DrawImage(wallImg, -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int width = 60;
            int height = 60;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(tankImgs[t.tank % 8], -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int width = 50;
            int height = 50;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(turretImgs[t.tank % 8], -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void NameTagDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            string text = t.name + ": " + t.score;

            // Centers the string text
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            e.Graphics.DrawString(text, new Font(SystemFonts.DefaultFont.FontFamily, 12, FontStyle.Regular), Brushes.White, 0, 30, stringFormat);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void HealthBarDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            Rectangle healthbar = new Rectangle(-21, -40, 41, 5);

            int hp = t.hp;
            Brush b = null;

            if (hp == 3)
                b = Brushes.Green;
            else if (hp == 2)
            {
                b = Brushes.Yellow;
                healthbar.Width = 41 * 2 / 3;
            }
            else if (hp == 1)
            {
                b = Brushes.Red;
                healthbar.Width = 41 * 1 / 3;
            }

            if (hp != 0)
                e.Graphics.FillRectangle(b, healthbar);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;

            int width = 30;
            int height = 30;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(shotImgs[p.owner % 8], -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            int width = 30;
            int height = 30;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(powerupImg, -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Starting point is the tank, ending point is the world size so that beam always covers full map.
            Point[] points = { new Point(0, 0), new Point(0, theWorld.worldSize) };
            
            if (b.frameCounter >= 20)
            {
                e.Graphics.DrawLines(new Pen(beamColors[b.owner % 8], 7), points);
                e.Graphics.DrawLines(new Pen(Color.WhiteSmoke, 3), points);
            }
            else if (b.frameCounter >= 10)
            {
                e.Graphics.DrawLines(new Pen(beamColors[b.owner % 8], 5), points);
                e.Graphics.DrawLines(new Pen(Color.WhiteSmoke, 1), points);
            }
            else if (b.frameCounter >= 0)
            {
                e.Graphics.DrawLines(new Pen(beamColors[b.owner % 8], 1), points);
            }
        }



        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void DeathAnimation(object o, PaintEventArgs e)
        {
            TankDeath td = o as TankDeath;
            int width = 100;
            int height = 100;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Depending on how many frames have passed, a certain explosion image is selected in order to animate an explosion
            if (td.frameCounter >= 28)
                e.Graphics.DrawImage(explosionImgs[0], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 24)
                e.Graphics.DrawImage(explosionImgs[1], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 20)
                e.Graphics.DrawImage(explosionImgs[2], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 16)
                e.Graphics.DrawImage(explosionImgs[3], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 12)
                e.Graphics.DrawImage(explosionImgs[4], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 8)
                e.Graphics.DrawImage(explosionImgs[5], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 4)
                e.Graphics.DrawImage(explosionImgs[6], -(width / 2), -(height / 2), width, height);
            else if (td.frameCounter >= 0)
                e.Graphics.DrawImage(explosionImgs[7], -(width / 2), -(height / 2), width, height);
        }


        /// <summary>
        /// Draws the background image to the screen
        /// </summary>
        /// <param name="e"></param>
        private void BackgroundDrawer(PaintEventArgs e)
        {
            // Length of world
            int length = theWorld.worldSize;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // The world is a square
            e.Graphics.DrawImage(backgroundImg, -(length / 2), -(length / 2), length, length);
        }


        /// <summary>
        /// This method is invoked when the DrawingPanel needs to be re-drawn
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Center the view on the middle of the world,
            // since the image and world use different coordinate systems
            e.Graphics.TranslateTransform(-(float)playerX + (450), -(float)playerY + (450));

            // Draw the players
            lock (theWorld)
            {
                // Draw background
                BackgroundDrawer(e);

                // Draw walls
                foreach (Wall wall in theWorld.Walls.Values)
                {
                    double p1x = wall.p1.GetX();
                    double p1y = wall.p1.GetY();
                    double p2x = wall.p2.GetX();
                    double p2y = wall.p2.GetY();

                    // Draws walls 50 pixels apart until the starting x-point and ending x-point are nearly the same.
                    // We subtract the two x-coordinates from each other to find how much distance is between them.
                    // The tolerance is 0.000001 because comparing doubles for equality doesn't always work due to floating point precision.
                    // This tolerance prevents an infinite loop when the two x-values are nearly the same.

                    // If the starting wall's x-coordinate is left of the ending wall's x-coordinate,
                    // continues drawing walls in the right direction.
                    while (p2x - p1x >= 0.000001)
                    {
                        DrawObjectWithTransform(e, wall, p1x, p1y, 0, WallDrawer);
                        // Adds 50 so that next wall is drawn 50 units apart
                        p1x += 50;
                    }

                    // If the starting wall's x-coordinate is right of the ending wall's x-coordinate,
                    // continues drawing walls in the left direction.
                    while (p1x - p2x >= 0.000001)
                    {
                        DrawObjectWithTransform(e, wall, p2x, p2y, 0, WallDrawer);
                        // Adds 50 so that next wall is drawn 50 units apart
                        p2x += 50;
                    }

                    // Repeats this same process if the y-coordinates are different.
                    // If the starting wall's y-coordinate is below the ending wall's y-coordinate,
                    // continues drawing walls in the upward direction.
                    while (p2y - p1y >= 0.000001)
                    {
                        DrawObjectWithTransform(e, wall, p1x, p1y, 0, WallDrawer);
                        // Adds 50 so that next wall is drawn 50 units apart
                        p1y += 50;
                    }

                    // If the starting wall's y-coordinate is above the ending wall's y-coordinate,
                    // continues drawing walls in the downward direction.
                    while (p1y - p2y >= 0.000001)
                    {
                        DrawObjectWithTransform(e, wall, p2x, p2y, 0, WallDrawer);
                        // Adds 50 so that next wall is drawn 50 units apart
                        p2y += 50;
                    }

                    // Draws the beginning and ending walls in case they were missed.
                    DrawObjectWithTransform(e, wall, wall.p1.GetX(), wall.p1.GetY(), 0, WallDrawer);
                    DrawObjectWithTransform(e, wall, wall.p2.GetX(), wall.p2.GetY(), 0, WallDrawer);
                }

                // Draw tanks and their associated parts (Turret, Health Bar, Name and Score Tag)
                foreach (Tank tank in theWorld.Tanks.Values)
                {
                    if (tank.hp != 0)
                    {
                        DrawObjectWithTransform(e, tank, tank.loc.GetX(), tank.loc.GetY(), tank.bdir.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, tank, tank.loc.GetX(), tank.loc.GetY(), tank.tdir.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, tank, tank.loc.GetX(), tank.loc.GetY(), 0, HealthBarDrawer);
                        DrawObjectWithTransform(e, tank, tank.loc.GetX(), tank.loc.GetY(), 0, NameTagDrawer);
                    }
                }

                // Draw projectiles
                foreach (Projectile projectile in theWorld.Projectiles.Values)
                {
                    DrawObjectWithTransform(e, projectile, projectile.loc.GetX(), projectile.loc.GetY(), projectile.dir.ToAngle(), ProjectileDrawer);
                }

                // Draw powerups
                foreach (Powerup powerup in theWorld.Powerups.Values)
                {
                    DrawObjectWithTransform(e, powerup, powerup.loc.GetX(), powerup.loc.GetY(), 0, PowerupDrawer);
                }

                // Draw explosion death animation
                ArrayList tanksToRemove = new ArrayList();
                foreach (TankDeath td in theWorld.TankDeaths.Values)
                {
                    DrawObjectWithTransform(e, td, td.x, td.y, 0, DeathAnimation);
                    td.frameCounter--;
                    if (td.frameCounter == 0)
                    {
                        tanksToRemove.Add(td.id);
                    }
                }
                foreach (int f in tanksToRemove)
                {
                    theWorld.TankDeaths.Remove(f);
                }

                // Draw beam
                foreach (Beam b in theWorld.Beams.Values)
                {
                    DrawObjectWithTransform(e, b, b.org.GetX(), b.org.GetY(), b.dir.ToAngle() + 180, BeamDrawer);
                    b.frameCounter--;
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }
    }
}
