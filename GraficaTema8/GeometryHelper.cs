using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficaTema8
{
    public class ConvexPolygon2D
    {
        public Color color;

        public List<Point2D> Corners;

        public ConvexPolygon2D(List<Point2D> corners)
        {
            Corners = new List<Point2D>(corners);
            color = Color.FromArgb(Engine.rnd.Next(255), Engine.rnd.Next(255), Engine.rnd.Next(255));
        }
    }

    public class Point2D
    {
        public float X;
        public float Y;
        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Point2D(Point2D point2D)
        {
            X = point2D.X;
            Y = point2D.Y;
        }
    }

    public class GeometryHelper
    {
        const double EquityTolerance = 0.000000001;

        private static bool IsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) <= EquityTolerance;
        }

        public virtual Point2D GetIntersectionPoint(Point2D l1p1, Point2D l1p2, Point2D l2p1, Point2D l2p2)
        {
            double A1 = l1p2.Y - l1p1.Y;
            double B1 = l1p1.X - l1p2.X;
            double C1 = A1 * l1p1.X + B1 * l1p1.Y;

            double A2 = l2p2.Y - l2p1.Y;
            double B2 = l2p1.X - l2p2.X;
            double C2 = A2 * l2p1.X + B2 * l2p1.Y;

            double det = A1 * B2 - A2 * B1;
            if (IsEqual(det, 0))
            {
                return null;
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;
                bool online1 = ((Math.Min(l1p1.X, l1p2.X) < x || IsEqual(Math.Min(l1p1.X, l1p2.X), x))
                    && (Math.Max(l1p1.X, l1p2.X) > x || IsEqual(Math.Max(l1p1.X, l1p2.X), x))
                    && (Math.Min(l1p1.Y, l1p2.Y) < y || IsEqual(Math.Min(l1p1.Y, l1p2.Y), y))
                    && (Math.Max(l1p1.Y, l1p2.Y) > y || IsEqual(Math.Max(l1p1.Y, l1p2.Y), y))
                    );
                bool online2 = ((Math.Min(l2p1.X, l2p2.X) < x || IsEqual(Math.Min(l2p1.X, l2p2.X), x))
                    && (Math.Max(l2p1.X, l2p2.X) > x || IsEqual(Math.Max(l2p1.X, l2p2.X), x))
                    && (Math.Min(l2p1.Y, l2p2.Y) < y || IsEqual(Math.Min(l2p1.Y, l2p2.Y), y))
                    && (Math.Max(l2p1.Y, l2p2.Y) > y || IsEqual(Math.Max(l2p1.Y, l2p2.Y), y))
                    );

                if (online1 && online2)
                    return new Point2D((float)x, (float)y);
            }
            return null;
        }


        public bool InPolygon(Point2D test, ConvexPolygon2D poly)
        {
            Point2D auxA = new Point2D(test);
            Point2D auxB = new Point2D(float.MaxValue,test.Y);
            
            return GetIntersectionPoints(auxA, auxB, poly).Count % 2 == 1;
        }

        public virtual List<Point2D> GetIntersectionPoints(Point2D l1p1, Point2D l1p2, ConvexPolygon2D poly)
        {
            List<Point2D> intersectionPoints = new List<Point2D>();
            for (int i = 0; i < poly.Corners.Count; i++)
            {
                Point2D ip = GetIntersectionPoint(l1p1, l1p2, poly.Corners[i], poly.Corners[(i+1)%poly.Corners.Count]);

                if (ip != null)
                {
                    intersectionPoints.Add(ip);
                }

            }

            return intersectionPoints.ToList();
        }

        private void AddPoints(List<Point2D> pool, List<Point2D> newPoints)
        {
            foreach (Point2D np in newPoints)
            {
                bool found = false;
                foreach (Point2D p in pool)
                {
                    if (IsEqual(p.X, np.X) && IsEqual(p.Y, np.Y))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) { pool.Add(np); }
            }
        }

        public bool ToTheLeft(Point2D a, Point2D b, Point2D c)
        {
            float value = ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X));

            if (value > 0) { return true; }
            else { return false; }
        }

        public List<Point2D> OrderClockwise(List<Point2D> points)
        {
            for (int i = 1; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (!ToTheLeft(points[0], points[i], points[j]))
                    {
                        Point2D aux =new Point2D(points[i]);
                        points[i] = new Point2D(points[j]);
                        points[j] = new Point2D(aux);
                    }
                }
            }

            return points;
        }

        public ConvexPolygon2D MyPolyAlg(ConvexPolygon2D basePoly, ConvexPolygon2D clippingPoly)
        {
            List<Point2D> clippedCorners = new List<Point2D>();
    
            for (int i = 0; i < basePoly.Corners.Count; i++)
            {
                if (InPolygon(basePoly.Corners[i], clippingPoly))
                {
                    AddPoints(clippedCorners, new List<Point2D>() { basePoly.Corners[i] });
                }
            }

            for (int i = 0; i < clippingPoly.Corners.Count; i++)
            {
                if (InPolygon(clippingPoly.Corners[i], basePoly))
                {
                    AddPoints(clippedCorners, new List<Point2D>() { clippingPoly.Corners[i] });
                }
            }

            for (int i = 0; i < basePoly.Corners.Count; i++)
            {
                AddPoints(clippedCorners, GetIntersectionPoints(basePoly.Corners[i], basePoly.Corners[(i + 1) % basePoly.Corners.Count], clippingPoly));
            }

            return new ConvexPolygon2D(OrderClockwise(clippedCorners.ToList()));
        }


        public ConvexPolygon2D SutherlandHodgmanAlg(ConvexPolygon2D basePoly, ConvexPolygon2D clipingPoly)
        {
            bool ok = true;
            for (int i = 0; i < clipingPoly.Corners.Count; i++)
            {
                if (!InPolygon(clipingPoly.Corners[i], basePoly) && !InPolygon(clipingPoly.Corners[(i+1)%clipingPoly.Corners.Count], basePoly) && 
                    GetIntersectionPoints(clipingPoly.Corners[i], clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count],basePoly)!= null)
                {
                    ok = false;
                    break;
                }
            }
            if (ok)
            {
                ConvexPolygon2D intersectedPoly = new ConvexPolygon2D(new List<Point2D>());

                for (int i = 0; i < clipingPoly.Corners.Count; i++)
                {
                    if (!InPolygon(clipingPoly.Corners[i], basePoly) && InPolygon(clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count], basePoly))
                    {
                        Point2D intersectionPoint = GetIntersectionPoints(clipingPoly.Corners[i], clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count], basePoly)[0];
                        intersectedPoly.Corners.Add(intersectionPoint);
                        intersectedPoly.Corners.Add(clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count]);
                    }
                    else if(InPolygon(clipingPoly.Corners[i],basePoly) && InPolygon(clipingPoly.Corners[(i+1)%clipingPoly.Corners.Count],basePoly))
                    {
                        intersectedPoly.Corners.Add(clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count]);
                    }
                    else if(InPolygon(clipingPoly.Corners[i],basePoly) && !InPolygon(clipingPoly.Corners[(i+1)%clipingPoly.Corners.Count],basePoly))
                    {
                        Point2D intersectionPoint = GetIntersectionPoints(clipingPoly.Corners[i], clipingPoly.Corners[(i + 1) % clipingPoly.Corners.Count], basePoly)[0];
                        intersectedPoly.Corners.Add(intersectionPoint);
                    }
                }

                for(int i=0;i<basePoly.Corners.Count;i++)
                {
                    if(InPolygon(basePoly.Corners[i],clipingPoly))
                    {
                        intersectedPoly.Corners.Add(basePoly.Corners[i]);
                    }
                }

                return new ConvexPolygon2D(OrderClockwise(intersectedPoly.Corners));
            }
            else
            {
                return MyPolyAlg(basePoly, clipingPoly);
            }
        }
    }
}
