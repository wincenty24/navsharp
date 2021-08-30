using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace navsharp
{
    class Slayer:Values
    {
        DispatcherTimer timer = new DispatcherTimer();
        
        public double[] current_point = new double[2];//a,b

        Lines main_line;
        //public double[] current_point = new double[2];
        public List<Pushpin> ppoints = new List<Pushpin>();
        public Pushpin pin_curloc = new Pushpin();
        public Pushpin pin_loc = new Pushpin();
        public Pushpin pin_loc2 = new Pushpin();
        public Pushpin pin_loc3 = new Pushpin();
        public List<Lines> mappolyline = new List<Lines>();
        public Templates tem = new Templates();


        //private
        private Map map;
        private double last_position_b = 0;
        private double last_position_a = 0;
        private double last_length_c = 0;
        private Value_For_Rise_Fun value_for_rise_fun;
        Function function;
        public Slayer(Map map)
        {
            this.map = map;
            
        }
        public void invoke_main_fun()
        {
            if (function == Function.growing)
            {
                


                double[] real_distance = Math_Formulas.real_shift_growing(current_point[0], current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);

                var w = is_track_decreasing_or_rising(real_distance[2], last_length_c);
                last_points(current_point[0], current_point[1]);
                //Debug.WriteLine($"distance: {diss[3]} {diss[3] * earth_radius_m}");
                add_pin(real_distance[0], real_distance[1], pin_loc3, map);
                // pin_loc3.ToolTip = $"{Math_Formulas.radian_to_degree(diss[0])} {Math_Formulas.radian_to_degree(diss[0])}";
                //pin_curloc.ToolTip = $"{Math_Formulas.radian_to_degree(current_point[0])} {Math_Formulas.radian_to_degree(current_point[1])}";
                double dist = preapre_distance(real_distance[3], current_point[0], current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.delta);
                Debug.WriteLine($"{dist}       {real_distance[3] * earth_radius_m}") ;
                add_pin(current_point[0], current_point[1], pin_curloc, map);
                last_length_c = real_distance[2];

            }
        }
        private void Timer_Tick(object sender, EventArgs e)//it does not work
        {
            if( function == Function.growing)
            {
              


                double[] diss = Math_Formulas.real_shift_growing(current_point[0], current_point[1], value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);

                //var w = is_track_decreasing_or_rising();
                //Debug.WriteLine($"{is_track_decreasing_or_rising()} distance: {diss[2]} {diss[2] * earth_radius_m}");
                add_pin(diss[0], diss[1], pin_loc3, map);
                // pin_loc3.ToolTip = $"{Math_Formulas.radian_to_degree(diss[0])} {Math_Formulas.radian_to_degree(diss[0])}";
                //pin_curloc.ToolTip = $"{Math_Formulas.radian_to_degree(current_point[0])} {Math_Formulas.radian_to_degree(current_point[1])}";
                add_pin(current_point[0], current_point[1], pin_curloc, map);
                last_length_c = diss[2];
                last_points(current_point[0], current_point[1]);
            }
            
            //draw_tracking_line(current_point[0], current_point[1], map);
        }

        public void validation( double[] cur_points)
        {
            int fun = tem.validate_points_and_template(ref main_points);
            function = (Function)Enum.Parse(typeof(Function), fun.ToString());

            last_position_a = cur_points[0];
            last_position_b = cur_points[1];

            if (value_for_rise_fun.validated_function != 4)//napraw to
            {
                double bx = Math_Formulas.calculate_b(main_points);
                value_for_rise_fun.alpha = Math_Formulas.calculate_alpha(main_points[0], bx);
                value_for_rise_fun.beta = Math.Acos(Math.Sin(value_for_rise_fun.alpha) * Math.Cos(bx));

                if (function == Function.growing)//fun rośnie
                {
                    value_for_rise_fun.delta = tem.template_for_one(bx, main_points[1]);
                }
            }


            map.Heading = - Math_Formulas.radian_to_degree( value_for_rise_fun.beta);
            ///Debug.WriteLine($"{main_points[0]} {main_points[1]} {main_points[2]} {main_points[3]}");
            main_line = new Lines(main_points[0], main_points[1], main_points[2], main_points[3], map);
            //Debug.WriteLine($"{main_points[0]} {main_points[1]} {main_points[2]} {main_points[3]}");
            main_line.draw_line();
            //timer.Start();
            pin_curloc.Name = "curr";
            map.Children.Add(pin_curloc);
            map.Children.Add(pin_loc);
            map.Children.Add(pin_loc2);
            map.Children.Add(pin_loc3);

            add_lines();
        }
        public string is_track_decreasing_or_rising(double current_c_length, double last_c_length)
        {
            if (function == Function.growing)
            {

            if (last_c_length < current_c_length) { 
                Debug.WriteLine("growing");
                return "growing";
            }
            else if (last_c_length > current_c_length)
            {
                Debug.WriteLine("decreasing");
                return "decreasing";
            }
            else
            {
                Debug.WriteLine("the same");
                return "the same";
            }
        }
        return "null";
            
        }
        private double preapre_distance(double real_distance,double curr_loc_a, double curr_loc_b, double alpha, double delta)
        {
            double dist = Math_Formulas.distance_to_line(real_distance * earth_radius_m, shif_m);
            if (function == Function.growing)
            {
                double a = Math_Formulas.calculate_a(alpha, delta + curr_loc_b);

                if (curr_loc_a > a)
                {
                    return (-1) * dist;
                }
            }


            return dist;
        } 
        private void add_lines()
        {
            if(function == Function.growing)
            {
                double shift_length = 0;

                for (int index = 0; index < how_many_lines; index++)
                {
                    shift_length +=Math_Formulas.calculate_radian_using_radius_and_length(shif_m, earth_radius_m + asl);



                    double current_c_down = Math_Formulas.calculate_c_using_a_and_b(main_points[0], main_points[1] + value_for_rise_fun.delta);
                    double current_c_top = Math_Formulas.calculate_c_using_a_and_b(main_points[2], main_points[3] + value_for_rise_fun.delta);
                    double shifted_c_down = current_c_down - Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, earth_radius_m + asl);
                    double shifted_c_top = current_c_top + Math_Formulas.calculate_radian_using_radius_and_length(line_distance_m, earth_radius_m + asl);
                    double shifted_a_down = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha);
                    double shifted_b_down = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_down, value_for_rise_fun.alpha) - value_for_rise_fun.delta;
                    double shifted_a_top = Math_Formulas.calculate_a_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha);
                    double shifted_b_top = Math_Formulas.calculate_b_using_c_and_alpha(shifted_c_top, value_for_rise_fun.alpha) - value_for_rise_fun.delta;

                    double[] down_growing_val = Math_Formulas.calculate_shift_down(shifted_a_down, shifted_b_down, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    double[] top_growing_val = Math_Formulas.calculate_shift_down(shifted_a_top, shifted_b_top, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    mappolyline.Add(new Lines(top_growing_val[0], top_growing_val[1], down_growing_val[0], down_growing_val[1], map));
                    mappolyline[mappolyline.Count - 1].draw_line();
                    //print_lines(top_growing_val[0], top_growing_val[1], down_growing_val[0], down_growing_val[1]);

                    double[] top_decreasing_val = Math_Formulas.calculate_shift_up(shifted_a_down, shifted_b_down, shift_length, value_for_rise_fun.alpha,value_for_rise_fun.beta, value_for_rise_fun.delta);
                    double[] down_decreasing_val = Math_Formulas.calculate_shift_up(shifted_a_top, shifted_b_top, shift_length, value_for_rise_fun.alpha, value_for_rise_fun.beta, value_for_rise_fun.delta);
                    //print_lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1]);
                    mappolyline.Add(new Lines(top_decreasing_val[0], top_decreasing_val[1], down_decreasing_val[0], down_decreasing_val[1], map));
                    mappolyline[mappolyline.Count - 1].draw_line();


                }

            }

           
        }

        private void print_lines(double a1, double b1, double a2, double b2)
        {
            MapPolyline polyline = new MapPolyline();
            polyline.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            polyline.StrokeThickness = 1;
            polyline.Opacity = 0.7;

             polyline.Locations = new LocationCollection() {
                              new Location(Math_Formulas.radian_to_degree(a1),Math_Formulas.radian_to_degree(b1)),
                new Location(Math_Formulas.radian_to_degree(a2),Math_Formulas.radian_to_degree(b2)) };
            //Location loc = new Location(Math_Formulas.radian_to_degree(a), Math_Formulas.radian_to_degree(b));

            //Pushpin pin = new Pushpin();
            //pin.Location = loc;
            map.Children.Add(polyline);
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
