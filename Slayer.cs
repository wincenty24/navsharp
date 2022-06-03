using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace navsharp
{
    class Slayer
    {

        Lines main_line;

        private List<Lines> mappolyline = new List<Lines>();

        //private
        private Map map;
        private double saved_shift_m;

        private bool is_validated = false;
        private bool is_main_line = false;
        private int last_line_num = -1;

        public Values.Value_For_Rise_Fun value_for_rise_fun;
        private Values.Value_For_Decreasing_Fun value_for_decreasing_fun;
        private Values.Value_For_Vertical_Fun value_for_vertical_fun;
        private Values.Value_For_Perpendicular_Fun value_for_perpendicular_fun;

        private Values.Direction direction;
        private Values.Function function;

        private double[] saved_main_points = new double[4];
        private double saved_line_distance_m;

        private int line_adder = 10;
        private int n_line = 10;

        private double line_shift_length_growing = 0;
        private double line_shift_length_decreasing = 0;
        private double line_shift_length_vertical = 0;
        private double line_shift_length_perpendicular= 0;

        public Slayer(Map map, double shift_m)
        {
            this.map = map;
            this.saved_shift_m = shift_m;
        }
        
        public bool IsValidated()
        {
            return is_validated;
        }
       
        public Values.value2return calculate_distance(in double compass, in double[] current_point)//current_points, compass, shift, look_ahead_m, asl, line_distance_m
        {

            if (function == Values.Function.growing)
            {
                direction = Templates.chceck_direction_growing(compass, value_for_rise_fun.beta);
                double[] real_distance = Math_Formulas.real_shift_growing(current_point[0], current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                double real_distance_m = real_distance[3] * (Values.earth_radius_m + Values.asl);
                double dist = preapre_distance_growing(real_distance_m, current_point, saved_shift_m);
                int line = change_line_color_growing(real_distance_m, saved_shift_m, current_point);
                adding_new_lines_growing(line, saved_shift_m, saved_line_distance_m);
                
                double angle = steering_growing(line, real_distance[3], dist, current_point[0], current_point[1], Values.earth_radius_m + Values.asl, compass, Values.look_ahead_m, saved_shift_m);

                return new Values.value2return() { distance = dist, dir = direction, line = line, angle = angle };
            }

            else if (function == Values.Function.decreasing)//maleje
            {
                direction = Templates.chceck_direction_decreasing(compass, value_for_decreasing_fun.beta);
                double[] real_distance = Math_Formulas.real_shift_decreasing(current_point[0], current_point[1], value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                double real_distance_m = real_distance[3] * (Values.earth_radius_m + Values.asl);
                int line = change_line_color_decreasing(real_distance_m, saved_shift_m, current_point);
                double dist = preapre_distance_decreasing(real_distance_m, current_point, saved_shift_m);
                double angle = steering_decreasing(line, real_distance[3], dist, current_point[0], current_point[1], Values.earth_radius_m + Values.asl, compass, Values.look_ahead_m, saved_shift_m);
                adding_new_lines_decreasing(line, saved_shift_m, saved_line_distance_m);
                return new Values.value2return() { distance = dist, dir = direction, line = line, angle = angle };
            }

            else if (function == Values.Function.vertical)//pionowe
            {
                direction = Templates.chceck_direction_vertical(compass, 0);
                double dis = Math_Formulas.distance_radian(current_point[0], current_point[0], current_point[1] - value_for_vertical_fun.reference_b);
                double real_distance = Math_Formulas.real_shift_vertical(current_point[0], current_point[1], value_for_vertical_fun.reference_b, Values.earth_radius_m + Values.asl);
                double[] b = Math_Formulas.calculate_shift_for_vertical(current_point[0], current_point[1], value_for_vertical_fun.reference_b, dis, Values.earth_radius_m + Values.asl);//sprawdz to
                double dist = preapre_distance_vertical(real_distance, current_point, saved_shift_m);
                int line = change_line_color_vertical(real_distance, saved_shift_m, current_point);
                double angle = steering_vertical(line, dis, dist, current_point[0], current_point[1], Values.earth_radius_m + Values.asl, compass, Values.look_ahead_m, saved_shift_m);
                adding_new_lines_vertical(line, saved_shift_m, saved_line_distance_m);
                return new Values.value2return() { distance = dist, dir = direction, line = line, angle = angle };
            }

            else if(function == Values.Function.perpendicular)
            {
                direction = Templates.chceck_direction_perpendicular(compass, 0);
                double dis = Math_Formulas.distance_radian(value_for_perpendicular_fun.reference_a, current_point[0], current_point[1] - current_point[1]);
                double distance_m = dis * (Values.earth_radius_m + Values.asl);
                double dist = preapre_distance_perpendicular(distance_m, current_point, saved_shift_m);
                int line = change_line_color_perpendicular(distance_m, saved_shift_m, current_point);
                double angle = steering_perpendicular(line, dis, dist, current_point[0], current_point[1], Values.earth_radius_m + Values.asl, compass, Values.look_ahead_m, saved_shift_m);
                adding_new_lines_perpendicular(line, saved_shift_m, saved_line_distance_m);
                return new Values.value2return() { distance = dist, dir = direction, line = line, angle = angle };
            }

            return new Values.value2return() { distance = 0, dir = Values.Direction.NULL, line = 0 };
        }

        public void validation(ref double[] _main_points,  in double line_distance_m, in double earth_radius)//main_points, points to calculate math's function
        {
            Templates.validate_points(ref _main_points);
            function = Templates.validate_template(_main_points);

            if (function != Values.Function.NULL)
            {

                //Debug.WriteLine(function.ToString());
                if (function == Values.Function.growing)//fun rośnie
                {
                    double bx = Math_Formulas.calculate_b(_main_points);
                    value_for_rise_fun.alpha = Math_Formulas.calculate_alpha(_main_points[0], bx);
                    value_for_rise_fun.beta = Math.Acos(Math.Sin(value_for_rise_fun.alpha) * Math.Cos(bx));
                    value_for_rise_fun.delta = Templates.template_for_growing(bx, _main_points[1]);
                    add_lines_growing(_main_points, line_distance_m, earth_radius, line_adder, saved_shift_m, true);
                }
                else if (function == Values.Function.decreasing)//maleje
                {
                    double bx = Math_Formulas.calculate_decreasing_b2(_main_points);
                    value_for_decreasing_fun.b_length = bx + _main_points[3];
                    value_for_decreasing_fun.alpha = Math.Atan(Math.Tan(_main_points[2]) / Math.Sin(bx));
                    value_for_decreasing_fun.beta = Math.Acos(Math.Sin(value_for_decreasing_fun.alpha) * Math.Cos(bx));
                    add_lines_decreasing(_main_points, line_distance_m, earth_radius, line_adder, saved_shift_m, true);

                }
                else if (function == Values.Function.vertical)//pionowe
                {
                    value_for_vertical_fun.reference_b = _main_points[1];
                    add_lines_vertical(_main_points, line_distance_m, earth_radius, line_adder, saved_shift_m, true, saved_shift_m);

                }
                else if(function == Values.Function.perpendicular)//poziome
                {
                    value_for_perpendicular_fun.reference_a = _main_points[0];
                    add_lines_perpendicular(_main_points, line_distance_m, earth_radius, line_adder, saved_shift_m, saved_shift_m, true);

                }
            }

            //add_lines(_main_points, line_distance_m, earth_radius, n_lines, shift_m, function);
            saved_main_points = _main_points;
            saved_line_distance_m = line_distance_m;
            is_validated = true;
        }

        private void adding_new_lines_growing(in int line, in double shift_m, in double line_distance_m)
        {
            if (n_line - Math.Abs(line) < 3)
            {
                add_lines_growing(saved_main_points, line_distance_m, (Values.earth_radius_m + Values.asl), line_adder, shift_m, false);
                n_line += line_adder;

            }
        }
        private void adding_new_lines_decreasing(in int line, in double shift_m, in double line_distance_m)
        {
            if (n_line - Math.Abs(line) < 3)
            {
                add_lines_decreasing(saved_main_points, line_distance_m, (Values.earth_radius_m + Values.asl), line_adder, shift_m, false);
                n_line += line_adder;

            }
        }
        private void adding_new_lines_vertical(in int line, in double shift_m, in double line_distance_m)
        {
            if (n_line - Math.Abs(line) < 3)
            {
                add_lines_vertical(saved_main_points, line_distance_m, (Values.earth_radius_m + Values.asl), line_adder, shift_m, false, line_shift_length_vertical);
                n_line += line_adder;

            }
        }
        private void adding_new_lines_perpendicular(in int line, in double shift_m, in double line_distance_m)
        {
            if (n_line - Math.Abs(line) < 3)
            {
                add_lines_perpendicular(saved_main_points, line_distance_m, (Values.earth_radius_m + Values.asl), line_adder, shift_m, line_shift_length_perpendicular, false);
                n_line += line_adder;

            }
        }
        private double steering_growing(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] points = calculate_look_ahead_growing(num_of_line, distance_to_ref, distance_to_line, current_a, current_b, earth_radius, vehicle_degree, look_ahead, shift);
            return Math_Formulas.calculate_steering(points[0], points[1], current_a, current_b, points[2] * distance_to_line, points[3], earth_radius, vehicle_degree, look_ahead, points[4]);
        }

        private double[] calculate_look_ahead_growing(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double shift_length = num_of_line * Math_Formulas.calculate_radian_using_radius_and_length(shift, earth_radius);
            double[] target_points = new double[5];
            double ahead = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead, earth_radius);

            double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
            double degree = (direction == Values.Direction.growing) ? value_for_rise_fun.beta : (value_for_rise_fun.beta + Math.PI);

            double current_c = Math_Formulas.calculate_c_using_a_and_b(current_a, current_b + value_for_rise_fun.delta);
            double shifted_c = current_c + multiplicator * ahead;
            double shifted_a = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c, value_for_rise_fun.alpha);
            double shifted_b = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c, value_for_rise_fun.alpha) - value_for_rise_fun.delta;

            double[] points = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a, shifted_b, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
            Location loc = new Location(Math_Formulas.radian_to_degree(points[0]), Math_Formulas.radian_to_degree(points[1]));
            target_points[0] = points[0];
            target_points[1] = points[1];
            target_points[2] = multiplicator;
            target_points[3] = ahead;
            target_points[4] = degree;
            return target_points;
        }

        private double steering_decreasing(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] points = calculate_look_ahead_decreasing(num_of_line, distance_to_ref, distance_to_line, current_a, current_b, earth_radius, vehicle_degree, look_ahead, shift);
            return Math_Formulas.calculate_steering(points[0], points[1], current_a, current_b, points[2] * distance_to_line, points[3], earth_radius, vehicle_degree, look_ahead, points[4]);
        }

        private double[] calculate_look_ahead_decreasing(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double shift_length = num_of_line * Math_Formulas.calculate_radian_using_radius_and_length(shift, earth_radius);
            double[] target_points = new double[5];
            double ahead = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead, earth_radius);

            double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
            double degree = (direction == Values.Direction.growing) ? 2 * Math.PI - value_for_decreasing_fun.beta : (Math.PI - value_for_decreasing_fun.beta);

            double current_c = Math_Formulas.calculate_c_using_a_and_b(current_a, value_for_decreasing_fun.b_length - current_b);
            double shifted_c = current_c + multiplicator * ahead;
            double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c, value_for_decreasing_fun.alpha);
            double shifted_b_down = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c, value_for_decreasing_fun.alpha);

            double[] points = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_down, shifted_b_down, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
            Location loc = new Location(Math_Formulas.radian_to_degree(points[0]), Math_Formulas.radian_to_degree(points[1]));
            target_points[0] = points[0];
            target_points[1] = points[1];
            target_points[2] = multiplicator;
            target_points[3] = ahead;
            target_points[4] = degree;

            return target_points;
        }

        private double steering_vertical(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] points = calculate_look_ahead_vertical(num_of_line, distance_to_ref, distance_to_line, current_a, current_b, earth_radius, vehicle_degree, look_ahead, shift);
            return Math_Formulas.calculate_steering(points[0], points[1], current_a, current_b, points[2] * distance_to_line, points[3], earth_radius, vehicle_degree, look_ahead, points[4]);
        }

        private double[] calculate_look_ahead_vertical(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] target_points = new double[5];
            double ahead = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead, earth_radius);

            double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
            double degree = (direction == Values.Direction.growing) ? 0 : Math.PI;
            double top_a = current_a + multiplicator * Math_Formulas.calculate_radian_using_radius_and_length(ahead * earth_radius, earth_radius);
            //Debug.WriteLine($"line:{num_of_line}");
            double[] b = Math_Formulas.calculate_shift_for_vertical(top_a, value_for_vertical_fun.reference_b, value_for_vertical_fun.reference_b, num_of_line * shift, earth_radius);
            double ret_b = (value_for_vertical_fun.reference_b < current_b) ? b[0] : b[1];

            Location loc = new Location(Math_Formulas.radian_to_degree(top_a), Math_Formulas.radian_to_degree(ret_b));
            target_points[0] = top_a;
            target_points[1] = ret_b;
            target_points[2] = multiplicator;
            target_points[3] = ahead;
            target_points[4] = degree;
            return target_points;
        }
       
        private double steering_perpendicular(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] points = calculate_look_ahead_perpendicular(num_of_line, distance_to_ref, distance_to_line, current_a, current_b, earth_radius, vehicle_degree, look_ahead, shift);
            return Math_Formulas.calculate_steering(points[0], points[1], current_a, current_b, points[2] * distance_to_line, points[3], earth_radius, vehicle_degree, look_ahead, points[4]);
        }

        private double[] calculate_look_ahead_perpendicular(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] target_points = new double[5];
            double ahead = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead, earth_radius);

            double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
            double degree = (direction == Values.Direction.growing) ? (Math.PI / 2) : 1.5 * Math.PI;

            double[] points = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, current_b, current_b, ahead * earth_radius, earth_radius);
            double top_b = current_b + multiplicator * points[2];
            double[] a_top = Math_Formulas.calculate_shift_for_perpendicular(value_for_perpendicular_fun.reference_a, current_b, value_for_perpendicular_fun.reference_a, num_of_line * shift, earth_radius);
            Location loc = new Location(Math_Formulas.radian_to_degree(a_top[0]), Math_Formulas.radian_to_degree(top_b));
            target_points[0] = a_top[0];
            target_points[1] = top_b;
            target_points[2] = multiplicator;
            target_points[3] = ahead;
            target_points[4] = degree;
            return target_points;
        }


        private void steering(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double[] points = calculate_look_ahead(num_of_line, distance_to_ref, distance_to_line, current_a, current_b, earth_radius, vehicle_degree, look_ahead, shift);
            Values.angle = Math_Formulas.calculate_steering(points[0], points[1], current_a, current_b, points[2] * distance_to_line, points[3], earth_radius, vehicle_degree, look_ahead, points[4]);
        }
        private double[] calculate_look_ahead(int num_of_line, double distance_to_ref, double distance_to_line, double current_a, double current_b, double earth_radius, double vehicle_degree, double look_ahead, double shift)
        {
            double shift_length = num_of_line * Math_Formulas.calculate_radian_using_radius_and_length(shift, earth_radius);
            double[] target_points = { 0, 0, 0, 0, 0 };
            double ahead = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead, earth_radius);

            if (function == Values.Function.growing)
            {
                double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
                double degree = (direction == Values.Direction.growing) ? value_for_rise_fun.beta : (value_for_rise_fun.beta + Math.PI);

                double current_c = Math_Formulas.calculate_c_using_a_and_b(current_a, current_b + value_for_rise_fun.delta);
                double shifted_c = current_c + multiplicator * ahead;
                double shifted_a = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c, value_for_rise_fun.alpha);
                double shifted_b = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c, value_for_rise_fun.alpha) - value_for_rise_fun.delta;

                double[] points = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a, shifted_b, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                Location loc = new Location(Math_Formulas.radian_to_degree(points[0]), Math_Formulas.radian_to_degree(points[1]));
                target_points[0] = points[0];
                target_points[1] = points[1];
                target_points[2] = multiplicator;
                target_points[3] = ahead;
                target_points[4] = degree;
                return target_points;

            }
            if (function == Values.Function.decreasing)
            {

                double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
                double degree = (direction == Values.Direction.growing) ? 2 * Math.PI - value_for_decreasing_fun.beta : (Math.PI - value_for_decreasing_fun.beta);

                double current_c = Math_Formulas.calculate_c_using_a_and_b(current_a, value_for_decreasing_fun.b_length - current_b);
                double shifted_c = current_c + multiplicator * ahead;
                double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c, value_for_decreasing_fun.alpha);
                double shifted_b_down = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c, value_for_decreasing_fun.alpha);

                double[] points = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_down, shifted_b_down, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                Location loc = new Location(Math_Formulas.radian_to_degree(points[0]), Math_Formulas.radian_to_degree(points[1]));
                target_points[0] = points[0];
                target_points[1] = points[1];
                target_points[2] = multiplicator;
                target_points[3] = ahead;
                target_points[4] = degree;

                return target_points;
                //Location loc = new Location(Math_Formulas.radian_to_degree(target_points[0]), Math_Formulas.radian_to_degree(target_points[1]));
                //pin1.Location = loc;

            }
            else if (function == Values.Function.vertical)
            {
                double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
                double degree = (direction == Values.Direction.growing) ? 0 : Math.PI;
                double top_a = current_a + multiplicator * Math_Formulas.calculate_radian_using_radius_and_length(ahead * earth_radius, earth_radius);
                //Debug.WriteLine($"line:{num_of_line}");
                double[] b = Math_Formulas.calculate_shift_for_vertical(top_a, value_for_vertical_fun.reference_b, value_for_vertical_fun.reference_b, num_of_line * shift, earth_radius);
                double ret_b = (value_for_vertical_fun.reference_b < current_b) ? b[0] : b[1];
                
                Location loc = new Location(Math_Formulas.radian_to_degree(top_a), Math_Formulas.radian_to_degree(ret_b));
                target_points[0] = top_a;
                target_points[1] = ret_b;
                target_points[2] = multiplicator;
                target_points[3] = ahead;
                target_points[4] = degree;
            }
            else if (function == Values.Function.perpendicular)
            {
                double multiplicator = (direction == Values.Direction.growing) ? 1 : -1;
                double degree = (direction == Values.Direction.growing) ? (Math.PI / 2) : 1.5 * Math.PI;
                
                double[] points = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, current_b, current_b, ahead* earth_radius, earth_radius);
                double top_b = current_b + multiplicator * points[2];
                double[] a_top = Math_Formulas.calculate_shift_for_perpendicular(value_for_perpendicular_fun.reference_a, current_b, value_for_perpendicular_fun.reference_a, num_of_line * shift, earth_radius);
                Location loc = new Location(Math_Formulas.radian_to_degree(a_top[0]), Math_Formulas.radian_to_degree(top_b));
                target_points[0] = a_top[0];
                target_points[1] = top_b;
                target_points[2] = multiplicator;
                target_points[3] = ahead;
                target_points[4] = degree;
                return target_points;
            }
                return target_points;
        }

        private int change_line_color_growing(in double real_shift, in double width, double[] points)
        {
            /*
            * 
            *     -/ 
            *     / +
            *    /
            * 
            */
            int line = Math_Formulas.which_line(real_shift, width);
            double a = Math_Formulas.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + points[1]);
            if (a > points[0])
            {
                prepare_line_for_change(line, 2);
                return line;

            }
            else
            {
                prepare_line_for_change(line, 1);
                return -line;
            }
        }
        private int change_line_color_decreasing(in double real_shift, in double width, double[] points)
        {
            int line = Math_Formulas.which_line(real_shift, width);
            double a = Math.Atan(Math.Tan(value_for_decreasing_fun.alpha) * Math.Sin(value_for_decreasing_fun.b_length - points[1]));

            if (a > points[0])
            {
                prepare_line_for_change(line, 2);
                return line;

            }
            else
            {
                prepare_line_for_change(line, 1);
                return -line;
            }
        }

        private int change_line_color_vertical(in double real_shift, in double width, double[] points)
        {
            /*
              * +
              * ======
              * -
              * 
              */
            int line = Math_Formulas.which_line(real_shift, width);

            if (value_for_vertical_fun.reference_b < points[1])
            {
                prepare_line_for_change(line, 2);
                return line;
            }
            else
            {
                prepare_line_for_change(line, 1);
                return -line;
            }
        }

        private int change_line_color_perpendicular(in double real_shift, in double width, double[] points)
        {
            /*
             *  - | + 
             * 
             * 
             * 
             */
            int line = Math_Formulas.which_line(real_shift, width);

            if (value_for_perpendicular_fun.reference_a < points[0])
            {
                prepare_line_for_change(line, 2);
                return line;

            }
            else
            {
                prepare_line_for_change(line, 1);
                return -line;
            }
        }


       
        private void prepare_line_for_change(in int line, in int factor)
        {
            //Debug.WriteLine($"mappolyline.Count {mappolyline.Count} {line}");
            if(line <= mappolyline.Count / 2)
            {
                if (line != 0)
                {
                    if (is_main_line)
                    {
                        var col = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Blue");
                        main_line.polyline.Stroke = new System.Windows.Media.SolidColorBrush(col);
                        is_main_line = false;
                    }
                    int line_num = (line * 2) - factor;
                    if (line_num != last_line_num)
                    {
                        var col = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Yellow");
                        var col1 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Red");
                        mappolyline[line_num].polyline.Stroke = new System.Windows.Media.SolidColorBrush(col);
                        if (last_line_num != -1)
                        {
                            mappolyline[last_line_num].polyline.Stroke = new System.Windows.Media.SolidColorBrush(col1);
                        }
                        last_line_num = line_num;
                    }
                }
                else
                {
                    if (!is_main_line)
                    {
                        var col = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Yellow");
                        main_line.polyline.Stroke = new System.Windows.Media.SolidColorBrush(col);
                        is_main_line = true;
                        if (last_line_num != -1)
                        {
                            var col1 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Red");
                            mappolyline[last_line_num].polyline.Stroke = new System.Windows.Media.SolidColorBrush(col1);
                            last_line_num = -1;
                        }
                    }
                }
            }
            else
            {
                
                if (is_main_line)
                {
                    var col = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Blue");
                    main_line.polyline.Stroke = new System.Windows.Media.SolidColorBrush(col);
                    is_main_line = false;
                }
                if(last_line_num != -1)
                {
                    var col = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString($"Red");
                    mappolyline[last_line_num].polyline.Stroke = new System.Windows.Media.SolidColorBrush(col);
                    last_line_num = -1;
                }
            }
        }

        private double preapre_distance_growing(in double real_distance, in double[] points, in double shift_m)
        {
            double dist = Math_Formulas.distance_to_line(real_distance, shift_m);
            double a = Math_Formulas.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + points[1]);
            if (points[0] > a)
            {
                return (-1) * dist;
            }
            return dist;
        }
        private double preapre_distance_decreasing(in double real_distance, in double[] points, in double shift_m)
        {
            double dist = Math_Formulas.distance_to_line(real_distance, shift_m);
            double a = Math.Atan(Math.Tan(value_for_decreasing_fun.alpha) * Math.Sin(value_for_decreasing_fun.b_length - points[1]));

            if (points[0] < a)
            {
                return (-1) * dist;
            }
            return dist;
        }

        private double preapre_distance_vertical(in double real_distance, in double[] points, in double shift_m)
        {
            double dist = Math_Formulas.distance_to_line(real_distance, shift_m);
            if (points[1] < value_for_vertical_fun.reference_b)
            {
                return (-1) * dist;
            }
            return dist;
        }

        private double preapre_distance_perpendicular(in double real_distance, in double[] points, in double shift_m)
        {
            double dist = Math_Formulas.distance_to_line(real_distance, shift_m);
            if (points[0] > value_for_perpendicular_fun.reference_a)
            {
                return (-1) * dist;
            }
            return dist;
        }

        private void add_lines_growing(in double[] main_points, in double line_distance_m, in double radius, in int n_lines, in double shift_m, bool draw_main_line)
        {
            double current_c_down = Math_Formulas.calculate_c_using_a_and_b(main_points[0], main_points[1] + value_for_rise_fun.delta);
            double current_c_top = Math_Formulas.calculate_c_using_a_and_b(main_points[2], main_points[3] + value_for_rise_fun.delta);
            double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha);
            double shifted_b_down = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha) - value_for_rise_fun.delta;
            double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha);
            double shifted_b_top = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha) - value_for_rise_fun.delta;
            if (draw_main_line)
            {
                main_line = new Lines(shifted_a_down, shifted_b_down, shifted_a_top, shifted_b_top, map, "Blue");
                main_line.draw_line();
            }
            for (int index = 0; index < n_lines; index++)
            {
                line_shift_length_growing += Math_Formulas.calculate_radian_using_radius_and_length(shift_m, radius);

                double[] down_growing_val = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a_down, shifted_b_down, line_shift_length_growing, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                double[] top_growing_val = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a_top, shifted_b_top, line_shift_length_growing, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                mappolyline.Add(new Lines(top_growing_val[0], top_growing_val[1], down_growing_val[0], down_growing_val[1], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();

                double[] top_decreasing_val = Math_Formulas.calculate_shift_for_growing_fun_up(shifted_a_down, shifted_b_down, line_shift_length_growing, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                double[] down_decreasing_val = Math_Formulas.calculate_shift_for_growing_fun_up(shifted_a_top, shifted_b_top, line_shift_length_growing, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                mappolyline.Add(new Lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();
            }
        }
        private void add_lines_decreasing(in double[] main_points, in double line_distance_m, in double radius, in double n_lines, in double shift_m, bool draw_main_line)
        {
            double current_c_down = Math_Formulas.calculate_c_using_a_and_b(main_points[2], value_for_decreasing_fun.b_length - main_points[3]);
            double current_c_top = Math_Formulas.calculate_c_using_a_and_b(main_points[0], value_for_decreasing_fun.b_length - main_points[1]);
            double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
            double shifted_b_down = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
            double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);
            double shifted_b_top = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);
            if (draw_main_line)
            {
                main_line = new Lines(shifted_a_down, shifted_b_down, shifted_a_top, shifted_b_top, map, "Blue");
                main_line.draw_line();
            }
            for (int index = 0; index < n_lines; index++)
            {
                line_shift_length_decreasing += Math_Formulas.calculate_radian_using_radius_and_length(shift_m, radius);

                double[] up_growing_val = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_down, shifted_b_down, line_shift_length_decreasing, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                double[] down_growing_val = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_top, shifted_b_top, line_shift_length_decreasing, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                mappolyline.Add(new Lines(down_growing_val[0], down_growing_val[1], up_growing_val[0], up_growing_val[1], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();

                double[] top_decreasing_val = Math_Formulas.calculate_shift_for_decreasing_fun_up(shifted_a_down, shifted_b_down, line_shift_length_decreasing, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                double[] down_decreasing_val = Math_Formulas.calculate_shift_for_decreasing_fun_up(shifted_a_top, shifted_b_top, line_shift_length_decreasing, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                mappolyline.Add(new Lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();

            }
        }
        private void add_lines_vertical(in double[] main_points, in double line_distance_m, in double radius, in int n_lines, in double shift_m, in bool draw_main_line, double start_shift)
        {
            double top_a = main_points[2] + Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            double down_a = main_points[0] - Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, radius);
            if (draw_main_line)
            {
                main_line = new Lines(top_a, main_points[1], down_a, main_points[3], map, "Blue");
                main_line.draw_line();
            }
            for (int index = 0; index < n_lines; index++)
            {
                double[] b = Math_Formulas.calculate_shift_for_vertical(top_a, value_for_vertical_fun.reference_b, value_for_vertical_fun.reference_b, start_shift, radius);

                start_shift += shift_m;
                mappolyline.Add(new Lines(top_a, b[0], down_a, b[0], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();
                mappolyline.Add(new Lines(top_a, b[1], down_a, b[1], map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();
            }
            line_shift_length_vertical = Math.Abs( start_shift);
        }
        private void add_lines_perpendicular(in double[] main_points, in double line_distance_m, in double radius, in double n_lines, in double shift_m, in double start_shift_m,in bool draw_main_line)
        {
            double _shift_top = start_shift_m;
            double _shift_down = -start_shift_m;
            double[] b_1 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, main_points[1], main_points[1], line_distance_m, radius);
            double[] b_3 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, main_points[3], main_points[3], line_distance_m, radius);
            if (draw_main_line)
            {
                main_line = new Lines(main_points[0], main_points[1] - b_1[2], main_points[2], main_points[3] + b_3[2], map, "Blue");
                main_line.draw_line();
            }
            for (int index = 0; index < n_lines; index++)
            {
                double top_b = main_points[3] + b_3[2];
                double down_b = main_points[1] - b_1[2];
                double[] a_top = Math_Formulas.calculate_shift_for_perpendicular(main_points[0], main_points[1], value_for_perpendicular_fun.reference_a, _shift_top, radius);
                double[] a_down = Math_Formulas.calculate_shift_for_perpendicular(main_points[0], main_points[1], value_for_perpendicular_fun.reference_a, _shift_down, radius);

                _shift_top += shift_m;
                _shift_down -= shift_m;

                mappolyline.Add(new Lines(a_top[0], top_b, a_top[0], down_b, map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();
                mappolyline.Add(new Lines(a_down[0], top_b, a_down[0], down_b, map, "Red"));
                mappolyline[mappolyline.Count - 1].draw_line();
            }
            line_shift_length_perpendicular = Math.Abs(_shift_top);
        }

        
        public void add_pin(double a, double b, Pushpin pin, Map maps)
        {
            a = Math_Formulas.radian_to_degree(a);
            b = Math_Formulas.radian_to_degree(b);
            Location loc = new Location(a, b);
            pin.Location = loc;
           // maps.Children.Add(pin);


        }
       
    }
}
