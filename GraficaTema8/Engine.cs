using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficaTema8
{
    public static class Engine
    {
        public static List<ConvexPolygon2D> polygons = new List<ConvexPolygon2D>();
        public static GeometryHelper geometryHelper = new GeometryHelper();
        public static Random rnd = new Random();
    }
}
