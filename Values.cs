using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    public static class Values
    {
        //const
        public const double earth_radius_km = 6371.0;
        public const double earth_radius_m = 6371000;
        public const double earth_radius_cm = 637100000;

        //values for users
        public static double shif_m = 3.0;
        public static double line_distance_m = 100;//in meters
        public static int how_many_lines = 50;
        public static double look_ahead_m = 1.0f;
        public static double machine_length = 3.0;


        public static double asl = 0;//abve sea level
        public static double object_azimute_degree = 0.0;
        public static double compass = 0;
        public static double tracking_move = 0;
        public static double angle = 0;
        public static double distance = 0;
        public static bool centerize = true;


        //arrays
        public static double[] main_points = new double[4];
        public static double[] current_point = { Math_Formulas.degree_to_radian(51.205802), Math_Formulas.degree_to_radian(17.312763) };// new double[2];
        

        public enum Direction
        {
            growing = 0,
            decreasing = 1,
        }
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
