// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System;
using System.Text;
using System.Text.RegularExpressions;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TankWars
{
    public class GameController
    {
        //MEMBER VARIABLES

        //the world (the model in MVC terms)
        private world theWorld;
        //our player's name
        private string playername;
        //our tank's id
        private int Id;
        //used to store any partially recieved json messages
        private string temp = "";
        //the client's tank's current x and y coords (used for centering the view)
        private double playerX;
        private double playerY;
        //socketState being used for data transmission
        private SocketState ss;
        //turret represents the mouse's location at all times
        private Vector2D turret;
        //used to hold either "main", "alt", or "none" depending on if the client is firing or not on that frame
        private string firing;
        //string array that acts as a queue for tank movement
        private string[] inputKeys;

        //ACTIONS/EVENT HANDLERS

        //Used to alert the form every time a new set of information has arrived
        public event Action UpdateArrived;
        //Used to send alerts to the view so it can create new message boxes
        public event EventHandler<String> Error;
        //Updates the view's x and y coords of the client's tank so it can center around them
        public event EventHandler<Double> UpdatePlayerX;
        public event EventHandler<Double> UpdatePlayerY;
        //Notifies the view that connection to the server has been made so it can change some of its settings (such as disabling the connect button)
        public event Action Connected;

        /// <summary>
        /// Just a default constructor
        /// </summary>
        public GameController()
        {
            theWorld = new world();
            playerX = 0;
            playerY = 0;
            turret = new Vector2D();
            firing = "none";
            inputKeys = new string[4];
        }

        /// <summary>
        /// Method that initializes a connection with the server
        /// </summary>
        /// <param name="playername">our client's playername</param>
        /// <param name="hostName">the server's ip address</param>
        public void Connect(string playername, string hostName)
        {
            //checks to see if the playername is too long
            if (playername.Length > 16 || playername.Length == 0)
            {
                Error.Invoke(null, "Player name is longer than 16 characters. Please shorten it.");
                return;
            }
            if (hostName.Length == 0)
            {
                Error.Invoke(null, "Please enter a server address.");
                return;
            }

            this.playername = playername;
            Networking.ConnectToServer(OnConnect, hostName, 11000);
        }

        /// <summary>
        /// Callback for connectToServer method, it initializes the getData sequence
        /// </summary>
        /// <param name="state">socketstate being used for data transmission</param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured == true)
            {
                Error.Invoke(null, "Unable to connect to server. Check ip address and try again.");
            }
            else
            {
                //if connection is successful, send playername and begin recieving data
                Networking.Send(state.TheSocket, playername + '\n');
                state.OnNetworkAction = RecieveStartup;
                ss = state;
                Connected.Invoke();
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// recieves the initial data sent from the server (worldsize and player ID)
        /// </summary>
        /// <param name="state"></param>
        private void RecieveStartup(SocketState state)
        {
            //get data and split it into parts
            string info = state.GetData();
            string[] parts = Regex.Split(info, @"(?<=[\n])");

            //if there are less than 2 parts, get more data
            if (parts.Length < 2 || !parts[1].EndsWith("\n"))
            {
                Networking.GetData(state);
                return;
            }

            //get player ID
            this.Id = int.Parse(parts[0]);
            //get the world size
            theWorld.worldSize = int.Parse(parts[1]);

            //remove data and start recieving more
            state.RemoveData(0, parts[0].Length + parts[1].Length);
            state.OnNetworkAction = ReceiveJson;
            Networking.GetData(state);
        }

        /// <summary>
        /// Processes incoming data and updates the view/world
        /// </summary>
        /// <param name="state">the socketstate being used for transmitting data</param>
        private void ReceiveJson(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error.Invoke(null, "Disconnected from server.");
                return;
            }

            string message = state.GetData();
            state.ClearData();

            if (temp != "")
            {
                message = temp + message;
                temp = "";
            }

            string[] parts = Regex.Split(message, @"(?<=[\n])");
            lock (theWorld)
            {
                foreach (string part in parts)
                {
                    //gets rid of any empty strings created by the regex
                    if (part.Length == 0)
                    {
                        continue;
                    }

                    // If the part does not end with a newline, then it is an incomplete message and saved into a temporary string.
                    // When the rest of the message is sent next time, this temp string will be rejoined with the first string in
                    // the next message since it should contain its second half.
                    if (!part.EndsWith("\n"))
                    {
                        temp = part;
                        break;
                    }

                    // Saves the object that is either a tank, wall, beam, projectile, powerup
                    JObject obj = JObject.Parse(part);
                    JToken token;

                    // If the object is a tank, its tank value will not be null and this deserializes the part into a tank and adds it to the tank dictionary.
                    token = obj["tank"];
                    if (token != null)
                    {
                        Tank t = JsonConvert.DeserializeObject<Tank>(part);
                        if ((t.died == true || t.dc == true) && theWorld.TankDeaths.ContainsKey(t.tank) == false)
                        {
                            theWorld.TankDeaths.Add(t.tank, new TankDeath(t.tank, t.loc.GetX(), t.loc.GetY()));
                            theWorld.Tanks.Remove(t.tank);
                            continue;
                        }
                        else
                            theWorld.Tanks[t.tank] = t;
                        if (t.tank == Id)
                        {
                            //updates the view to keep it centered
                            if (playerX != t.loc.GetX())
                            {
                                playerX = t.loc.GetX();
                                UpdatePlayerX.Invoke(null, playerX);
                            }
                            if (playerY != t.loc.GetY())
                            {
                                playerY = t.loc.GetY();
                                UpdatePlayerY.Invoke(null, playerY);
                            }
                        }
                        continue;
                    }

                    // If the object is a projectile, its projectile value will not be null and this deserializes the part into a projectile and adds it to the projectile dictionary.
                    token = obj["proj"];
                    if (token != null)
                    {
                        Projectile p = JsonConvert.DeserializeObject<Projectile>(part);
                        if (p.died == true)
                            theWorld.Projectiles.Remove(p.proj);
                        else
                            theWorld.Projectiles[p.proj] = p;
                        continue;
                    }

                    // If the object is a powerup, its powerup value will not be null and this deserializes the part into a powerup and adds it to the powerup dictionary.
                    token = obj["power"];
                    if (token != null)
                    {
                        Powerup p = JsonConvert.DeserializeObject<Powerup>(part);
                        if (p.died == true)
                            theWorld.Powerups.Remove(p.power);
                        else
                            theWorld.Powerups[p.power] = p;
                        continue;
                    }

                    // If the object is a beam, its beam value will not be null and this deserializes the part into a beam and adds it to the beam dictionary.
                    token = obj["beam"];
                    if (token != null)
                    {
                        Beam b = JsonConvert.DeserializeObject<Beam>(part);
                        theWorld.Beams[b.beam] = b;
                        continue;
                    }

                    //wall comes last because it is only recieved one time
                    // If the object is a wall, its wall value will not be null and this deserializes the part into a wall and adds it to the wall dictionary.
                    token = obj["wall"];
                    if (token != null)
                    {
                        Wall w = JsonConvert.DeserializeObject<Wall>(part);
                        theWorld.Walls[w.wall] = w;
                        continue;
                    }
                }
            }

            // This will trigger the view to redraw the game
            UpdateArrived?.Invoke();

            // Keeps the loop going
            Networking.GetData(state);
        }

        /// <summary>
        /// Updates the direction that the tank should be aiming 
        /// </summary>
        /// <param name="x">x coordinate of cursor</param>
        /// <param name="y">y coordinate of cursor</param>
        public void Aim(int x, int y)
        {
            x = x - 450;
            y = y - 450;

            //prevents it from crashing if you mouse over the drawing panel before you're connected to the server
            if (ss != null)
            {
                //updates the class variable that represents the cursors location
                turret = new Vector2D(x, y);
                turret.Normalize();
            }
        }

        /// <summary>
        /// updates the status of the firing variable
        /// </summary>
        /// <param name="main">bool representing which firing mode being used</param>
        public void Fire(bool main)
        {
            //the null check prevents it from crashing if you click on the drawing panel before you're connected to the server
            if (ss != null && main == true)
                firing = "main";
            if (ss != null && main == false)
                firing = "alt";
        }

        /// <summary>
        /// Decides which key to add to the movement queue
        /// </summary>
        /// <param name="key">key being pressed</param>
        public void KeyDown(char key)
        {
            switch (key)
            {
                case 'w': Enqueue("up"); break;
                case 'a': Enqueue("left"); break;
                case 's': Enqueue("down"); break;
                case 'd': Enqueue("right"); break;
            }
        }

        /// <summary>
        /// Decides which key to remove from the movement queue
        /// </summary>
        /// <param name="key">key no longer being pressed</param>
        public void KeyUp(char key)
        {
            switch (key)
            {
                case 'w': Dequeue("up"); break;
                case 'a': Dequeue("left"); break;
                case 's': Dequeue("down"); break;
                case 'd': Dequeue("right"); break;
            }
        }

        /// <summary>
        /// This method sends commands back to the server
        /// </summary>
        public void SendInstructions()
        {
            //creates a stringbuilder, then builds a complete instruction one part at a time
            StringBuilder instructions = new StringBuilder("{\"moving\":\"");
            if (inputKeys[0] == null)
                instructions.Append("none" + "\",\"fire\":\"");
            else
                instructions.Append(inputKeys[0] + "\",\"fire\":\"");
            instructions.Append(firing + "\",\"tdir\":{\"x\":");
            instructions.Append(turret.GetX() + ",\"y\":");
            instructions.Append(turret.GetY() + "}}\n");

            //then sends the instruction
            Networking.Send(ss.TheSocket, instructions.ToString());

            //resets firing
            firing = "none";


        }

        /// <summary>
        /// Adds a string to the movement queue
        /// </summary>
        /// <param name="s"></param>
        private void Enqueue(string s)
        {
            for (int i = 0; i < 4; i++)
            {
                //if this spot in the queue is occupied and equals the thing trying to be enqueued, exit
                if (inputKeys[i] != null && inputKeys[i].Equals(s))
                {
                    return;
                }
                //store s in the first non null spot
                if (inputKeys[i] == null)
                {
                    inputKeys[i] = s;
                    return;
                }
            }
        }

        /// <summary>
        /// Removes a string from the movement queue
        /// </summary>
        /// <param name="s">string being dequeued</param>
        private void Dequeue(string s)
        {
            int index = 0;
            //find location of string s
            for (int i = 0; i < 4; i++)
            {
                //finds the index of s if it exists in the array
                if (inputKeys[i] != null && inputKeys[i].Equals(s))
                {
                    index = i;
                    break;
                }
            }

            //moves the rest of the queue to the front
            for (int i = index; i < 4; i++)
            {
                if (i < 3)
                {
                    inputKeys[i] = inputKeys[i + 1];
                }
            }

        }

        /// <summary>
        /// Returns the world member variable
        /// </summary>
        /// <returns>a world</returns>
        public world GetWorld()
        {
            return theWorld;
        }
    }
}



