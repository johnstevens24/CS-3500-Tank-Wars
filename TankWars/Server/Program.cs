// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System;

namespace TankWars
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerController sc = new ServerController();
            if(sc.serverSetupOk)
            {
                sc.Start();
                Console.Read();
            }
            
                
        }
    }
}
