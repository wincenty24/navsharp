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
        Math_Formulas math = new Math_Formulas();
        Slayer slayer = new Slayer();
        DispatcherTimer timer = new DispatcherTimer();
        public double[] current_point = new double[2];//a,b
        MapPolygon polygon = new MapPolygon();
        

        public MainWindow()
        {
            InitializeComponent();

            MapPolyline polyline = new MapPolyline();
            polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            polyline.Opacity = 0.7;
            polyline.Locations= new LocationCollection() {
        new Location(51.238248, 17.376002),
        new Location(51.240980, 17.379699)};

            main_map.Children.Add(polyline);
            main_map.Center = new Location(51.239736, 17.377578);
            main_map.ZoomLevel = 17;
            polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
           
            
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
            double a = slayer.predictable_a_postions(current_point[1]);

            //double a = mf.calculate_a(alpha, delta + current_point[1]);
            double b = current_point[1];


            double next_shift = math.length_angle_sift(100, 637100000);// <----to jest stałe
            double shif = math.shift(next_shift, slayer.value_for_rise_fun.alpha);// <----to też jest stałe


            double aa = a + math.shift(shif, slayer.value_for_rise_fun.alpha);// <----
            double radian_real_shift = math.real_shift(aa, slayer.value_for_rise_fun.alpha);// <----i to też jest stałe
            double real_sh = math.length_sift(radian_real_shift, 637100000);// <---- to również jest stałe
            Debug.WriteLine($"real_sh {real_sh}");
            slayer.draw_tracking_line(aa, b, main_map);
        }
        
        private void button_b_minus_Click(object sender, RoutedEventArgs e)
        {
            current_point[1] -= 0.0000007;
        }

        private void button_b_plus_Click(object sender, RoutedEventArgs e)
        {
            current_point[1] += 0.0000007;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            assign_start_point_A();
            assign_start_point_B();
            current_point[0] = slayer.main_points[0];
            current_point[1] = slayer.main_points[1];

        }
        public void assign_start_point_A()
        {
            //51.141572, 17.224826
            double bs1 = math.degree_to_radian(17.376002);
            double as1 = math.degree_to_radian(51.238248);
            //double as1 = math.degree_to_radian(51.141572);
            //double bs1 = math.degree_to_radian(17.224826);
            slayer.main_points[0] = as1;
            slayer.main_points[1] = bs1;
        }
        public void assign_start_point_B()
        {
            //51.141479, 17.223516
            double bs2 = math.degree_to_radian(17.379699);
            double as2 = math.degree_to_radian(51.240980);
            //double bs2 = math.degree_to_radian(17.223516);
            //double as2 = math.degree_to_radian(51.141479);


            slayer.main_points[2] = as2;
            slayer.main_points[3] = bs2;

            slayer.validation(current_point);
           
            
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
            current_point[0] -= 0.0000007;
        }

        private void button_a_plus_Click(object sender, RoutedEventArgs e)
        {
            current_point[0] += 0.0000007;

            /*
            double a = math.radian_to_degree(current_point[0]);
            double b = math.radian_to_degree(current_point[1]);
            Location loc = new Location(a, b);

            polygon.Locations = l;
            main_map.Children.Add(polygon);

            */
        }
    }
}
