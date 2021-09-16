using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Values
    {
        //const
        public const double earth_radius_km = 6371.0;
        public const double earth_radius_m = 6371000;
        public const double earth_radius_cm = 637100000;


        public double asl = 0;//abve sea level
        public double shif_m = 3.0;
        public double line_distance_m = 100;//in meters
        public double how_many_lines = 20;

        //arrays
        public double[] main_points = new double[4];
  
       public enum Function
       {
            growing = 0,
            decreasing = 1,
            vertical = 2,//pionowe
            perpendicular = 3,//perpendicular

        }
        public struct Value_For_Vertical_Fun
        {
            public double reference_b;

        };
        public struct Value_For_Perpendicular_Fun
        {
            public double reference_a;

        };
        public struct Value_For_Rise_Fun
        {
            public double validated_function;
            public double alpha;
            public double beta;
            public double delta;
        };
        public struct Value_For_Decreasing_Fun
        {
            public double alpha;
            public double beta;
            public double b_length;
        };
    }
}
