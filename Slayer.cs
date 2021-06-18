using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace navsharp
{
    class Slayer 
    {
        public struct Value_For_Rise_Fun
        {
            public double validated_function;//raz brane
            public double alpha;//wiele razy
            //public double bx;//wiele
            public double delta;//wiele
        };
        private  double last_position_b = 0;
        private double last_position_a = 0;

        private bool is_first_draw_line = true;
        public double[] main_points = new double[4];
        //public double[] current_point = new double[2];
        public Value_For_Rise_Fun value_for_rise_fun;
        public List<Pushpin> ppoints = new List<Pushpin>();
        public List<MapPolyline> mappolyline = new List<MapPolyline>();
        public Templates tem = new Templates();
        public Math_Formulas mf = new Math_Formulas();

        public void validation( double[] cur_points)
        {


            //var t = tem.validate_points_and_template(ref main_points);
            value_for_rise_fun.validated_function = tem.validate_points_and_template(ref main_points);
            last_position_a = cur_points[0];
            last_position_b = cur_points[1];

            if (value_for_rise_fun.validated_function != 4)
            {
                double bx = mf.calculate_b(main_points);
                value_for_rise_fun.alpha = mf.calculate_alpha(main_points[0], bx);

                if (value_for_rise_fun.validated_function == 0)//fun rośnie
                {
                    value_for_rise_fun.delta = tem.template_for_one(bx, main_points[1]);
                }
            }
        }
        public double predictable_a_postions(double b_value)
        {
            return mf.calculate_a(value_for_rise_fun.alpha, value_for_rise_fun.delta + b_value);
        }
        
        public void draw_tracking_line(double longitiude, double latitiude, Map map)//a, b
        {
            if ((last_position_b != latitiude) || (last_position_a != longitiude))
            {
                add_lines(longitiude, latitiude, map);
                add_pins(longitiude, latitiude, map);
                last_position_a = longitiude;
                last_position_b = latitiude;

                ///Debug.WriteLine($"{latitiude } {last_position_b} {last_position_b - latitiude}");
                //Debug.WriteLine("It does not the same");
               
                
            }
            else
            {
                //Debug.WriteLine("the same1");

            }
        }

        public void add_lines(double a, double b, Map map)
        {


            LocationCollection loccol = new LocationCollection()
            {
                new Location(mf.radian_to_degree(last_position_a), mf.radian_to_degree(last_position_b)),
                new Location(mf.radian_to_degree(a), mf.radian_to_degree(b))

                
            };
            //Debug.WriteLine(mf.radian_to_degree(last_position_a) +"   "+ mf.radian_to_degree(last_position_b));
            //Debug.WriteLine(mf.radian_to_degree(a) + " a " + mf.radian_to_degree(b));
            mappolyline.Add(new MapPolyline());
            //MapPolyline polyline = new MapPolyline();
            mappolyline[mappolyline.Count-1].Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
           //mappolyline[mappolyline.Count - 1].StrokeThickness = 5;
            mappolyline[mappolyline.Count - 1].Opacity = 0.7;
            mappolyline[mappolyline.Count - 1].Locations = new LocationCollection()
            {
                new Location(mf.radian_to_degree(last_position_a), mf.radian_to_degree(last_position_b)),
                new Location(mf.radian_to_degree(a), mf.radian_to_degree(b))


            };
            map.Children.Add(mappolyline[mappolyline.Count - 1]);



        }
            public void add_pins(double a, double b, Map map)
        {

            a = mf.radian_to_degree(a);
            b = mf.radian_to_degree(b);
            //Map.Children.Remove(pin);
            Location loc = new Location(a, b);
            ppoints.Add(new Pushpin());
            ppoints[ppoints.Count-1].Location = loc;
            map.Children.Add(ppoints[ppoints.Count - 1]);
            Debug.WriteLine("add_pins");
            
            //counter += 1;

        }
       
    }
}
