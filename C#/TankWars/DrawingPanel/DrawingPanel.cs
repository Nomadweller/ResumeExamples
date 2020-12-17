using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;
using World;

namespace DrawingPanel
{
    /// <summary>
    /// Class used to display the game.
    /// </summary>
    public class DrawingPanel : Panel
    {
        //world used by game and all images drawing panel will need to draw.
        private TheWorld drawingWorld;
        private Image yellowTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\YellowTank.png");
        private Image redTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\RedTank.png");
        private Image blueTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\BlueTank.png");
        private Image greenTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\GreenTank.png");
        private Image lightgreenTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\LightGreenTank.png");
        private Image orangeTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\OrangeTank.png");
        private Image darkTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\DarkTank.png");
        private Image purpleTank = Image.FromFile(@"..\..\..\Resources\Libraries\Images\PurpleTank.png");

        private Image yellowTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\YellowTurret.png");
        private Image redTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\RedTurret.png");
        private Image blueTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\BlueTurret.png");
        private Image greenTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\GreenTurret.png");
        private Image lightgreenTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\LightGreenTurret.png");
        private Image orangeTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\OrangeTurret.png");
        private Image darkTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\DarkTurret.png");
        private Image purpleTurret = Image.FromFile(@"..\..\..\Resources\Libraries\Images\PurpleTurret.png");

        private Image yellowProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-yellow.png");
        private Image redProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-red.png");
        private Image blueProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-blue.png");
        private Image greenProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-green.png");
        private Image lightgreenProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-green.png");
        private Image orangeProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-brown.png");
        private Image darkProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-grey.png");
        private Image purpleProj = Image.FromFile(@"..\..\..\Resources\Libraries\Images\shot-violet.png");

        private Image wallImage = Image.FromFile(@"..\..\..\Resources\Libraries\Images\WallSprite.png");
        private Image backgroundImage = Image.FromFile(@"..\..\..\Resources\Libraries\Images\Background.png");


