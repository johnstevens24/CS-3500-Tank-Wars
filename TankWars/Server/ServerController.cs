// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021


using NetworkUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace TankWars
{
    class ServerController
    {
        private world theWorld;
        private Settings settings;
        private Dictionary<int, SocketState> clients = new Dictionary<int, SocketState>();
        private StringBuilder wallInfo;
        public bool serverSetupOk; // Whether or not reading from the xml and setting up the world was successful (used in program.cs)


        /// <summary>
        /// Constructs a ServerController.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="theWorld"></param>
        public ServerController()
        {
            theWorld = new world();
            settings = new Settings("..\\..\\..\\..\\Resources\\settings.xml", theWorld);
            if (!settings.ReadXML())
            {
                serverSetupOk = false;
                Console.WriteLine("Shutting down");
            }
            else
            {
                theWorld.worldSize = settings.GetUniverseSize();
                theWorld.FramesPerShot = settings.GetFramesPerShot();
                theWorld.RespawnRate = settings.GetRespawnRate();
                wallInfo = new StringBuilder();

                foreach (Wall w in theWorld.Walls.Values)
                {
                    wallInfo.Append(JsonConvert.SerializeObject(w));
                    wallInfo.Append("\n");
                }
                serverSetupOk = true;
            }

        }


        /// <summary>
        /// Constructs a ServerController with a provided settings file path. Not needed but useful.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="theWorld"></param>
        public ServerController(string settingsFilePath)
        {
            theWorld = new world();
            settings = new Settings(settingsFilePath, theWorld);
            if (!settings.ReadXML())
            {
                serverSetupOk = false;
                Console.WriteLine("Shutting down");
            }
            else
            {
                theWorld.worldSize = settings.GetUniverseSize();
                theWorld.FramesPerShot = settings.GetFramesPerShot();
                theWorld.RespawnRate = settings.GetRespawnRate();
                wallInfo = new StringBuilder();

                foreach (Wall w in theWorld.Walls.Values)
                {
                    wallInfo.Append(JsonConvert.SerializeObject(w));
                    wallInfo.Append("\n");
                }
                serverSetupOk = true;
            }

        }


        /// <summary>
        /// Starts the server and creates a new thread to update the world and send info to clients
        /// </summary>
        internal void Start()
        {
            Networking.StartServer(NewClient, 11000);
            Thread t = new Thread(Update);
            t.Start();
            Console.WriteLine("Server connected. Clients can connect now.");
        }


        /// <summary>
        /// Begins client handshake and sets the callback to ReceivePlayerName
        /// </summary>
        /// <param name="client">the client's SocketState</param>
        private void NewClient(SocketState client)
        {
            client.OnNetworkAction = ReceivePlayerName;
            Networking.GetData(client);
        }


        /// <summary>
        /// Takes in the client's name and sends them their id, the world size, and the walls.
        /// Also adds the client to list of clients and creates a tank for them in the world.
        /// Sets the callback to ReceiveControlCommand
        /// </summary>
        /// <param name="client"></param>
        private void ReceivePlayerName(SocketState client)
        {
            string name = client.GetData();
            //If it doesn't end with \n, its an incomplete message so wait for more
            if (!name.EndsWith("\n"))
            {
                client.GetData();
                return;
            }

            //removing the name data from the SocketState data buffer
            client.RemoveData(0, name.Length);
            name = name.Trim();

            //Send ID and world size
            Networking.Send(client.TheSocket, client.ID + "\n" + theWorld.worldSize + "\n");
            //Send wall info
            Networking.Send(client.TheSocket, wallInfo.ToString());

            //Create and add the client's tank to the world
            //Chose to lock the world because adding a tank to it while enumerating would throw an error
            lock (theWorld)
            {
                theWorld.Tanks[(int)client.ID] = new Tank((int)client.ID, name);
                theWorld.Tanks[(int)client.ID].join = true;
                theWorld.Tanks[(int)client.ID].loc = theWorld.GetRandomSpawnPoint();
            }

            //Add the client to the list of clients
            lock (clients)
            {
                clients.Add((int)client.ID, client);
            }

            Console.WriteLine("Client " + client.ID + " connected.");
            // Callback changed to ReceiveControlCommand (callback will no longer be changed)
            client.OnNetworkAction = ReceiveControlCommand;
            Networking.GetData(client);
        }


        /// <summary>
        /// Contains the infinite loop that updates clients' and the server's individual worlds. Sends info to client and recieves
        /// control commands from the client.
        /// </summary>
        private void Update()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //infinite while loop that updates everything
            while (true)
            {
                while (watch.ElapsedMilliseconds < settings.GetMSPerFrame())
                {
                    // waits until the specified amount of time has passed
                }
                watch.Restart();

                StringBuilder sb = new StringBuilder();

                //create a message to send to the clients
                lock (theWorld)
                {
                    theWorld.Update();

                    foreach (Tank t in theWorld.Tanks.Values)
                    {
                        if (t.respawnCounter == 0)  // tanks is alive, so send
                        {
                            sb.Append(JsonConvert.SerializeObject(t) + "\n");
                        }
                        else if (t.respawnCounter == 1) // means tank is about to respawn so acquire necessary spawn data
                        {
                            t.loc = theWorld.GetRandomSpawnPoint();
                            t.hp = 3;//change to whatever the default is
                            sb.Append(JsonConvert.SerializeObject(t) + "\n");
                            t.respawnCounter--;
                        }
                        else if (t.died == true) // tank died this frame. will only be sent once. notifies the client the tank died.
                        {
                            sb.Append(JsonConvert.SerializeObject(t) + "\n");
                            t.died = false;
                        }
                        else // keeps counting down the respawn wait time
                        {
                            t.respawnCounter--;
                        }

                        if (t.join == true)
                            t.join = false;
                    }

                    foreach (Projectile p in theWorld.Projectiles.Values)
                    {
                        sb.Append(JsonConvert.SerializeObject(p) + "\n");
                    }

                    foreach (Beam b in theWorld.Beams.Values)
                    {
                        sb.Append(JsonConvert.SerializeObject(b) + "\n");
                    }

                    foreach (Powerup p in theWorld.Powerups.Values)
                    {
                        sb.Append(JsonConvert.SerializeObject(p) + "\n");
                    }
                }

                String frame = sb.ToString();

                //Send update to each of the clients
                lock (clients)
                {
                    foreach (SocketState client in clients.Values)
                    {
                        Networking.Send(client.TheSocket, frame);
                    }
                }

                //Removes anything that died on this frame
                lock (theWorld)
                {
                    theWorld.RemoveDeadObjects();
                }
            }
        }


        /// <summary>
        /// The callback for after a client has been connected and added to the world.
        /// This callback will be called infinitely until the client diconnects.
        /// This method receives data from the client and deserializes the ControlCommand message.
        /// It receives the client's movement request and moves the tank in the correct direction.
        /// It receives the information of where the turret should be pointing and points the turret in that direction.
        /// If it receives a fire command, it will fire a projectile if "main" and a beam if "alt".
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveControlCommand(SocketState client)
        {
            string totalData = client.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            foreach (string p in parts)
            {
                if (p.Length == 0)
                    continue;

                if (p[p.Length - 1] != '\n')
                    break;

                // Deserializes the control command and adds it to the control commands dictionary
                // The control commands dictionary will not have more than one control command per a client.
                ControlCommand ctrlCmd = JsonConvert.DeserializeObject<ControlCommand>(p);

                //this if statement only allows control commands to change the world if they're coming from a tank thats alive
                if (theWorld.Tanks[(int)client.ID].hp != 0)
                    lock (theWorld)
                    {
                        if (theWorld.CtrlCmds.ContainsKey((int)client.ID) && (theWorld.CtrlCmds[(int)client.ID].fire == "main" || theWorld.CtrlCmds[(int)client.ID].fire == "alt"))
                        {
                            //if for some reason two control commands show up on the same frame, it will prioritize the one that is firing so the game feels responsive.
                            //Without this if statement sometimes when you right click beams wouldn't fire
                        }
                        else
                            theWorld.CtrlCmds[(int)client.ID] = ctrlCmd;
                    }
                // This removes the used data.
                client.RemoveData(0, p.Length);
            }

            if (client.ErrorOccured == true)
            {
                Console.WriteLine("Client " + client.ID + " disconnected.");
                theWorld.Tanks[(int)client.ID].dc = true;
                theWorld.Tanks[(int)client.ID].hp = 0;
                clients.Remove((int)client.ID);
            }
            else
                Networking.GetData(client);
        }


    }
}


