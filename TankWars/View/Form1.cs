// @Author Alyssa Johnson and John Stevens
// CS 3500, Fall 2021

using System;
using System.Drawing;
using System.Windows.Forms;
using TankWars;

namespace View
{
    public partial class Form1 : Form
    {
        // Controller
        private GameController GC;
        // Draws for the View
        private DrawingPanel drawingPanel;

        public Form1()
        {
            InitializeComponent();

            //GameController aka the controller in MVC terms
            GC = new GameController();

            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(GC.GetWorld());
            drawingPanel.Location = new Point(0, 45);
            drawingPanel.Size = new Size(900, 900);
            this.Controls.Add(drawingPanel);

            //add event listeners
            GC.UpdateArrived += UpdateArrived;
            GC.Error += GC_Error;
            GC.UpdatePlayerX += GC_UpdatePlayerX;
            GC.UpdatePlayerY += GC_UpdatePlayerY;
            GC.Connected += GC_Connected;
            drawingPanel.MouseDown += DrawingPanel_MouseDown;
            drawingPanel.MouseMove += DrawingPanel_MouseMove;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }

        /// <summary>
        /// Sends information on when a key is no longer being pressed to the game controller
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">a key that is no longer pressed</param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: GC.KeyUp('w'); break;
                case Keys.A: GC.KeyUp('a'); break;
                case Keys.S: GC.KeyUp('s'); break;
                case Keys.D: GC.KeyUp('d'); break;
            }

            //stops that annoying sound from playing
            e.Handled = true;
            e.SuppressKeyPress = true;

        }

        /// <summary>
        /// Sends information on what key was pressed to the game controller
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">a key that was pressed</param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: GC.KeyDown('w'); break;
                case Keys.A: GC.KeyDown('a'); break;
                case Keys.S: GC.KeyDown('s'); break;
                case Keys.D: GC.KeyDown('d'); break;
            }

            //stops that annoying sound from playing
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// Changes some settings once the client is connected to the server
        /// </summary>
        private void GC_Connected()
        {
            this.Invoke(new MethodInvoker(delegate { this.ConnectButton.Enabled = false; }));
            this.Invoke(new MethodInvoker(delegate { this.NameInputTextBox.Enabled = false; }));
            this.Invoke(new MethodInvoker(delegate { this.ServerInputTextBox.Enabled = false; }));
            this.Invoke(new MethodInvoker(delegate { this.KeyPreview = true; }));
            this.BackColor = Color.LightGray;
        }

        /// <summary>
        /// Updates the players y coordinate so that the drawing panel can be notified and adjust the image accordingly
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">client's y coordinate</param>
        private void GC_UpdatePlayerY(object sender, double e)
        {
            drawingPanel.SetY(e);
        }

        /// <summary>
        /// Updates the players x coordinate so that the drawing panel can be notified and adjust the image accordingly
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">client's x coordinate</param>
        private void GC_UpdatePlayerX(object sender, double e)
        {
            drawingPanel.SetX(e);
        }

        /// <summary>
        /// Tells the game controller where to aim based on the mouse's current position
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">mouse's location</param>
        private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            GC.Aim(e.X, e.Y);
        }

        /// <summary>
        /// This method tells the game controller when the mouse is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">mouse button</param>
        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                GC.Fire(true);
            else
            if (e.Button == MouseButtons.Right)
                GC.Fire(false);
        }

        /// <summary>
        /// Method associated with clicking the connect button in the GUI
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //Tells the game controller to begin connecting to the server
            GC.Connect(NameInputTextBox.Text, ServerInputTextBox.Text);
        }


        /// <summary>
        /// This method contains all the things that need to be done every time a frame arrives.
        /// (redrawing the form and sending commands to the server)
        /// </summary>
        private void UpdateArrived()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            try
            {
                this.Invoke(new MethodInvoker(delegate { this.Invalidate(true); }));
            }
            catch
            {
                //surrounding the above statement in a try catch as recommended on piazza
            }

            //prompt sending new instructions
            GC.SendInstructions();
        }

        /// <summary>
        /// Used for throwing errors that came from the Controller/Model
        /// </summary>
        private void GC_Error(object sender, string message)
        {
            MessageBox.Show(message, "", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Opens a text box to show the user the controls
        /// </summary>
        /// <param name="sender">not important, isn't used</param>
        /// <param name="e">not important, isn't used</param>
        private void ControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("W:\t\tMove Up\n"
                       + "A:\t\tMove Left\n"
                       + "S:\t\tMove Down\n"
                       + "D:\t\tMove Right\n"
                       + "Mouse:\t\tAim\n"
                       + "Left Click:\tFire Projectile\n"
                       + "Right Click:\tFire Beam\n"
                , "Controls", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Opens a text box to show the user the "about" info
        /// </summary>
        /// <param name="sender">not important, isn't used</param>
        /// <param name="e">not important, isn't used</param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tankwars client" +
                "\nAuthors Alyssa Johnson and John Stevens" +
                "\nCS 3500 Fall 2021, University of Utah"
                , "About", MessageBoxButtons.OK);
        }
    }
}