        /// <summary>
        ///initializing the world for the drawing panel to draw
        /// </summary>
        /// <param name="w">The game world.</param>
        public void SetWorld(TheWorld w)
        {
            drawingWorld = w;
            DoubleBuffered = true;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;

        }

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }
        /// <summary>
        /// Method used to draw game walls.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall p = o as Wall;
            int width = 50;
            int height = 50;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(wallImage, -(width / 2), -(height / 2), width, height);
        }

        /// <summary>
        /// Method used to draw game projectiles.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void ProjDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            int tankID = p.GetOwnerID();
            int width = 30;
            int height = 30;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            while (tankID >= 7)
                tankID -= 7;
            switch (tankID)
            {
                case 0:
                    e.Graphics.DrawImage(yellowProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 1:
                    e.Graphics.DrawImage(blueProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 2:
                    e.Graphics.DrawImage(darkProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 4:
                    e.Graphics.DrawImage(orangeProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 5:
                    e.Graphics.DrawImage(purpleProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 6:
                    e.Graphics.DrawImage(redProj, -(width / 2), -(height / 2), width, height);
                    break;
                case 7:
                    e.Graphics.DrawImage(lightgreenProj, -(width / 2), -(height / 2), width, height);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Method used to draw game powerups.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void PowerUpDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 16;
            int height = 16;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush greenBrush = new SolidBrush(Color.Green))
            using (SolidBrush yellowBrush = new SolidBrush(Color.Yellow))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.FillEllipse(yellowBrush, r);
                width = 8;
                height = 8;
                r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.FillEllipse(greenBrush, r);
            }
        }
        /// <summary>
        /// Method used to draw tank turrets.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            int tankID = t.GetID();
            int width = 50;
            int height = 50;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            while (tankID >= 8)
                tankID -= 8;
            switch (tankID)
            {
                case 0:
                    e.Graphics.DrawImage(yellowTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 1:
                    e.Graphics.DrawImage(blueTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 2:
                    e.Graphics.DrawImage(darkTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 4:
                    e.Graphics.DrawImage(orangeTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 5:
                    e.Graphics.DrawImage(purpleTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 6:
                    e.Graphics.DrawImage(redTurret, -(width / 2), -(height / 2), width, height);
                    break;
                case 7:
                    e.Graphics.DrawImage(lightgreenTurret, -(width / 2), -(height / 2), width, height);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Method used to draw game tanks.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            int tankID = t.GetID();
            int width = 60;
            int height = 60;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //switchs tank color based on playerID
            while (tankID >= 8)
                tankID -= 8;
            switch (tankID)
            {
                case 0:
                    e.Graphics.DrawImage(yellowTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 1:
                    e.Graphics.DrawImage(blueTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 2:
                    e.Graphics.DrawImage(darkTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 4:
                    e.Graphics.DrawImage(orangeTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 5:
                    e.Graphics.DrawImage(purpleTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 6:
                    e.Graphics.DrawImage(redTank, -(width / 2), -(height / 2), width, height);
                    break;
                case 7:
                    e.Graphics.DrawImage(lightgreenTank, -(width / 2), -(height / 2), width, height);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Method used to draw game beam attacks.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                int width = 2400;
                int height = 2;
                Rectangle r = new Rectangle(0, 0, width, height);
                e.Graphics.FillRectangle(whiteBrush, r);
            }
        }
        /// <summary>
        /// Method used to draw game user interface.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        private void GUIDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            int health = t.GetHealth();
            int width;
            int height = 8;
            Rectangle r;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            using (SolidBrush greenBrush = new SolidBrush(Color.Green))
            using (SolidBrush yellowBrush = new SolidBrush(Color.Yellow))
            {
                //Handles varying healthbars of player
                if (health >= 3)
                {
                    width = 36;
                    r = new Rectangle(-20, -50, width, height);
                    e.Graphics.FillRectangle(greenBrush, r);
                }
                else if (health == 2)
                {
                    width = 24;
                    r = new Rectangle(-20, -50, width, height);
                    e.Graphics.FillRectangle(yellowBrush, r);
                }
                else if (health == 1)
                {
                    width = 12;
                    r = new Rectangle(-20, -50, width, height);
                    e.Graphics.FillRectangle(redBrush, r);
                }
            }
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                string playerDetails = t.GetName() + " : " + t.GetScore().ToString();
                Font arial = new Font("Arial", 12, FontStyle.Bold);
                e.Graphics.DrawString(playerDetails, arial, whiteBrush, -40, 30);
            }
        }
        /// <summary>
        /// Helper method to handle dynamic walls and the information they can come with.
        /// </summary>
        /// <param name="wall">The wall to draw.</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void DrawWall(Wall wall, PaintEventArgs e)
        {
            float startX = (float)wall.GetStartLocation().GetX();
            float startY = (float)wall.GetStartLocation().GetY();
            float endX = (float)wall.GetEndLocation().GetX();
            float endY = (float)wall.GetEndLocation().GetY();
            float temp = startX;
            while (temp <= endX)
            {
                DrawObjectWithTransform(e, wall, drawingWorld.GetWorldSize(), temp, startY, 0, WallDrawer);
                temp += 50;
            }

            temp = startX;
            while (endX <= temp)
            {
                DrawObjectWithTransform(e, wall, drawingWorld.GetWorldSize(), temp, startY, 0, WallDrawer);
                temp -= 50;
            }

            temp = startY;
            while (temp <= endY)
            {
                DrawObjectWithTransform(e, wall, drawingWorld.GetWorldSize(), startX, temp, 0, WallDrawer);
                temp += 50;
            }

            temp = startY;
            while (endY <= temp)
            {
                DrawObjectWithTransform(e, wall, drawingWorld.GetWorldSize(), startX, temp, 0, WallDrawer);
                temp -= 50;
            }
        }
        /// <summary>
        /// Method used to draw the game on frame.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (drawingWorld == null)
            {
                return;
            }
            try
            {
                //centers the world.
                int playerID = drawingWorld.GetPlayerID();
                int worldSize = drawingWorld.GetWorldSize();
                double playerX = drawingWorld.GetTankLocation(playerID).GetX();
                double playerY = drawingWorld.GetTankLocation(playerID).GetY();

                // calculate view/world size ratio
                double ratio = (double)this.Size.Width / (double)worldSize;
                int halfSizeScaled = (int)(worldSize / 2.0 * ratio);

                double inverseTranslateX = -WorldSpaceToImageSpace(worldSize, playerX) + halfSizeScaled;
                double inverseTranslateY = -WorldSpaceToImageSpace(worldSize, playerY) + halfSizeScaled;

                e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

            }
            catch
            {

            }
            lock (drawingWorld)
            {

                //draws background before anything else
                e.Graphics.DrawImage(backgroundImage, 0, 0, drawingWorld.GetWorldSize(), drawingWorld.GetWorldSize());

                //draws each object based on the type of dictionary it pulls from in the world.
                foreach (Wall wall in drawingWorld.GetWalls())
                {
                    DrawWall(wall, e);
                }

                foreach (Projectile proj in drawingWorld.GetProjectiles())
                {
                    double X = proj.GetLocation().GetX();
                    double Y = proj.GetLocation().GetY();
                    double angle = proj.GetOrientation().ToAngle();
                    DrawObjectWithTransform(e, proj, drawingWorld.GetWorldSize(), X, Y, angle, ProjDrawer);
                }

                foreach (Tank tank in drawingWorld.GetTanks())
                {
                    double X = tank.GetLocation().GetX();
                    double Y = tank.GetLocation().GetY();
                    double angle = tank.GetOrientation().ToAngle();
                    double turretangle = tank.GetAim().ToAngle();

                    DrawObjectWithTransform(e, tank, drawingWorld.GetWorldSize(), X, Y, angle, TankDrawer);
                    DrawObjectWithTransform(e, tank, drawingWorld.GetWorldSize(), X, Y, turretangle, TurretDrawer);
                    DrawObjectWithTransform(e, tank, drawingWorld.GetWorldSize(), X, Y, 0, GUIDrawer);

                }
                foreach (Powerup power in drawingWorld.GetPowerups())
                {
                    double X = power.GetLocation().GetX();
                    double Y = power.GetLocation().GetY();
                    DrawObjectWithTransform(e, power, drawingWorld.GetWorldSize(), X, Y, 0, PowerUpDrawer);
                }
                List<Beam> myBeams = drawingWorld.GetBeams().ToList();
                foreach (Beam beam in myBeams)
                {
                    if(beam == null)
                    {
                        continue;
                    }
                    Task.Run(()=>FireBeam(drawingWorld,beam));
                    double X = beam.GetLocation().GetX();
                    double Y = beam.GetLocation().GetY();
                    double angle = beam.GetDirection().ToAngle();
                    DrawObjectWithTransform(e, beam, drawingWorld.GetWorldSize(), X, Y, angle - 90, BeamDrawer);
                }
            }
            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
            //drawingWorld.RemoveAll();

        }

        private async void FireBeam(TheWorld world, Beam beam)
        {
            //Invoke drawing on main thread
            await Task.Delay(500);
            //remove beam
            world.RemoveBeam(beam);
        }

    }


}
