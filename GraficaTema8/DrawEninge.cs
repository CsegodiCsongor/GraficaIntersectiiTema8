using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraficaTema8
{
    public static class DrawEngine
    {
        public static Bitmap bmp;
        public static Graphics grp;
        public static PictureBox canvas;

        public static Pen myPen;
        public static int PenWidth = 3;

        public static void Init(PictureBox pictureBox)
        {
            canvas = pictureBox;
            bmp = new Bitmap(canvas.Width, canvas.Height);
            grp = Graphics.FromImage(bmp);
            myPen = new Pen(Color.Black, PenWidth);
        }

        public static void DrawPoint(Point2D point2D, int lineSize)
        {
            grp.DrawLine(myPen, point2D.X - lineSize, point2D.Y - lineSize, point2D.X + lineSize, point2D.Y + lineSize);
            grp.DrawLine(myPen, point2D.X + lineSize, point2D.Y - lineSize, point2D.X - lineSize, point2D.Y + lineSize);

            canvas.Image = bmp;
        }


        public static void FillPoly(ConvexPolygon2D polygon, Point2D currentPoint)
        {
            List<Point2D> points = new List<Point2D>();
            points.Add(currentPoint);
            while(points.Count != 0)
            {
                currentPoint = new Point2D(points[0]);
                points.RemoveAt(0);

                bmp.SetPixel((int)currentPoint.X, (int)currentPoint.Y, polygon.color);

                if(bmp.GetPixel((int)currentPoint.X + 1, (int)currentPoint.Y)!= polygon.color && !points.Contains(new Point2D(currentPoint.X + 1, currentPoint.Y)))
                {
                    points.Add(new Point2D(currentPoint.X + 1, currentPoint.Y));
                }
                if (bmp.GetPixel((int)currentPoint.X - 1, (int)currentPoint.Y) != polygon.color && !points.Contains(new Point2D(currentPoint.X - 1, currentPoint.Y)))
                {
                    points.Add(new Point2D(currentPoint.X - 1, currentPoint.Y));
                }
                if (bmp.GetPixel((int)currentPoint.X, (int)currentPoint.Y + 1) != polygon.color && !points.Contains(new Point2D(currentPoint.X, currentPoint.Y + 1)))
                {
                    points.Add(new Point2D(currentPoint.X, currentPoint.Y + 1));
                }
                if (bmp.GetPixel((int)currentPoint.X, (int)currentPoint.Y - 1) != polygon.color && !points.Contains(new Point2D(currentPoint.X, currentPoint.Y - 1)))
                {
                    points.Add(new Point2D(currentPoint.X, currentPoint.Y - 1));
                }
            }
        }

        private static void FillPolygonLeft(ConvexPolygon2D polygon, Point2D currentPoint)
        {
            if(bmp.GetPixel((int)currentPoint.X, (int)currentPoint.Y) != polygon.color)
            {
                bmp.SetPixel((int)currentPoint.X, (int)currentPoint.Y, polygon.color);
                FillPolygonLeft(polygon, new Point2D((int)currentPoint.X - 1, (int)currentPoint.Y));
                FillPolygonLeft(polygon, new Point2D((int)currentPoint.X, (int)currentPoint.Y + 1));
                FillPolygonLeft(polygon, new Point2D((int)currentPoint.X, (int)currentPoint.Y - 1));
            }
        }

        private static void FillPolygonRight(ConvexPolygon2D polygon, Point2D currentPoint)
        {
            if (bmp.GetPixel((int)currentPoint.X, (int)currentPoint.Y) != polygon.color)
            {
                bmp.SetPixel((int)currentPoint.X, (int)currentPoint.Y, polygon.color);
                FillPolygonRight(polygon, new Point2D((int)currentPoint.X + 1, (int)currentPoint.Y));
                FillPolygonRight(polygon, new Point2D((int)currentPoint.X, (int)currentPoint.Y + 1));
                FillPolygonRight(polygon, new Point2D((int)currentPoint.X, (int)currentPoint.Y - 1));
            }
        }

        public static void DrawPolygon(ConvexPolygon2D polygon)
        {
            Pen auxPen = new Pen(polygon.color, PenWidth);

            for (int i = 0; i < polygon.Corners.Count; i++)
            {
                BresenhamLine((int)polygon.Corners[i].X, (int)polygon.Corners[i].Y, (int)polygon.Corners[(i + 1) % polygon.Corners.Count].X, (int)polygon.Corners[(i + 1) % polygon.Corners.Count].Y, polygon.color);
            }

            if (polygon.Corners.Count > 0)
            {
                Point2D pointInsidePoly = new Point2D(polygon.Corners[0].X, polygon.Corners[0].Y);

                for(int i = 1; i< polygon.Corners.Count;i++)
                {
                    pointInsidePoly.X += polygon.Corners[i].X;
                    pointInsidePoly.Y += polygon.Corners[i].Y;
                }
                pointInsidePoly.X /= polygon.Corners.Count;
                pointInsidePoly.Y /= polygon.Corners.Count;

                //DrawPoint(pointInsidePoly, 15);

                FillPolygonRight(polygon, pointInsidePoly);
                pointInsidePoly.X -= 1;
                FillPolygonLeft(polygon, pointInsidePoly);
                //FillPoly(polygon, pointInsidePoly); //Too Slow...
            }
            canvas.Image = bmp;
        }

        public static void DrawIntersection()
        {
            for(int i=0;i<Engine.polygons.Count - 1;i++)
            {
                for(int j=i+1;j<Engine.polygons.Count;j++)
                {
                    DrawPolygon(Engine.geometryHelper.SutherlandHodgmanAlg(Engine.polygons[i], Engine.polygons[j]));
                }
            }
        }


        public static void BresenhamLine(int x, int y, int x2, int y2, Color color)
        {
            //Bresenham's line-algorith
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest * 2;
            for (int i = 0; i <= longest; i++)
            {
                bmp.SetPixel(x, y, color);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public static void ClearCanvas()
        {
            bmp = new Bitmap(canvas.Width, canvas.Height);
            grp = Graphics.FromImage(bmp);
            canvas.Image = bmp;
            Engine.polygons = new List<ConvexPolygon2D>();
        }
    }
}
