using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Templates
    {
      
        public static void validate_points(ref double[] points)
        {
            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];

            if ((b1 > b2) && (a1 != a2))
            {
                points[0] = a2;
                points[1] = b2;
                points[2] = a1;
                points[3] = b1;
            }
            else if ((b1 == b2) && (a1 > a2))
            {
                points[0] = a2;
                points[2] = a1;
                /*
                 * | a2
                 * |
                 * |
                 * |
                 * |
                 * | a1
                 */
            }
            else if ( (a1 == a2) && (b1 > b2))
            {
                points[1] = b2;
                points[3] = b1;
                /*
                 * b1----------------b2
                 */
            }
        }
        public static Values.Function validate_template(double[] points)
        {
            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];

            if ((a1 < a2) && (b1 != b2))
            {
                return Values.Function.growing;
            }
            else if ((a1 > a2) && (b1 != b2))
            {
                return Values.Function.decreasing;
            }
            else if ((a1 != a2) && (b1 == b2))
            {
                return Values.Function.vertical;
            }
            else if ((a1 == a2) && (b1 < b2))
            {
                return Values.Function.perpendicular;
            }
            return Values.Function.NULL;
        }
        public static double template_for_growing(double b, double b1) {
            double wynik = 0;
            if (b > b1) {//  # wzorzec nr 1a
                wynik = b - b1;
                wynik = Math.Abs(wynik);

            }
            else if (b == b1) {  //# wzorzec nr 1b
                wynik = 0;
                            }
            else if (b < b1) {  //# wzorzec nr 1c
                wynik = -(b1 - b);
               }
            return wynik;
        }
        public static Values.Direction chceck_direction_growing(double current_degree, double ref_degree)
        {
            if (current_degree >= Math_Formulas.prepare_degree_for_compass(ref_degree - Math.PI / 2) || current_degree <= Math.PI / 2 + Math_Formulas.prepare_degree_for_compass(ref_degree))
            {
                return Values.Direction.growing;
            }
            else if (current_degree < Math_Formulas.prepare_degree_for_compass(ref_degree - Math.PI / 2) && current_degree > Math.PI / 2 + Math_Formulas.prepare_degree_for_compass(ref_degree))
            {
                return Values.Direction.decreasing;
            }
            return Values.Direction.NULL;
        }
        public static Values.Direction chceck_direction_decreasing(double current_degree, double ref_degree)
        {
            if (current_degree >= Math_Formulas.prepare_degree_for_compass(-Math.PI / 2 - ref_degree) || current_degree <= Math.PI / 2 - ref_degree)
            {
                return Values.Direction.growing;
            }
            else if (current_degree < Math_Formulas.prepare_degree_for_compass(-Math.PI / 2 - ref_degree) && current_degree > Math.PI / 2 - ref_degree)
            {
                return Values.Direction.decreasing;
            }
            return Values.Direction.NULL;
        }
        public static Values.Direction chceck_direction_vertical(double current_degree, double ref_degree)
        {
            if (current_degree >= 1.5 * Math.PI || current_degree <= Math.PI / 2)
            {
                return Values.Direction.growing;

            }
            else if (current_degree < 1.5 * Math.PI && current_degree > Math.PI / 2)
            {
                return Values.Direction.decreasing;
            }
            return Values.Direction.NULL;

        }
        public static Values.Direction chceck_direction_perpendicular(double current_degree, double ref_degree)
        {
            if (current_degree >= Math.PI)
            {
                return Values.Direction.decreasing;

            }
            else if (current_degree < Math.PI)
            {
                return Values.Direction.growing;
            }
            return Values.Direction.NULL;
        }

        public static Values.Direction chceck_direction(Values.Function function, double current_degree, double ref_degree)
        {
            if (function == Values.Function.growing)
            {
                if (current_degree >= Math_Formulas.prepare_degree_for_compass(ref_degree - Math.PI / 2) || current_degree <= Math.PI / 2 + Math_Formulas.prepare_degree_for_compass(ref_degree))
                {
                    return Values.Direction.growing;
                }
                else if (current_degree < Math_Formulas.prepare_degree_for_compass(ref_degree - Math.PI / 2) && current_degree > Math.PI / 2 + Math_Formulas.prepare_degree_for_compass(ref_degree))
                {
                    return Values.Direction.decreasing;
                }
            }
            else if (function == Values.Function.decreasing)
            {
                if (current_degree >= Math_Formulas.prepare_degree_for_compass(-Math.PI / 2 - ref_degree) || current_degree <= Math.PI / 2 - ref_degree)
                {
                    return Values.Direction.growing;
                }
                else if (current_degree < Math_Formulas.prepare_degree_for_compass(-Math.PI / 2 - ref_degree) && current_degree > Math.PI / 2 - ref_degree)
                {
                    return Values.Direction.decreasing;
                }
            }
            else if (function == Values.Function.vertical)
            {
                if (current_degree >= 1.5*Math.PI || current_degree <= Math.PI / 2)
                {
                    return Values.Direction.growing;

                }
                else if (current_degree < 1.5 * Math.PI && current_degree > Math.PI / 2)
                {
                    return Values.Direction.decreasing;
                }
            }
            else if(function == Values.Function.perpendicular) {
                if (current_degree >= Math.PI)
                {
                    return Values.Direction.decreasing;
                 
                }
                else if (current_degree < Math.PI)
                {
                    return Values.Direction.growing;
                }
            }
           
            return 0;
        }
    }
}
