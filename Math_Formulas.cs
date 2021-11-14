using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Math_Formulas
    {
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         *                 ##
         *                # #
         *               #  #
         *              #   #
         *             #    #
         *            #     #
         *      c    #      #
         *          #       #
         *         #        #
         *        #         #      a
         *       #          #
         *      #           #
         *     #            #
         *    #             #
         *   #              #
         *  #  A/alpha      #
         * ##################
                    b         
         
         
         */      
        




        



        public static double calculate_b(double[] main_points)
        {
            double a1 = main_points[0];
            double b1 = main_points[1];
            double a2 = main_points[2];
            double b2 = main_points[3];
            double delta = b2 - b1;
            Debug.WriteLine($"data {delta}");
            double val1 = Math.Tan(a1) / Math.Tan(a2);
            double val2 = (1 / (val1 * Math.Sin(delta))) - (Math.Cos(delta) / Math.Sin(delta));
            double result = (Math.PI / 2) - Math.Atan(val2);
  
            return result;
        }
        public static double calculate_decreasing_b2(double[] main_points)
        {
            double a1 = main_points[0];
            double b1 = main_points[1];
            double a2 = main_points[2];
            double b2 = main_points[3];

            double delta = Math.Abs(b2 - b1);
            double f = Math.Sin(delta);
            double e = Math.Cos(delta);
            double val1 = Math.Tan(a1) / Math.Tan(a2);
            double val2 = (val1 - e) / f;
            double result = (Math.PI / 2) - Math.Atan(val2);

            return result;
        }
        public static double[] shift_up(double beta,double a)
        {
            double alpha = (Math.PI / 2) - beta;
            double c = Math.Asin(Math.Sin(a) / Math.Sin(alpha));
            double b = Math.Atan(Math.Cos(alpha) * Math.Tan(c));
            double d = Math.Atan(Math.Cos(beta) * Math.Atan(b));
            double e = Math.Asin(Math.Sin(beta) * Math.Sin(b));
            double g = c - e;
            double[] shifts = new double[] { d, g };

            return shifts;
        }
        public static double[] shift_down(double alpha, double b, double d)
        {
            double beta = Math.Acos(Math.Sin(alpha)*Math.Cos(b));
            double sigma = Math.PI / 2 - beta;
            double e = Math.Atan(Math.Cos(sigma)*Math.Tan(d));
            double f = Math.Sin(Math.Sin(sigma)*Math.Sin(d));
            double[] shifts = new double[] { f, e };

            return shifts;
        }
        public static double length_angle_sift(double l, double R )
        {
            return l/R;
        }
        public static double length_sift(double angle, double R)
        {
            return angle * R;
        }
        public static double shift(double b, double angle)
        {
            return Math.Atan2(Math.Atan(b), Math.Cos(angle));
        }
        public static double[] calculate_shift_for_growing_fun_down(double a, double b, double shift, double alpha, double beta, double delta)
        {
            double c = calculate_c_using_a_and_b(a, b + delta);
            double b1 = Math.Atan(Math.Tan(beta) * Math.Sin(shift));
            double c2 = c - b1;
            double a2 = Math.Asin(Math.Sin(alpha) * Math.Sin(c2));
            double a3 = Math.Asin(Math.Tan(shift) / Math.Tan(beta));
            double c3 = c + a3;
            double b2 = Math.Atan(Math.Cos(alpha) * Math.Tan(c3)) - delta;
            double[] returner = {a2, b2}; 
            return returner;
        }
        public static double[] calculate_shift_for_growing_fun_up(double a, double b, double shift, double alpha, double beta, double delta)
        {
            double b1 = calculate_b_using_a_and_alpha(shift, beta);
            double c = calculate_c_using_a_and_b(a, b + delta);
            double c1 = c - b1;
            double c2 = calculate_c_using_a_and_b(shift, b1);
            double b2 = Math.Atan(Math.Cos(alpha) * Math.Tan(c1)) - delta;
            double a2 = Math.Asin(Math.Sin(alpha) * Math.Sin(c1));
            double[] returner = { a2 + c2, b2 };
            return returner;
        }
        public static double[] calculate_shift_for_decreasing_fun_down(double a, double b, double shift, double alpha, double beta, double b_length)
        {
            double b1 = Math.Atan(Math.Tan(beta) * Math.Sin(shift));
            double c = calculate_c_using_a_and_b(a, b_length - b) ;
            double c1 = c - b1;
            double a2 = Math.Asin(Math.Sin(alpha) * Math.Sin(c1));
            double a3 = Math.Asin(Math.Tan(shift) / Math.Tan(beta));
            double c3 = c + a3;
            double b2 = Math.Atan(Math.Cos(alpha) * Math.Tan(c3));
            double[] returner = { a2, b_length - (b2) };
            return returner;
        }
        public static double[] calculate_shift_for_decreasing_fun_up(double a, double b, double shift, double alpha, double beta, double b_length)
        {
            double b1 = calculate_b_using_a_and_alpha(shift, beta);
            double c = calculate_c_using_a_and_b(a, b_length - b);
            double c1 = c - b1;
            double c2 = calculate_c_using_a_and_b(shift, b1);
            double b2 = Math.Atan(Math.Cos(alpha) * Math.Tan(c1));
            double a2 = Math.Asin(Math.Sin(alpha) * Math.Sin(c1));
            double[] returner = { a2 + c2, b_length - (b2) };
            return returner;
        }
        public static double[] real_shift_growing(double curr_loc_a, double curr_loc_b, double alpha, double beta, double delta)
        {
            double a = calculate_a(alpha, delta + curr_loc_b);
            double return_a = curr_loc_a, return_b = curr_loc_b, return_c = 0, return_shift = 0;
            if (curr_loc_a > a)
            {
                double a1 = curr_loc_a - a;
                a1 = Math.Abs(a1);
                double b2 = Math.Atan(Math.Cos(beta) * Math.Tan(a1));
                double c = Math.Acos(Math.Cos(a) * Math.Cos(curr_loc_b + delta));
                double c2 = c + b2;
                double a3 = Math.Asin(Math.Sin(alpha) * Math.Sin(c2));
                double b3 = Math.Atan(Math.Cos(alpha) * Math.Tan(c2)) - delta;
                return_a = a3;
                return_b = b3;
             
                double radian_distance = distance_radian(curr_loc_a, a3, Math.Abs(curr_loc_b - b3));
                //return_shift = Math.Asin(Math.Sin(beta) * Math.Sin(a1));
                return_shift = radian_distance;
                return_c = c2;
            }

            else if (curr_loc_a < a)
            {
                
                double bb1 = calculate_b_using_a_and_alpha(curr_loc_a, alpha) - delta;
                double b1 = bb1 - curr_loc_b;
                b1 = Math.Abs(b1);                                                                                                                                                                                                      
                double b2 = Math.Atan(Math.Cos(alpha) * Math.Tan(b1));
                double shift = Math.Asin(Math.Tan(b2) / Math.Tan(beta));
                double c = Math.Acos(Math.Cos(curr_loc_a) * Math.Cos(delta + bb1));
                double c2 = c + b2;
                double a3 = Math.Asin(Math.Sin(alpha) * Math.Sin(c2));
                double b3 = Math.Atan(Math.Cos(alpha) * Math.Tan(c2)) - delta;
                
                double radian_distance = distance_radian(curr_loc_a, a3, Math.Abs(curr_loc_b - b3));
             
                return_a = a3;
                return_b = b3;
                //double zdupya = Math.Acos(Math.Cos(chujowe_a) * Math.Cos(chuhowe_b));
              
                return_shift = radian_distance;
                return_c = c2;
            }
            else if (curr_loc_a == a)
            {

            }
 
            double[] points = { return_a, return_b, return_c, return_shift };
            
            return points;
        }
        public static double[] calculate_shift_for_vertical(double curr_loc_a, double curr_loc_b, double reference_b, double d, double r)
        {
            //double dis = curr_loc_b - reference_b;//distance_radian(curr_loc_a, curr_loc_a, curr_loc_b - reference_b);
            double val4 = Math.Pow(Math.Sin(d/(2*r)), 2);
            double val5 = Math.Pow(Math.Cos(curr_loc_a), 2);
            double val6 = Math.Sqrt(val4 / val5);
            double val7 = 2 * Math.Asin(val6);
            double val8 = val7 + reference_b;
            double val9 = reference_b - val7;
            double[] returner = { val8, val9, val7 };
            return returner;
        }
        public static double[] calculate_shift_for_perpendicular(double curr_loc_a, double curr_loc_b, double reference_a, double d, double r)
        {
            //double dis = curr_loc_b - reference_b;//distance_radian(curr_loc_a, curr_loc_a, curr_loc_b - reference_b);
            double val1 = Math.Sin(d / (2 * r));
            double val2 = 2*Math.Asin(val1);
            double val3 = val2 + curr_loc_a;
            double[] returner = { val3, val2 };
            return returner;
        }

        public static double real_shift_vertical(double curr_loc_a, double curr_loc_b, double reference_b, double r)
        {
            double val1 = Math.Pow(Math.Cos(curr_loc_a), 2);
            double val2 = Math.Pow(Math.Sin((curr_loc_b - reference_b) / 2), 2);
            double val3 = Math.Sqrt(val1 * val2);
            double e = 2 * r * Math.Asin(val3);
            return e;
        }
        public static double real_shift_perpendicular(double curr_loc_a, double curr_loc_b, double reference_a, double r)
        {
            double val1 = Math.Sin((reference_a - curr_loc_a)/2);
            double val2 = Math.Asin(val1);
            double val3 = 2 * r * val2;
            return val3;
        }
        public static double[] real_shift_decreasing(double curr_loc_a, double curr_loc_b, double alpha, double beta, double b_length)
        {
           
            
            double a = Math.Atan(Math.Tan(alpha) * Math.Sin(b_length - curr_loc_b));
            double b = calculate_b_using_a_and_alpha(curr_loc_a, alpha);
            double return_a = curr_loc_a, return_b = curr_loc_b, return_c = 0, return_shift = 0;
            if (curr_loc_a > a)
            {
                double a2 = curr_loc_a - a;
                double c1 = Math.Atan(Math.Cos(beta) * Math.Tan(a2));
                double c = calculate_c_using_a_and_b(a, b_length -curr_loc_b);
                double c2 = c + c1 ;
                double a3 = Math.Asin(Math.Sin(alpha) * Math.Sin(c2));
                double b3 = Math.Atan(Math.Cos(alpha) * Math.Tan(c2));
                return_a = a3;
                return_b = b_length - b3;
                double radian_distance = distance_radian(a3, curr_loc_a, Math.Abs(curr_loc_b - b3));
                return_shift = Math.Asin(Math.Sin(beta) * Math.Sin(a2));
            }

            else if (curr_loc_a < a)
            {

                double c1 = b_length - b - curr_loc_b;
                double b1 = calculate_b_using_c_and_alpha(c1, alpha);
                double c = calculate_c_using_a_and_b(curr_loc_a, b);
                double c2 = b1 + c;
                double a3 = Math.Asin(Math.Sin(alpha) * Math.Sin(c2));
                double b3 = Math.Atan(Math.Cos(alpha) * Math.Tan(c2));
                return_a = a3;
                return_b = b_length - b3;
                double radian_distance = distance_radian(a3, curr_loc_a, curr_loc_b - b3);
                return_shift = Math.Asin(Math.Tan(b1) / Math.Tan(beta));
            }


            double[] points = { return_a, return_b, return_c, return_shift };

            return points;
        }

        public static double real_shift_down(double a1, double b1, double alpha)
        {
            double a3 = calculate_a(alpha, b1);
            double beta = calculate_beta_using_b_and_alpha(b1, alpha);
            double c = a3 - a1;
            double a = calculate_a_using_c_and_alpha(c ,beta);
            return a;
        }
        public static double calculate_c_using_a_alpha(double a, double alpha)
        {
            return Math.Asin(Math.Sin(a)/ Math.Asin(alpha));
        }
        public static double calculate_alpha(double a, double b)
        {
            return Math.Atan2(Math.Tan(a), Math.Sin(b));
        }

        public static double calculate_a(double alpha, double b) {
            return Math.Atan(Math.Tan(alpha) * Math.Sin(b));
        }

        public static double degree_to_radian(double degree)
        {
            return (degree * Math.PI) / 180; 
        }

        public static double radian_to_degree(double radian)
        {
            return (radian * 180) / Math.PI;
        }

        public static double calculate_b_using_a_and_alpha(double a, double alpha)
        {
            return Math.Asin(Math.Tan(a) / Math.Tan(alpha));
        }

        public static double calculate_beta_using_b_and_alpha(double b, double alpha)//9
        {
            return Math.Acos(Math.Sin(alpha) * Math.Cos(b));
        }
        public static double calculate_beta_using_a_and_b(double a, double b)//5
        {
            return Math.Atan2(Math.Tan(b) , Math.Sin(a));
        }
        public static double calculate_b_using_beta_and_c(double c, double beta)//3
        {
            return Math.Asin(Math.Sin(beta) * Math.Sin(c));
        }

        public static double calculate_b_using_c_and_alpha(double c, double alpha)//3 +1
        {
            return Math.Atan(Math.Cos(alpha) * Math.Tan(c));
        }

        public static double calculate_c_using_a_and_b(double a, double b)
        {
            return Math.Acos(Math.Cos(a) * Math.Cos(b));
        }

        public static double calculate_a_using_b_and_c(double b, double c)
        {
            return Math.Acos(Math.Cos(c) / Math.Cos(b));
        }
        public static double calculate_b_using_a_and_c(double a, double c)
        {
            return Math.Acos(Math.Cos(c) / Math.Cos(a));
        }
        public static double calculate_a_using_c_and_alpha(double c, double alpha)
        {
            return Math.Asin(Math.Sin(alpha) * Math.Sin(c));
        }
        public static double distance_radian(double fi1, double fi2, double lam_1_2)//lam_12= lam2 - lam1;
        {

            double tan = (Math.Sqrt(Math.Pow(Math.Cos(fi1) * Math.Sin(fi2) - Math.Sin(fi1) * Math.Cos(fi2) * Math.Cos(lam_1_2), 2) + Math.Pow(Math.Cos(fi2) * Math.Sin(lam_1_2), 2))) / (Math.Sin(fi1) * Math.Sin(fi2) + Math.Cos(fi1) * Math.Cos(fi2) * Math.Cos(lam_1_2));
            tan = Math.Atan(tan);

            double tan_ = Math.Atan2(Math.Sqrt(Math.Pow(Math.Cos(fi1) * Math.Sin(fi2) - Math.Sin(fi1) * Math.Cos(fi2) * Math.Cos(lam_1_2), 2) + Math.Pow(Math.Cos(fi2) * Math.Sin(lam_1_2), 2)), Math.Sin(fi1) * Math.Sin(fi2) + Math.Cos(fi1) * Math.Cos(fi2) * Math.Cos(lam_1_2));


            //Console.WriteLine("qqqqqq "+tan_);
            return tan;
        }
        public static double distance_m(double dis_rad, double radius)
        {
            return radius*dis_rad;
        }
        public static double calculate_radian_using_radius_and_length(double length, double radius)
        {
            return length / radius;
        }

        public static int which_line(double real_shift, double width)
        {
            return (int)Math.Round(real_shift / width, 0, MidpointRounding.AwayFromZero);
        }
        public static int map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public static float mapf(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public static double mapd(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static double calculate_b_for_the_same_a(double b_2, double a, double distance)
        {
            return b_2 - 2 * Math.Asin(Math.Sin(distance / 2)/ Math.Cos(a));
        }
        public static double calculate_a_for_the_same_b(double a_2, double distance)
        {
            return a_2 + 2 * Math.Asin(Math.Sin(distance/2));
        }

        public static double[] calculate_points_using_beta_angle(double current_a, double current_b, double c, double angle)
        {
            double b = Math.Cos(angle) * c;
            double a = Math.Sin(angle) * c;
            double new_a = calculate_a_for_the_same_b(current_a, a);
            double new_b = calculate_b_for_the_same_a(current_b, current_a, b);

            double[] returner = { new_a, new_b };

            return returner;
        }

        public static double distance_to_line(double curr_diss, double my_distance)
        {
            double half_shift = my_distance / 2;
            double rest = curr_diss % my_distance;
            if (rest >= half_shift)
            {
                double diss_to_line = rest - my_distance ;
                return diss_to_line;
            }
            else if (rest < half_shift)
            {
                return rest;
            }

            return 0;
        }

        public static double calculate_steering(double target_a, double target_b, double current_a, double current_b, double shift_to_line, double ahead, double earth_radius, double vehicle_degree, double look_ahead_m, double ref_degree)
        {
            double distance = Math.Abs(distance_radian(target_a, current_a, target_b - current_b));
            shift_to_line /= earth_radius;
            double L = calculate_radian_using_radius_and_length(look_ahead_m, earth_radius);
            double alpha = (ref_degree - vehicle_degree) - Math.Asin(shift_to_line / (distance));
            double k = (2 * shift_to_line) / Math.Pow(distance, 2);
            double r = Math.Pow(distance, 2) / (2 * shift_to_line);
            double theta = Math.Atan2(2 * L * Math.Sin(alpha), distance);
            //Debug.WriteLine($"{value_for_rise_fun.beta} {Values.compass} alpha {alpha} theta {alpha}");
            //Values.angle = theta;
            return theta;
        }
        public static double prepare_degree_for_compass(double degree)
        {

            degree = degree % (2 * Math.PI);
            if (degree < 0)
            {
                return degree + (2 * Math.PI);
            }
            return degree;
        }
        public static double distance_to_radian(double dist, double radius)
        {
            return dist / radius;
        }

    }
}
