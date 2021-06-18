﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Templates
    {
        public double validate_points_and_template(ref double[] points)
        {

            double a1 = points[0];
            double b1 = points[1];
            double a2 = points[2];
            double b2 = points[3];

            double[] validated_points = new double[4];
            int fun_up_down_const = 0;  // 0 rośnie, 1 maleje, 2 stała pionowo, 3 stała poziomo if == 4 to jest ch*jowo bo są takie same punkty


            if ((b1 < b2) && (a1 != a2)){
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
            }

            else if ((b1 > b2) && (a1 != a2)){
                validated_points[0] = a2;
                validated_points[1] = b2;
                validated_points[2] = a1;
                validated_points[3] = b1;
            }

            else if ((b1 == b2) && (a1 != a2)){
                fun_up_down_const = 2;
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
            }

            else if ((a1 > a2) && (b1 != b2))
            {
                fun_up_down_const = 1;
            }

            else if ((a1 < a2) && (b1 != b2))
            {
                fun_up_down_const = 0;
            }

            else if ((a1 == a2) && (b1 != b2))
            {
                validated_points[0] = a1;
                validated_points[1] = b1;
                validated_points[2] = a2;
                validated_points[3] = b2;
                fun_up_down_const = 3;
            }
            
            if ((a1 == a2) && (b1 == b2)){
                fun_up_down_const = 4;
            }

            for(int i=0; i<4; i++){
                points[i] = validated_points[i];
            }
            
            
            return fun_up_down_const;
        }


        public double template_for_one(double b,double b1) {
            double wynik = 0;
            if (b > b1) {//  # wzorzec nr 1a
                wynik = b - b1;
                wynik = Math.Abs(wynik);

            }
            if (b == b1) {  //# wzorzec nr 1b
                wynik = 0;
                            }
            if (b < b1) {  //# wzorzec nr 1c
                wynik = -(b1 - b);
               }
            return wynik;
        }

    }
}