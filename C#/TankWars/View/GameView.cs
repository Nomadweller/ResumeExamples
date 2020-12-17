using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using World;

namespace View
{
    /// <summary>
    /// A user view that contains all the visual information the player needs to see.
    /// </summary>
    public partial class GameView : Form
    {
        // The controller handles updates from the "server"
        // and notifies us via an event
        private GameController controller;
        DrawingPanel.DrawingPanel drawingPanelGame;

        // World is a simple container for Players and Powerups
        // The controller owns the world, but we have a reference to it
        //private World theWorld;

        public GameView(GameController c)
        {
            InitializeComponent();
            controller = c;
            //registering events that need to happen as soon as they are called on by the controller.
            controller.Connected += getViewName;
            controller.MessagesArrived += OnFrame;
            ClientSize = new Size(800, 800);
            drawingPanelGame = new DrawingPanel.DrawingPanel();
            drawingPanelGame.Location = new Point(0, 0);
            drawingPanelGame.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            drawingPanelGame.Anchor = AnchorStyles.Bottom;
            drawingPanelGame.Enabled = false;
            this.Controls.Add(drawingPanelGame);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            txtBoxName.Enabled = false;
            txtBoxServer.Enabled = false;
            btnConnect.Enabled = false;
            controller.ConnectToServer(txtBoxServer.Text);
        }
        private void OnFrame(TheWorld world)
        {
            // Don't try to redraw if the window doesn't exist yet.
            // This might happen if the controller sends an update
            // before the Form has started.
            if (!IsHandleCreated)
                return;

            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            try
            {
                drawingPanelGame.SetWorld(world);
                MethodInvoker m = new MethodInvoker(() => this.Invalidate(true));
                this.Invoke(m);
                controller.TalkToserver();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Method used by the controller via an event in order to find out what the player names themself.
        /// </summary>
        /// <returns>Name input by the user</returns>
        public string getViewName()
        {
            return txtBoxName.Text;
        }
        /// <summary>
        /// Method used to control all button commands.
        /// </summary>
        /// <param name="e">Key pressed by user</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            controller.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            controller.OnKeyPress(e);
            base.OnKeyPress(e);
        }

        /// <summary>
        /// Method stops movement
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            controller.OnKeyUp(e);
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Method stops movement
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            controller.OnMouseUp(e);
            base.OnMouseUp(e);
        }


        /// <summary>
        /// Method used to rotate turret when moved.
        /// </summary>
        /// <param name="e">Mouse movement by user.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        { 
            controller.OnMouseMove(e);
            base.OnMouseMove(e);
        }
        /// <summary>
        /// Method used to handle mouse click events typically used for firing tank.
        /// </summary>
        /// <param name="e">Mouse button press.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            controller.OnMouseDown(e);
            base.OnMouseDown(e);
        }


    }
}
