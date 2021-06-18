using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Math_Formulas
    {
        public double calculate_b(double[] main_points)
        {
            double a1 = main_points[0];
            double b1 = main_points[1];
            double a2 = main_points[2];
            double b2 = main_points[3];
            double delta = b2 - b1;
            double val1 = Math.Tan(a1) / Math.Tan(a2);
            double val2 = (1 / (val1 * Math.Sin(delta))) - (Math.Cos(delta) / Math.Sin(delta));
            double result = (Math.PI / 2) - Math.Atan(val2);
  
            return result;
        }
        public double length_angle_sift(double l, double R )
        {
            return l/R;
        }
        public double length_sift(double angle, double R)
        {
            return angle * R;
        }
        public double shift(double b, double angle)
        {
            return Math.Atan2(Math.Atan(b), Math.Cos(angle));
        }
        public double real_shift(double c, double angle)
        {
            return Math.Atan(Math.Cos(angle)*Math.Tan(c));
        }
        public double calculate_alpha(double a, double b)
        {
            return Math.Atan2(Math.Tan(a), Math.Sin(b));
        }

        public double calculate_a(double alpha, double b) {
            return Math.Atan(Math.Tan(alpha) * Math.Sin(b));
        }

        public double degree_to_radian(double degree)
        {
            return (degree * Math.PI) / 180; 
        }

        public double radian_to_degree(double radian)
        {
            return (radian * 180) / Math.PI;
        }





    }
}
