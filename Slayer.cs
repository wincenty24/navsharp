using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace navsharp
{
    class Slayer
    {
        DispatcherTimer timer = new DispatcherTimer();


        Lines main_line;
        //public double[] current_point = new double[2];
        public List<Pushpin> ppoints = new List<Pushpin>();

        public Pushpin pin_curloc = new Pushpin();
        public Pushpin pin_loc = new Pushpin();
        public Pushpin pin1 = new Pushpin();
        public Pushpin pin2 = new Pushpin();


        public List<Lines> mappolyline = new List<Lines>();


        private bool is_validated = false;
        public bool is_working = false;



        MapPolyline poly = new MapPolyline();

        //private
        private Map map;
        private bool is_main_line = false;
        private double last_position_b = 0;
        private double last_position_a = 0;
        private int last_line_num = -1;
        public Values.Value_For_Rise_Fun value_for_rise_fun;
        private Values.Value_For_Decreasing_Fun value_for_decreasing_fun;
        private Values.Value_For_Vertical_Fun value_for_vertical_fun;
        private Values.Value_For_Perpendicular_Fun value_for_perpendicular_fun;

        Values.Direction direction;
        Values.Function function;
        public Slayer(Map map)
        {
            this.map = map;
            
        }
        public void stop()
        {
            is_validated = false;
        }
        public bool IsValidated()
        {
            return is_validated;
        }
        public double[] invoke_main_fun()
        {
            if (function == Values.Function.growing)
            {
                direction = Templates.chceck_direction(function, Values.compass, value_for_rise_fun.beta);
                double[] real_distance = Math_Formulas.real_shift_growing(Values.current_point[0], Values.current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                double real_distance_m = real_distance[3] * (Values.earth_radius_m + Values.asl);
                double dist = preapre_distance(real_distance_m);
                int line = change_line_color(real_distance_m, Values.shif_m);
                //Debug.WriteLine($"{dist} {real_distance_m}");
                steering(line, real_distance[3], dist, Values.current_point[0], Values.current_point[1], Values.earth_radius_m + Values.asl, Values.compass, Values.look_ahead_m, Values.shif_m);
                add_pin(real_distance[0], real_distance[1], pin_loc, map);
                add_pin(Values.current_point[0], Values.current_point[1], pin_curloc, map);
                double dir = (direction == Values.Direction.growing) ? 0.0f : 1.0f;
                double[] ret = { dist, line, dir};
                return ret;

            }
            else if (function == Values.Function.decreasing)//maleje
            {
                direction = Templates.chceck_direction(function, Values.compass, value_for_decreasing_fun.beta);
                double[] real_distance = Math_Formulas.real_shift_decreasing(Values.current_point[0], Values.current_point[1], value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                double real_distance_m = real_distance[3] * (Values.earth_radius_m + Values.asl);
                int line = change_line_color(real_distance_m, Values.shif_m);
                double dist = preapre_distance(real_distance_m);
                steering(line, real_distance[3], dist, Values.current_point[0], Values.current_point[1], Values.earth_radius_m + Values.asl, Values.compass, Values.look_ahead_m, Values.shif_m);
                add_pin(real_distance[0], real_distance[1], pin_loc, map);
                add_pin(Values.current_point[0], Values.current_point[1], pin_curloc, map);
                //Debug.WriteLine($"asd {dist}       {real_distance_m}");
                double dir = (direction == Values.Direction.growing) ? 0.0f : 1.0f;
                double[] ret = { dist, line, dir };
                return ret;
            }
            else if (function == Values.Function.vertical)//pionowe
            {
                direction = Templates.chceck_direction(function, Values.compass, 0);
                double dis = Math_Formulas.distance_radian(Values.current_point[0], Values.current_point[0], Values.current_point[1] - value_for_vertical_fun.reference_b);
                double real_distance = Math_Formulas.real_shift_vertical(Values.current_point[0], Values.current_point[1], value_for_vertical_fun.reference_b, Values.earth_radius_m + Values.asl);
                double[] b = Math_Formulas.calculate_shift_for_vertical(Values.current_point[0], Values.current_point[1], value_for_vertical_fun.reference_b, dis, Values.earth_radius_m + Values.asl);//sprawdz to
                //add_pin(Values.current_point[0], value_for_vertical_fun.reference_b, pin_loc3, map);
                double dist = preapre_distance(real_distance);
                int line = change_line_color(real_distance, Values.shif_m);
                steering(line, dis, dist, Values.current_point[0], Values.current_point[1], Values.earth_radius_m + Values.asl, Values.compass, Values.look_ahead_m, Values.shif_m);
                //Debug.WriteLine($"{Math_Formulas.radian_to_degree(dis)} dist:{dist} {real_distance} {dis * earth_radius_m}  ");
                add_pin(Values.current_point[0], Values.current_point[1], pin_curloc, map);
                add_pin(Values.current_point[0], b[0], pin_loc, map);
                double dir = (direction == Values.Direction.growing) ? 0.0f : 1.0f;
                double[] ret = { dist, line, dir };
                return ret;
            }
            else if(function == Values.Function.perpendicular)
            {
                direction = Templates.chceck_direction(function, Values.compass, 0);
                double dis = Math_Formulas.distance_radian(value_for_perpendicular_fun.reference_a, Values.current_point[0], Values.current_point[1] - Values.current_point[1]);
                double distance_m = dis * (Values.earth_radius_m + Values.asl);
                double dist = preapre_distance(distance_m);
                //Debug.WriteLine($"my dis {dist} {dis * earth_radius_m}");
                int line = change_line_color(distance_m, Values.shif_m);
                steering(line, dis, dist, Values.current_point[0], Values.current_point[1], Values.earth_radius_m + Values.asl, Values.compass, Values.look_ahead_m, Values.shif_m);

                add_pin(Values.current_point[0], Values.current_point[1], pin_curloc, map);
                add_pin(value_for_perpendicular_fun.reference_a, Values.current_point[1], pin_loc, map);
                double dir = (direction == Values.Direction.growing) ? 0.0f : 1.0f;
                double[] ret = { dist, line, dir };
                return ret;
            }
            double[] val = {0};
            return val;
        }
        private void Timer_Tick(object sender, EventArgs e)//it does not work
        {
            if( function == Values.Function.growing)
            {
              


                double[] diss = Math_Formulas.real_shift_growing(Values.current_point[0], Values.current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);

                //var w = is_track_decreasing_or_rising();
                //Debug.WriteLine($"{is_track_decreasing_or_rising()} distance: {diss[2]} {diss[2] * earth_radius_m}");
                add_pin(diss[0], diss[1], pin_loc, map);
                // pin_loc3.ToolTip = $"{Math_Formulas.radian_to_degree(diss[0])} {Math_Formulas.radian_to_degree(diss[0])}";
                //pin_curloc.ToolTip = $"{Math_Formulas.radian_to_degree(Values.current_point[0])} {Math_Formulas.radian_to_degree(Values.current_point[1])}";
                add_pin(Values.current_point[0], Values.current_point[1], pin_curloc, map);

                last_points(Values.current_point[0], Values.current_point[1]);
            }
            
            //draw_tracking_line(Values.current_point[0], Values.current_point[1], map);
        }

        public void calck(double curr_loc_a, double curr_loc_b, double delta, double alpha, double shift)
        {

            double c = Math.Acos(Math.Cos(curr_loc_a) * Math.Cos(delta + curr_loc_b));
            double mojec = c + shift;
            double mojenowe_a = Math.Asin(Math.Sin(alpha) * Math.Sin(mojec));
            double mojenowe_b = Math.Atan(Math.Cos(alpha) * Math.Tan(mojec)) - delta;
            Pushpin pin1 = new Pushpin();
            Location loc1 = new Location(Math_Formulas.radian_to_degree(mojenowe_a), Math_Formulas.radian_to_degree(mojenowe_b));
            pin1.Location = loc1;

        }
        public void validation( double[] cur_points)
        {
            Templates.validate_points(ref Values.main_points);
            int fun = Templates.validate_template(Values.main_points);
            //Debug.WriteLine($"funnnnn {fun}");
            function = (Values.Function)Enum.Parse(typeof(Values.Function), fun.ToString());

            last_position_a = cur_points[0];
            last_position_b = cur_points[1];

            if (fun != 4)
            {

                //Debug.WriteLine(function.ToString());
                if (function == Values.Function.growing)//fun rośnie
                {
                    double bx = Math_Formulas.calculate_b(Values.main_points);
                    value_for_rise_fun.alpha = Math_Formulas.calculate_alpha(Values.main_points[0], bx);
                    value_for_rise_fun.beta = Math.Acos(Math.Sin(value_for_rise_fun.alpha) * Math.Cos(bx));
                    value_for_rise_fun.delta = Templates.template_for_growing(bx, Values.main_points[1]);

                    //Values.angle = value_for_rise_fun.beta;
                }
                else if (function == Values.Function.decreasing)//maleje
                {
                    double bx = Math_Formulas.calculate_decreasing_b2(Values.main_points);
                    value_for_decreasing_fun.b_length = bx + Values.main_points[3];
                    value_for_decreasing_fun.alpha = Math.Atan(Math.Tan(Values.main_points[2]) / Math.Sin(bx));
                    value_for_decreasing_fun.beta = Math.Acos(Math.Sin(value_for_decreasing_fun.alpha) * Math.Cos(bx));
                }
                else if (function == Values.Function.vertical)//pionowe
                {
                    value_for_vertical_fun.reference_b = Values.main_points[1]; 
                }
                else if(function == Values.Function.perpendicular)//poziome
                {
                    value_for_perpendicular_fun.reference_a = Values.main_points[0];
                }
            }


            //map.Heading = - Math_Formulas.radian_to_degree( value_for_rise_fun.beta);
            ///Debug.WriteLine($"{main_points[0]} {main_points[1]} {main_points[2]} {main_points[3]}");

            //timer.Start();
            
            map.Children.Add(pin_curloc);
            map.Children.Add(pin_loc);
            map.Children.Add(pin1);
            map.Children.Add(pin2);
            
            map.Children.Add(poly);
            
            //draw_main_line();
            add_lines();
            //draw_route();
            is_validated = true;
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
                pin1.Location = loc;
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
                pin1.Location = loc;
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
                Debug.WriteLine($"line:{num_of_line}");
                double[] b = Math_Formulas.calculate_shift_for_vertical(top_a, value_for_vertical_fun.reference_b, value_for_vertical_fun.reference_b, num_of_line * shift, earth_radius);
                double ret_b = (value_for_vertical_fun.reference_b < current_b) ? b[0] : b[1];
                
                Location loc = new Location(Math_Formulas.radian_to_degree(top_a), Math_Formulas.radian_to_degree(ret_b));
                pin1.Location = loc;
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
                pin1.Location = loc;
                target_points[0] = a_top[0];
                target_points[1] = top_b;
                target_points[2] = multiplicator;
                target_points[3] = ahead;
                target_points[4] = degree;
                return target_points;
            }
                return target_points;
        }

        private int change_line_color(double real_shift, double width)//rest of division
        {
            int line = Math_Formulas.which_line(real_shift, width);
            if (function == Values.Function.vertical)
            {/*
              * +
              * ======
              * -
              * 
              */

                if (value_for_vertical_fun.reference_b < Values.current_point[1])
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
            if (function == Values.Function.perpendicular)
            {
                /*
                 *  - | + 
                 * 
                 * 
                 * 
                 */
                if (value_for_perpendicular_fun.reference_a < Values.current_point[0])
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
            if (function == Values.Function.growing)
            {
                /*
                 * 
                 *     -/ 
                 *     / +
                 *    /
                 * 
                 */
                double a = Math_Formulas.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + Values.current_point[1]);
                if (a > Values.current_point[0])
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
            if (function == Values.Function.decreasing)
            {
                double a = Math.Atan(Math.Tan(value_for_decreasing_fun.alpha) * Math.Sin(value_for_decreasing_fun.b_length - Values.current_point[1]));
               
                if (a > Values.current_point[0])
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
            return 0;
        }
        private void prepare_line_for_change(int line, int factor)
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
        private void draw_main_line()////trzeba zrobić dla rosnącej  ale i tak jest nie używane
        {
            if(function == Values.Function.perpendicular)
            {
                double[] b_1 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, Values.main_points[1], Values.main_points[1], Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double[] b_3 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, Values.main_points[3], Values.main_points[3], Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double top_b = Values.main_points[3] + b_3[2];
                double down_b = Values.main_points[1] - b_1[2];
                main_line = new Lines(value_for_perpendicular_fun.reference_a, down_b, value_for_perpendicular_fun.reference_a, top_b, map, "Blue");
                main_line.draw_line();

            }
            else if(function == Values.Function.decreasing)
            {
                double current_c_down = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[2], value_for_decreasing_fun.b_length - Values.main_points[3]);
                double current_c_top = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[0], value_for_decreasing_fun.b_length - Values.main_points[1]);
                double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
                double shifted_b_down = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
                double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);
                double shifted_b_top = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);

                main_line = new Lines(shifted_a_down, shifted_b_down, shifted_a_top, shifted_b_top, map, "Blue");

            }
            else
            {
                main_line = new Lines(Values.main_points[0], Values.main_points[1], Values.main_points[2], Values.main_points[3], map, "Blue");
                main_line.draw_line();

            }

        } 
        public string is_track_decreasing_or_rising(double current_c_length, double last_c_length)
        {
            if (function == Values.Function.growing)
            {

            if (last_c_length < current_c_length) { 
                //Debug.WriteLine("growing");
                return "growing";
            }
            else if (last_c_length > current_c_length)
            {
                //Debug.WriteLine("decreasing");
                return "decreasing";
            }
            else
            {
                //Debug.WriteLine("the same");
                return "the same";
            }
        }
        return "null";
            
        }
        private double preapre_distance(double real_distance)
        {

            if (function == Values.Function.growing)
            {
                double dist = Math_Formulas.distance_to_line(real_distance, Values.shif_m);
                double a = Math_Formulas.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + Values.current_point[1]);
                if (Values.current_point[0] > a)
                {
                    return (-1) * dist;
                }
                return dist;
            }
            else if (function == Values.Function.decreasing)
            {
                double dist = Math_Formulas.distance_to_line(real_distance, Values.shif_m);
                double a = Math.Atan(Math.Tan(value_for_decreasing_fun.alpha) * Math.Sin(value_for_decreasing_fun.b_length - Values.current_point[1]));

                if (Values.current_point[0] < a)
                {
                    return (-1) * dist;
                }
                return dist;
            }
            else if (function == Values.Function.vertical)
            {

                double dist = Math_Formulas.distance_to_line(real_distance, Values.shif_m);
                if (Values.current_point[1] < value_for_vertical_fun.reference_b)
                {
                    return (-1) * dist;
                }
                return dist;
            }
            else if (function == Values.Function.perpendicular)
            {
                /* -
                 *------------------------------
                 *+
                 */
                double dist = Math_Formulas.distance_to_line(real_distance, Values.shif_m);
                if (Values.current_point[0] > value_for_perpendicular_fun.reference_a)
                {
                    return (-1) * dist;
                }
                return dist;
            }


                return 0;
        } 
        private void add_lines()
        {
            if (function == Values.Function.growing)
            {
                double shift_length = 0;
                double current_c_down = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[0], Values.main_points[1] + value_for_rise_fun.delta);
                double current_c_top = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[2], Values.main_points[3] + value_for_rise_fun.delta);
                double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha);
                double shifted_b_down = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha) - value_for_rise_fun.delta;
                double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha);
                double shifted_b_top = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha) - value_for_rise_fun.delta;
                main_line = new Lines(shifted_a_down, shifted_b_down, shifted_a_top, shifted_b_top, map, "Blue");
                main_line.draw_line();
                for (int index = 0; index < Values.how_many_lines; index++)
                {
                    shift_length += Math_Formulas.calculate_radian_using_radius_and_length(Values.shif_m, Values.earth_radius_m + Values.asl);

                    double[] down_growing_val = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a_down, shifted_b_down, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    double[] top_growing_val = Math_Formulas.calculate_shift_for_growing_fun_down(shifted_a_top, shifted_b_top, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    mappolyline.Add(new Lines(top_growing_val[0], top_growing_val[1], down_growing_val[0], down_growing_val[1], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();

                    double[] top_decreasing_val = Math_Formulas.calculate_shift_for_growing_fun_up(shifted_a_down, shifted_b_down, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    double[] down_decreasing_val = Math_Formulas.calculate_shift_for_growing_fun_up(shifted_a_top, shifted_b_top, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    mappolyline.Add(new Lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();
                }
            }

            else if (function == Values.Function.decreasing)
            {
                double shift_length = 0;
                double current_c_down = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[2], value_for_decreasing_fun.b_length - Values.main_points[3]);
                double current_c_top = Math_Formulas.calculate_c_using_a_and_b(Values.main_points[0], value_for_decreasing_fun.b_length - Values.main_points[1]);
                double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
                double shifted_b_down = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_decreasing_fun.alpha);
                double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);
                double shifted_b_top = value_for_decreasing_fun.b_length - Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_decreasing_fun.alpha);
                main_line = new Lines(shifted_a_down, shifted_b_down, shifted_a_top, shifted_b_top, map, "Blue");
                main_line.draw_line();
                for (int index = 0; index < Values.how_many_lines; index++)
                {
                    shift_length += Math_Formulas.calculate_radian_using_radius_and_length(Values.shif_m, Values.earth_radius_m + Values.asl);

                    double[] up_growing_val = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_down, shifted_b_down, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                    double[] down_growing_val = Math_Formulas.calculate_shift_for_decreasing_fun_down(shifted_a_top, shifted_b_top, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                    mappolyline.Add(new Lines(down_growing_val[0], down_growing_val[1], up_growing_val[0], up_growing_val[1], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();

                    double[] top_decreasing_val = Math_Formulas.calculate_shift_for_decreasing_fun_up(shifted_a_down, shifted_b_down, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                    double[] down_decreasing_val = Math_Formulas.calculate_shift_for_decreasing_fun_up(shifted_a_top, shifted_b_top, shift_length, value_for_decreasing_fun.alpha, value_for_decreasing_fun.beta, value_for_decreasing_fun.b_length);
                    mappolyline.Add(new Lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();

                }
            }
            else if (function == Values.Function.vertical)
            {
                double shift_length = 0;
                double shift_right = Values.shif_m;
                double shift_left = -Values.shif_m;
                double top_a = Values.main_points[2] + Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double down_a = Values.main_points[0] - Math_Formulas.calculate_radian_using_radius_and_length(Values.line_distance_m, Values.earth_radius_m + Values.asl);
                main_line = new Lines(top_a, Values.main_points[1], down_a, Values.main_points[3], map, "Blue");
                main_line.draw_line();
                for (int index = 0; index < Values.how_many_lines; index++)
                {
                    shift_length += Math_Formulas.calculate_radian_using_radius_and_length(Values.shif_m, Values.earth_radius_m + Values.asl);

                    double[] b = Math_Formulas.calculate_shift_for_vertical(top_a, value_for_vertical_fun.reference_b, value_for_vertical_fun.reference_b, shift_right, Values.earth_radius_m + Values.asl);
      
                    shift_right += Values.shif_m;
                    shift_left -= Values.shif_m;
                    /*
                    Pushpin pin1 = new Pushpin();
                    Pushpin pin2 = new Pushpin();
                    Location loc1 = new Location(Math_Formulas.radian_to_degree(top_a), Math_Formulas.radian_to_degree(b[0]));
                    Location loc2 = new Location(Math_Formulas.radian_to_degree(main_points[2]), Math_Formulas.radian_to_degree(b[0]));
                    pin1.Location = loc1;
                    map.Children.Add(pin1);
                    pin2.Location = loc2;
                    map.Children.Add(pin2);
                    */
                    mappolyline.Add(new Lines(top_a, b[0], down_a, b[0], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();
                    mappolyline.Add(new Lines(top_a, b[1], down_a, b[1], map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();
                }
            }
            else if (function == Values.Function.perpendicular)
            {
                double shift_length = 0;
                double _shift_top = Values.shif_m;
                double _shift_down = -Values.shif_m;
                double[] b_1 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, Values.main_points[1], Values.main_points[1], Values.line_distance_m, Values.earth_radius_m + Values.asl);
                double[] b_3 = Math_Formulas.calculate_shift_for_vertical(value_for_perpendicular_fun.reference_a, Values.main_points[3], Values.main_points[3], Values.line_distance_m, Values.earth_radius_m + Values.asl);
                
                main_line = new Lines(Values.main_points[0], Values.main_points[1] - b_1[2], Values.main_points[2], Values.main_points[3] + b_3[2], map, "Blue");
                main_line.draw_line();
                for (int index = 0; index < Values.how_many_lines; index++)
                {
                    
                    shift_length += Math_Formulas.calculate_radian_using_radius_and_length(Values.shif_m, Values.earth_radius_m + Values.asl);
                    double top_b = Values.main_points[3] + b_3[2];
                    double down_b = Values.main_points[1] - b_1[2];
                    double[] a_top = Math_Formulas.calculate_shift_for_perpendicular(Values.main_points[0],Values. main_points[1], value_for_perpendicular_fun.reference_a, _shift_top, Values.earth_radius_m + Values.asl);
                    double[] a_down = Math_Formulas.calculate_shift_for_perpendicular(Values.main_points[0], Values.main_points[1], value_for_perpendicular_fun.reference_a, _shift_down, Values.earth_radius_m + Values.asl);

                    _shift_top += Values.shif_m;
                    _shift_down -= Values.shif_m;
                    /*
                    Pushpin pin1 = new Pushpin();
                    Pushpin pin2 = new Pushpin();
                    Location loc1 = new Location(Math_Formulas.radian_to_degree(a_top[0]), Math_Formulas.radian_to_degree(top_b));
                    Location loc2 = new Location(Math_Formulas.radian_to_degree(a_down[0]), Math_Formulas.radian_to_degree(down_b));
                    pin1.Location = loc1;
                    map.Children.Add(pin1);
                    pin2.Location = loc2;
                    map.Children.Add(pin2);
                   */
                    mappolyline.Add(new Lines(a_top[0], top_b, a_top[0], down_b, map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();
                    mappolyline.Add(new Lines(a_down[0], top_b, a_down[0], down_b, map, "Red"));
                    mappolyline[mappolyline.Count - 1].draw_line();
                }
            }


        }
        protected double predictable_a_postions(double b_value)
        {
            return Math_Formulas.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + b_value);
        }
        
        protected double predictable_b_postions(double a)
        {
            return Math_Formulas.calculate_b_using_a_and_alpha(a, value_for_rise_fun.alpha);
        }
        public void last_points(double longitiude, double latitiude)//a, b
        {
            last_position_a = longitiude;
            last_position_b = latitiude;
            if ((last_position_b != latitiude) || (last_position_a != longitiude))
            {
                //last_position_a = longitiude;
                //last_position_b = latitiude;

                ///Debug.WriteLine($"{latitiude } {last_position_b} {last_position_b - latitiude}");
                //Debug.WriteLine("It does not the same");              
            }
            else
            {
                //Debug.WriteLine("the same1");

            }
        }
        public void add_pin(double a, double b, Pushpin pin, Map maps)
        {
            a = Math_Formulas.radian_to_degree(a);
            b = Math_Formulas.radian_to_degree(b);
            Location loc = new Location(a, b);
            pin.Location = loc;
           // maps.Children.Add(pin);


        }
            public void add_pins(double a, double b, Map map)
        {

            a = Math_Formulas.radian_to_degree(a);
            b = Math_Formulas.radian_to_degree(b);
            //Map.Children.Remove(pin);
            Location loc = new Location(a, b);
            ppoints.Add(new Pushpin());
            ppoints[ppoints.Count-1].Location = loc;
            map.Children.Add(ppoints[ppoints.Count - 1]);
        }
       
    }
}
