using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Gravity
{
    public partial class Form1 : Form
    {

        delegate void SetTextCallback(string text);
        DateTime Time;
        Graphics gPanel;
        Pen MyPen;
        int FPS, FPSCounter, seconds, miliseconds, before, CenterMass;
        Planet A;
        Line Arrow;
        bool MovingAllowed, MousePressed;
        double G = 0.0005;
        public Form1()
        {
            InitializeComponent();
            Time = new DateTime();
            MyPen = new Pen(Color.Black);
            gPanel = pictureBox1.CreateGraphics();
            Arrow = new Line();
            MovingAllowed = false;
            seconds = 0;
            FPSCounter = 0;
            MousePressed = false;
            miliseconds = 1;
            CenterMass = 200;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.Text = text;
            }
        }
        private void CalculatePhysics()
        {
            miliseconds = Math.Abs(Time.Millisecond - before);
            before = Time.Millisecond;
            //this.Inv
            //this.SetText(Time.Millisecond.ToString());
            //SetText(miliseconds.ToString());
            A.Position.X += A.Speed.X/10;
            A.Position.Y += A.Speed.Y/10;
            A.Acceleration=(G*CenterMass*A.Weight/Math.Pow(getDistance(A.Position),2)*(new PointD(getCenter().X-A.Position.X,getCenter().Y-A.Position.Y)));
            A.Speed += miliseconds * A.Acceleration;
        }
        private void DrawAll()
        {
            MyPen.Color = Color.Black;
            gPanel.FillEllipse(MyPen.Brush, getCenter().X - 10, getCenter().Y - 10, 20,20);
            gPanel.FillEllipse(MyPen.Brush, Convert.ToInt32(A.Position.X) - 10, Convert.ToInt32(A.Position.Y) - 10, 20, 20);
            SetText(FPS.ToString());
        }
        private void DrawClear()
        {
            MyPen.Color = Color.White;
            gPanel.FillEllipse(MyPen.Brush, getCenter().X - 10, getCenter().Y - 10, 20,20);
            gPanel.FillEllipse(MyPen.Brush, Convert.ToInt32(A.Position.X) - 10, Convert.ToInt32(A.Position.Y) - 10, 20, 20);
        }
        private void Mov()
        {
            if (MovingAllowed)
            {
                Time = DateTime.Now;
                if (Time.Second != seconds)
                {
                    FPS = FPSCounter;
                    FPSCounter = 0;
                    seconds = Time.Second;
                }
                else
                {
                    FPSCounter++;
                }
                DrawClear();
                CalculatePhysics();
                DrawAll();
            }
        }
        public Point getCenter()
        {
            return new Point(pictureBox1.Width/2, pictureBox1.Height/2);
        }
        public double getDistance(PointD Position)
        {
            return Math.Sqrt(Math.Pow(Position.X-getCenter().X,2)+Math.Pow(Position.Y-getCenter().Y,2));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            gPanel.Clear(Color.White);
            MovingAllowed = false;
            Arrow.Begin = e.Location;
            MousePressed = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //Arrow.End = e.Location;
            MousePressed = false;
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gPanel = pictureBox1.CreateGraphics();
            A = new Planet(200, new PointD(Arrow.Begin.X,Arrow.Begin.Y), Arrow.End.X - Arrow.Begin.X, Arrow.End.Y - Arrow.Begin.Y);
            gPanel.Clear(Color.White);
            MovingAllowed = true;
            timer1.Start();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePressed)
            {
                MyPen.Color = Color.White;
                gPanel.DrawLine(MyPen, Arrow.Begin, Arrow.End);
                Arrow.End = e.Location;
                MyPen.Color = Color.Red;
                gPanel.DrawLine(MyPen, Arrow.Begin, Arrow.End);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Mov();
        }
      
    }
    public class Planet
    {
        public double Weight;
        public PointD Position, Acceleration, Speed;
        public Planet() { }
        public Planet(double mass, PointD Pos, double SX, double SY)
        {
            Weight = mass;
            Speed = new PointD(SX, SY);
            Position = Pos;
        }
    }
    public class Line
    {
        public Point Begin, End;
        public Line() { }
        public Line(Point A, Point B)
        {
            Begin = A;
            End = B;
        }
    }
    public class PointD
    {
        public double X, Y;
        public PointD(double a, double b)
        {
            X = a;
            Y = b;
        }
        public static PointD operator *(double c1, PointD c2)
        {
            return new PointD(c1 * c2.X, c1 * c2.Y);
        }
        public static PointD operator +(PointD c1, PointD c2)
        {
            return new PointD(c1.X + c2.X, c1.Y + c2.Y);
        }
    }
}
