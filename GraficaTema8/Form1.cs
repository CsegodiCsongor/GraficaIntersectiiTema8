using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraficaTema8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<Point2D> notDrawnPoints = new List<Point2D>();

        private void button1_Click(object sender, EventArgs e)
        {
            Engine.polygons.Add(new ConvexPolygon2D(Engine.geometryHelper.OrderClockwise(notDrawnPoints)));
            notDrawnPoints = new List<Point2D>();

            DrawEngine.DrawPolygon(Engine.polygons[Engine.polygons.Count - 1]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DrawEngine.DrawIntersection();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            Point2D clickedPoint = new Point2D(me.X, me.Y);

            notDrawnPoints.Add(clickedPoint);
            DrawEngine.DrawPoint(clickedPoint, 5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DrawEngine.Init(pictureBox1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DrawEngine.ClearCanvas();
        }
    }
}
