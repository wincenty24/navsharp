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
        public MainWindow()
        {
            InitializeComponent();

            main_map.Children.Add(pin);

            /*
            //MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            */


            //double bs1 = mf.degree_to_radian(17.376002);
            //double bs2 = mf.degree_to_radian(17.379699);//  # 18
            //double as1 = mf.degree_to_radian(51.238248);//  # 51
            //double as2 = mf.degree_to_radian(51.240980);//  # 52
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DOIT();
        }

        private void DOIT()
        {

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
            slayer.current_point[1] -= 0.0000007;
        }

        private void button_b_plus_Click(object sender, RoutedEventArgs e)
        {
            slayer.current_point[1] += 0.0000007;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            slayer = new Slayer(main_map);
            assign_start_point_A();
            assign_start_point_B();
            slayer.current_point[0] = slayer.main_points[0];
            slayer.current_point[1] = slayer.main_points[1];

        }
        public void assign_start_point_A()
        {
            //51,230797, 17,361655
            double as1 = Math_Formulas.degree_to_radian(51.225702);
            double bs1 = Math_Formulas.degree_to_radian(17.372333);
            //51,225702, 17,372333
            //double as1 = math.degree_to_radian(51.141572);
            //double bs1 = math.degre e_to_radian(17.224826);
            slayer.main_points[0] = as1;
            slayer.main_points[1] = bs1;
        }
        public void assign_start_point_B()
        {
            //51,227277, 17,372381
            double as2 = Math_Formulas.degree_to_radian(51.227277);
            double bs2 = Math_Formulas.degree_to_radian(17.372333);
            //double bs2 = math.degree_to_radian(17.223516);
            //double as2 = math.degree_to_radian(51.141479);


            slayer.main_points[2] = as2;
            slayer.main_points[3] = bs2;

            slayer.validation(slayer.current_point);

            main_map.Center = new Location(51.230415, 17.362015);
            main_map.ZoomLevel = 19;
            
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


            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();


        }

        private void button_a_minus_Click(object sender, RoutedEventArgs e)
        {
            slayer.current_point[0] -= 0.0000007;
        }

        private void button_a_plus_Click(object sender, RoutedEventArgs e)
        {
            slayer.current_point[0] += 0.0000007;

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
            slayer.current_point[0] = Math_Formulas.degree_to_radian(Convert.ToDouble(val1));
            slayer.current_point[1] = Math_Formulas.degree_to_radian(Convert.ToDouble(val2));
            slayer.invoke_main_fun();
            // Adds the pushpin to the map.

        }

        private void main_map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("click");
        }
    }
}
