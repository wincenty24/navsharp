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
        public int validate_points_and_template(ref double[] points)
        {

            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];

            double[] validated_points = new double[4];
            int fun_up_down_const = 0;  // 0 rośnie, 1 maleje, 2 stała pionowo, 3 stała poziomo if == 4 to jest ch*jowo bo są takie same punkty


            if ((b1 < b2) && (a1 != a2)){
                Debug.WriteLine("1");
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
            }

            else if ((b1 > b2) && (a1 != a2)){
                Debug.WriteLine("2");
                validated_points[0] = a2;
                validated_points[1] = b2;
                validated_points[2] = a1;
                validated_points[3] = b1;
            }

            else if ((b1 == b2) && (a1 != a2)){
                Debug.WriteLine("3");
                fun_up_down_const = 2;
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
            }

            else if ((a1 > a2) && (b1 != b2))
            {
                Debug.WriteLine("4");
                fun_up_down_const = 1;
            }

            else if ((a1 < a2) && (b1 != b2))
            {
                Debug.WriteLine("5");
                fun_up_down_const = 0;
            }

            else if ((a1 == a2) && (b1 != b2))
            {
                Debug.WriteLine("6");
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
                fun_up_down_const = 3;
            }
            
            else if ((a1 == a2) && (b1 == b2)){
                Debug.WriteLine("7");
                fun_up_down_const = 4;
            }

            for(int i=0; i<4; i++){
                points[i] = validated_points[i];
            }

            Debug.WriteLine($"a1:{a1}, points:{points[0]}  a2:{a2}, points:{points[2]} {(a1 > a2) && (b1 != b2)} ");
            return fun_up_down_const;
        }

        public static void validate_points(ref double[] points)
        {
            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];

            if ((b1 < b2) && (a1 != a2))//to można usunąć
            {
                Debug.WriteLine("1");
            }
            else if ((b1 > b2) && (a1 != a2))
            {
                Debug.WriteLine("2");
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
                 * |
                 * |
                 * | a1
                 */
            }
        }
        public static int validate_template(double[] points)
        {
            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];
            if ((a1 < a2) && (b1 != b2))
            {
                Debug.WriteLine("1");
                return 0;
            }
            else if ((a1 > a2) && (b1 != b2))
            {
                Debug.WriteLine("2");
                return 1;
            }
            else if ((a1 != a2) && (b1 == b2))
            {
                Debug.WriteLine("3");
                return 2;
            }
            else if ((a1 == a2) && (b1 < b2))
            {
                return 3;
            }
            return 4;
        }
        public static double template_for_growing(double b,double b1) {
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


    }
}
