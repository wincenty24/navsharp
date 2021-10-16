using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Maps.MapControl.WPF;
using System.Timers;
using System.Windows.Threading;
using System.Globalization;

namespace navsharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public int counter = 0;
        Slayer slayer;
        DispatcherTimer timer = new DispatcherTimer();
        MapPolygon polygon = new MapPolygon();
        Pushpin pin = new Pushpin();
        SerialBoss sb = new SerialBoss();
        MapPolyline polyline = new MapPolyline();
        LocationCollection locations;
        LocationCollection points = new LocationCollection();
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DOIT();
        }
        bool asd = false;
        private void DOIT()
        {
            if (sb.is_port_available())
            {
                double[] val = sb.prepared_data();
                //Debug.Write($"{Math_Formulas.radian_to_degree(val[2])} {Math_Formulas.radian_to_degree(val[2])}  ");
                points.Add(new Location(Math_Formulas.radian_to_degree(Values.current_point[0]), Math_Formulas.radian_to_degree(Values.current_point[1])));
                polyline.Locations = points;
                main_map.Center = new Location(Math_Formulas.radian_to_degree(Values.current_point[0]), Math_Formulas.radian_to_degree(Values.current_point[1]));

                main_map.Heading = -Math_Formulas.radian_to_degree(val[2]);
                                //main_map.SetView(new Location(Math_Formulas.radian_to_degree(Values.current_point[0]), Math_Formulas.radian_to_degree(Values.current_point[1])), main_map.ZoomLevel);
                if (!asd)
                {
                    main_map.Children.Add(pin);
                    polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                    polyline.StrokeThickness = 5;
                    polyline.Opacity = 0.7;
                    main_map.Children.Add(polyline);
                    asd = true;
                }
                if (slayer.is_validated)
                {
                    double[] dist = slayer.invoke_main_fun();
                    double l = Math_Formulas.calculate_radian_using_radius_and_length(3.0, Values.earth_radius_m + Values.asl);
                   // slayer.calculate_angle(dist[0], l, slider2.Value);
                    Debug.WriteLine($"{dist[0]} {dist[1]}");
                }
                Values.current_point[0] = val[0];
                Values.current_point[1] = val[1];
            }
            /*
         if (sb.is_port_available())
         {
             string data = sb.data;
             string[] splitted_data = data.Split(' ');
             NumberFormatInfo provider = new NumberFormatInfo();
             provider.NumberDecimalSeparator = ".";
             provider.NumberGroupSeparator = ",";
             double a1 = Convert.ToDouble(splitted_data[0], provider);
             double b1 = Convert.ToDouble(splitted_data[1], provider);
             double angl = Convert.ToDouble(splitted_data[2], provider);
             //double b1 = Convert.ToDouble(splitted_data[1]);
             //double angle1 = Convert.ToDouble(splitted_data[2]);
             Debug.WriteLine($"{a1} {b1} {angl}");
             slayer.draw_route((a1), (b1), (angl));
         }
         */
            //slayer.draw_tracking_line(slayer.current_point[0], slayer.current_point[1], main_map);
            //double a = slayer.predictable_a_postions(slayer.current_point[1]);

            //double a = mf.calculate_a(alpha, delta + current_point[1]);
            /*
            double next_shift = Math_Formulas.length_angle_sift(100, 637100000);// <----to jest stałe
            double b = slayer.current_point[1];
            double[] up_points = new double[2];
            up_points = Math_Formulas.shift_up(slayer.value_for_rise_fun.alpha, next_shift) ;
            double qwe = b - up_points[0];
            double ewq = a + up_points[1];
            
           

            //double shif = math.calculate_b_using_a_and_alpha(next_shift, Math.PI/2 - slayer.value_for_rise_fun.alpha);// <----to też jest stałe
            */

            //double aa = a + math.shift(shif, slayer.value_for_rise_fun.alpha);// <----
            //double radian_real_shift = math.real_shift(aa, slayer.value_for_rise_fun.alpha);// <----i to też jest stałe
            //double real_sh = math.length_sift(radian_real_shift, 637100000);// <---- to również jest stałe
            //Debug.WriteLine($"real_sh {real_sh}");
        }
        

        private void button_b_minus_Click(object sender, RoutedEventArgs e)
        {
            Values.current_point[1] -= 0.0000007;
        }

        private void button_b_plus_Click(object sender, RoutedEventArgs e)
        {
            Values.current_point[1] += 0.0000007;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            slayer = new Slayer(main_map);
            assign_start_point_A();
            assign_start_point_B();
            Values.current_point[0] = Values.main_points[0];
            Values.current_point[1] = Values.main_points[1];

            
           
        }
        public void assign_start_point_A()
        {
            // -- 51.203977, 17.312593
            // || 51.208477 17.312763
            // / 51.238247, 17.375997
            // \ 51.230939, 17.361992
            double as1 = Math_Formulas.degree_to_radian(51.230939);
            double bs1 = Math_Formulas.degree_to_radian(17.361992);

            //51,225702, 17,372333
            //double as1 = math.degree_to_radian(51.141572);
            //double bs1 = math.degre e_to_radian(17.224826);
            Values.main_points[0] = as1;
            Values.main_points[1] = bs1;
        }
        public void assign_start_point_B()
        {
            // -- 51.203977, 17.312763
            // || 51.205802 17.312763
            // / 51.240987, 17.379715
            // \ 51.230131, 17.362590
            double as2 = Math_Formulas.degree_to_radian(51.230131);
            double bs2 = Math_Formulas.degree_to_radian(17.362590);
            Values.main_points[2] = as2;
            Values.main_points[3] = bs2;

            slayer.validation(Values.current_point);
            main_map.ZoomLevel = 18;
            
            /*
            //start validate
            validated_function = tem.validate_points_and_template(ref slayer.main_points);
            Debug.WriteLine(validated_function);
            if (validated_function != 4)
            {
                bx = mf.calculate_b(slayer.main_points);
                alpha = mf.calculate_alpha(slayer.main_points[0], bx);

                if(validated_function == 0)//fun rośnie
                {
                    delta = tem.template_for_one(bx, slayer.main_points[1]);
                }   
            }
            */


            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();


        }

        private void button_a_minus_Click(object sender, RoutedEventArgs e)
        {
            Values.current_point[0] -= 0.0000007;
        }

        private void button_a_plus_Click(object sender, RoutedEventArgs e)
        {
            Values.current_point[0] += 0.0000007;

            /*
            double a = math.radian_to_degree(current_point[0]);
            double b = math.radian_to_degree(current_point[1]);
            Location loc = new Location(a, b);

            polygon.Locations = l;
            main_map.Children.Add(polygon);

            */
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = main_map.ViewportPointToLocation(mousePosition);
            string loc = Convert.ToString(pinLocation);
            string[] splitted_loc = loc.Split(',');
            string val1 = $"{splitted_loc[0]},{splitted_loc[1]}";
            string val2 = $"{splitted_loc[2]},{splitted_loc[3]}";
            //slayer.current_point[0] = Convert.ToDouble(val1);
            //slayer.current_point[1] = Convert.ToDouble(val2);
           
            Location location = new Location(Convert.ToDouble(val1), Convert.ToDouble(val2));
            //pin.Location = location;
            // The pushpin to add to the map.
            Values.current_point[0] = Math_Formulas.degree_to_radian(Convert.ToDouble(val1));
            Values.current_point[1] = Math_Formulas.degree_to_radian(Convert.ToDouble(val2));

            if (slayer.is_validated)
            {
              double[] dist = slayer.invoke_main_fun();
                Debug.WriteLine(dist[0]);
            }
            // Adds the pushpin to the map.

        }

        private void main_map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("click");
        }

        private void angle_plus_Click(object sender, RoutedEventArgs e)
        {
            
            string data = $"{Math_Formulas.radian_to_degree(Values.main_points[0])} {Math_Formulas.radian_to_degree(Values.main_points[1])} {Math_Formulas.radian_to_degree(slayer.value_for_rise_fun.beta)} ";
            data = data.Replace(",", ".");
            sb.send(data);
            Debug.WriteLine(data);
            sb.create_fake_coordinates(Values.current_point[0], Values.current_point[1], slayer.value_for_rise_fun.beta) ;
        }
    }
}
