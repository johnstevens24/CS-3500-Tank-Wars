// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System;
using System.IO;
using System.Xml;

namespace TankWars
{
    class Settings
    {
        // private member variables
        // Settings XML
        private int UniverseSize; // The number of units on each side of the square universe.
        private int MSPerFrame; // How often the server attempts to update the world.
        private int FramesPerShot; // How many frames a tank waits between firing projectiles. This is not in units of time. It is in units of frames.
        private int RespawnRate; // How many frames a tank waits before respawning. This is not in units of time. It is in units of frames.

        // temporary variables used when reading an XML
        private Wall tempWall; 
        private Vector2D tempVectorStart; 
        private Vector2D tempVectorEnd; 
        private double tempX; // temporary x-coordinate of wall
        private double tempY; // temporary y-coordinate of wall

        private world theWorld; 
        private int wallIDCount = 0; // Increments up and gives each wall a unique ID
        private string filePathway;


        /// <summary>
        /// A constructor for creating a Settings object.
        /// It will read the settings information from an XML file.
        /// </summary>
        /// <param name="filepath"> The relative path of the file containing settings information. </param>
        /// <param name="theWorld"></param>
        public Settings(string filepath, world theWorld)
        {
            this.theWorld = theWorld;
            filePathway = filepath;
        }


        /// <summary>
        /// This reads an XML file containing settings information.
        /// It initializes the values of a Settings object.
        /// Initializes values for UniverseSize, MSPerFrame, FramesPerShot, RespawnRate, and creates 
        /// Walls (if they exist) and populates theWorld's Walls dictionary.
        /// </summary>
        /// <param name="filePathway"> The relative path of the file containing settings information. </param>
        public bool ReadXML()
        {
            try
            {
                // Throws an Exception when the given file path does not exist.
                if (!File.Exists(Path.GetFullPath(filePathway)))
                {
                    throw new Exception("Unable to locate file");
                }


                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filePathway))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "UniverseSize":
                                    reader.Read();
                                    UniverseSize = Int32.Parse(reader.Value);
                                    break;

                                case "MSPerFrame":
                                    reader.Read();
                                    MSPerFrame = Int32.Parse(reader.Value);
                                    break;

                                case "FramesPerShot":
                                    reader.Read();
                                    FramesPerShot = Int32.Parse(reader.Value);
                                    break;
                                case "RespawnRate":
                                    reader.Read();
                                    RespawnRate = Int32.Parse(reader.Value);
                                    break;
                                case "Wall":
                                    reader.Read();
                                    while (reader.Name != "Wall")
                                    {
                                        reader.Read();
                                        switch (reader.Name)
                                        {
                                            case "p1":
                                                reader.Read(); // value should be <x>
                                                reader.Read(); // value should be x-coordinate
                                                Double.TryParse(reader.Value, out tempX);
                                                reader.Read(); // value should be <x>
                                                reader.Read(); // value should be <y>
                                                reader.Read(); // value should be y-coordinate
                                                Double.TryParse(reader.Value, out tempY);
                                                reader.Read(); // value should be <y>
                                                reader.Read(); // value should be <p1>
                                                tempVectorStart = new Vector2D(tempX, tempY);
                                                break;
                                            case "p2":
                                                reader.Read(); // value should be <x>
                                                reader.Read(); // value should be x-coordinate
                                                Double.TryParse(reader.Value, out tempX);
                                                reader.Read(); // value should be <x>
                                                reader.Read(); // value should be <y>
                                                reader.Read(); // value should be y-coordinate
                                                Double.TryParse(reader.Value, out tempY);
                                                reader.Read(); // value should be <y>
                                                reader.Read(); // value should be <p1>
                                                tempVectorEnd = new Vector2D(tempX, tempY);
                                                break;
                                        }
                                    }
                                    tempWall = new Wall(tempVectorStart, tempVectorEnd, wallIDCount);
                                    // Adds wall to theWorld's Walls dictionary. The key is an integer starting from zero and counting the number of walls.
                                    theWorld.Walls.Add(wallIDCount, tempWall);
                                    wallIDCount++;
                                    break;
                                case "GameSettings": break;
                                default: throw new Exception("Illegal xml tag encountered.");
                            }
                        }
                    }
                }
                return true;
            }
            // Catches Errors if there was a problem finding or reading an XML file.
            catch (Exception e)
            {
                Console.WriteLine("There was an error reading the settings file: " + e.Message);
                return false;
            }
            
        }


        /// <summary>
        /// Getter method for the Universe Size.
        /// </summary>
        /// <returns> UniverseSize </returns>
        public int GetUniverseSize()
        {
            return UniverseSize;
        }


        /// <summary>
        /// Getter method for the MSPerFrame.
        /// </summary>
        /// <returns> MSPerFrame </returns>
        public int GetMSPerFrame()
        {
            return MSPerFrame;
        }


        /// <summary>
        /// Getter method for the FramesPerShot.
        /// </summary>
        /// <returns> FramesPerShot </returns>
        public int GetFramesPerShot()
        {
            return FramesPerShot;
        }


        /// <summary>
        /// Getter method for the RespawnRate.
        /// </summary>
        /// <returns> RespawnRate </returns>
        public int GetRespawnRate()
        {
            return RespawnRate;
        }
    }
}

