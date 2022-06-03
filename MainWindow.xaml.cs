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
using System.Windows.Forms;
using System.IO.Ports;
using System.Numerics;

namespace navsharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //testing
        Stopwatch startwatch;
        double stop = 0;


        Slayer slayer;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer_controller = new DispatcherTimer();
        MapPolygon polygon = new MapPolygon();
        SerialBoss sb= new SerialBoss();
        Controller controller = new Controller();
        MapPolyline polyline = new MapPolyline();
        LocationCollection points = new LocationCollection();
        List<LocationCollection> list_location_collection = new List<LocationCollection>();
        List<MapPolyline> list_MapPolyline = new List<MapPolyline>();
        Pushpin main_pin = new Pushpin();
        Pushpin a_pin = new Pushpin();
        Pushpin b_pin = new Pushpin();

        bool is_sent = false;
        bool is_working = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        // Testing mode
        private const bool testing_connect2controller = false;
        
        void prepare_testing_mode()
        {
            choose_controller_port.Visibility = Visibility.Hidden;
        }


        // end of testing mode
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Debug.WriteLine(sw.ElapsedMilliseconds);

            //sw = Stopwatch.StartNew();
            TimeSpan ts = startwatch.Elapsed;
            double delta = ts.TotalMilliseconds - stop;
            
            //Debug.WriteLine($"Elapsed Time {delta}");
            stop = ts.TotalMilliseconds;
            DOIT();

        }
        private void draw_trace()
        {
            //Debug.WriteLine($"list_location_collection:{list_location_collection.Count} Values.num_track_line:{Values.num_track_line}");
            list_location_collection[Values.num_track_line].Add(new Location(Math_Formulas.radian_to_degree(Values.current_point[0]), Math_Formulas.radian_to_degree(Values.current_point[1])));
            list_MapPolyline[Values.num_track_line].Locations = list_location_collection[Values.num_track_line];
        }
        private void calculate_show_distance()
        {
            if (slayer != null)
            {
                if ( slayer.IsValidated())
                {
                    Values.value2return v2r = slayer.calculate_distance(Values.compass, Values.current_point);
                    Values.angle = v2r.angle;
                    if (v2r.distance > 0)
                    {
                        if (v2r.dir == Values.Direction.growing)
                        {
                            arrow_left.Visibility = Visibility.Collapsed;
                            arrow_right.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            arrow_left.Visibility = Visibility.Visible;
                            arrow_right.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (v2r.distance < 0)
                    {
                        if (v2r.dir == Values.Direction.decreasing)
                        {
                            arrow_left.Visibility = Visibility.Collapsed;
                            arrow_right.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            arrow_left.Visibility = Visibility.Visible;
                            arrow_right.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        arrow_left.Visibility = Visibility.Collapsed;
                        arrow_right.Visibility = Visibility.Collapsed;
                    }
                    distance_textblock.Text = string.Format("{0:0.00}", Math.Abs(v2r.distance).ToString());
                    draw_trace();
                }
            }
        }
        private void DOIT()//get data from satellite
        {
            if (sb.is_port_available())
            {
                double[] val = sb.prepared_data();
                Values.current_point[0] = Math_Formulas.degree_to_radian( val[0]);
                Values.current_point[1] = Math_Formulas.degree_to_radian(val[1]);
                Debug.WriteLine($"{Values.current_point[0]}");
                Location location = new Location(Math_Formulas.radian_to_degree(Values.current_point[0]), Math_Formulas.radian_to_degree(Values.current_point[1]));
                main_pin.Location = location;
                Values.compass = 0;//Math_Formulas.degree_to_radian(val[2]);
                if (Values.centerize)
                {
                    main_map.Center = location;
                }
                if (Values.heading)
                {
                    main_map.Heading = -Math_Formulas.radian_to_degree(Values.compass);
                }
                calculate_show_distance();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            check_availabale_ports();
            load_ports_automatically();
            load_values_from_memory();


            //ts.Minutes, ts.Seconds, ts.Milliseconds startwatch = new Stopwatch();
            startwatch = new Stopwatch();
            startwatch.Start();

            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();

            if (testing_connect2controller)
            {
                timer_controller.Interval = TimeSpan.FromMilliseconds(50);
                timer_controller.Tick += Timer_controller_Tick;
                timer_controller.Start();
            }
            else
            {
                prepare_testing_mode();
            }

            main_map.Children.Add(main_pin);
            //assign_start_point_A();
            //assign_start_point_B();


        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                weight_textbox.Text = convert(weight_textbox.Text);
                length_textbox.Text = convert(length_textbox.Text);
                look_ahead_textbox.Text = convert(look_ahead_textbox.Text);
                lines_to_draw_textbox.Text = convert(lines_to_draw_textbox.Text);
                look_ahead_to_lines_textbox.Text = convert(look_ahead_to_lines_textbox.Text);


                Properties.Settings.Default.weight_save = Convert.ToDouble(convert(weight_textbox.Text));
                Properties.Settings.Default.length_save = Convert.ToDouble(convert(length_textbox.Text));
                Properties.Settings.Default.look_ahead_save = Convert.ToDouble(convert(look_ahead_textbox.Text));
                Properties.Settings.Default.lines_to_draw_save = Convert.ToInt32(convert(lines_to_draw_textbox.Text));
                Properties.Settings.Default.look_ahead_to_lines_save = Convert.ToDouble(convert(look_ahead_to_lines_textbox.Text));
                Properties.Settings.Default.Save();


                Values.shif_m = Convert.ToDouble(Properties.Settings.Default.weight_save.ToString());
                Values.machine_length = Convert.ToDouble(Properties.Settings.Default.length_save.ToString());
                Values.look_ahead_m = Convert.ToDouble(Properties.Settings.Default.look_ahead_save.ToString());
                Values.how_many_lines = Convert.ToInt32(Properties.Settings.Default.lines_to_draw_save.ToString());
                Values.line_distance_m = Convert.ToDouble(Properties.Settings.Default.look_ahead_to_lines_save.ToString());
                //Debug.WriteLine($"Values.look_ahead_m: {Values.look_ahead_m}");
            }
            catch
            {
                System.Windows.MessageBox.Show("Something went wrong!");
            }
        }
        private void load_values_from_memory()
        {
            weight_textbox.Text = Properties.Settings.Default.weight_save.ToString();
            length_textbox.Text = Properties.Settings.Default.length_save.ToString();
            look_ahead_textbox.Text = Properties.Settings.Default.look_ahead_save.ToString();
            lines_to_draw_textbox.Text = Properties.Settings.Default.lines_to_draw_save.ToString();
            look_ahead_to_lines_textbox.Text = Properties.Settings.Default.look_ahead_to_lines_save.ToString();

            Values.shif_m = Convert.ToDouble(Properties.Settings.Default.weight_save.ToString());
            Values.machine_length = Convert.ToDouble(Properties.Settings.Default.length_save.ToString());
            Values.look_ahead_m = Convert.ToDouble(Properties.Settings.Default.look_ahead_save.ToString());
            Values.how_many_lines = Convert.ToInt32(Properties.Settings.Default.lines_to_draw_save.ToString());
            Values.line_distance_m = Convert.ToDouble(Properties.Settings.Default.look_ahead_to_lines_save.ToString());


        }
        private void load_ports_automatically()
        {
            string controller_port = Properties.Settings.Default.controller_port_save.ToString();
            string satellite_port = Properties.Settings.Default.satellite_module_port_save.ToString();

            if (check_ports(controller_port, satellite_port))
            {
                controller.set(controller_port, 9600);
                sb.set(satellite_port, 460800);

            }
            else
            {
                //controller.set("COM4", 9600);
               // sb.set("COM3", 9600);

                System.Windows.MessageBox.Show("Can't connect with a ports!");
            }
        }
        private bool check_ports(string controller_port, string satellite_port)
        {
            string[] ports = SerialPort.GetPortNames();
            bool is_controller_port_available = false;
            bool is_satellite_port_available = false;
            foreach (string port in ports)
            {
                if(port == controller_port)
                {
                    is_controller_port_available = true;
                }

                if(port == satellite_port)
                {
                    is_satellite_port_available = true;
                }
            }
            return is_controller_port_available & is_satellite_port_available;
        }
        public void assign_start_point_A()
        {
            // -- 51.203977, 17.312593
            // || 51.208477 17.312763
            // / 51.238247, 17.375997
            // \ 51.230939, 17.361992
            double as1 = Values.current_point[0]; //Math_Formulas.degree_to_radian(51.203977);
            double bs1 = Values.current_point[1];// Math_Formulas.degree_to_radian(17.312593);
            //main_map.Center = new Location(Math_Formulas.radian_to_degree(as1), Math_Formulas.radian_to_degree(bs1));
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
            double as2 = Values.current_point[0];//Math_Formulas.degree_to_radian(51.203977);
            double bs2 = Values.current_point[1]; //Math_Formulas.degree_to_radian(17.312763);
            Values.main_points[2] = as2;
            Values.main_points[3] = bs2;

            //Values.current_point[0] = Values.main_points[0];
            //Values.current_point[1] = Values.main_points[1];
            //prepare_fake_coordinates();
        }

        private void Timer_controller_Tick(object sender, EventArgs e)
        {            
            if (controller.is_port_open())
            {
                double angle = Math_Formulas.prepare_wheel_angle(Values.angle, 50, -50);               
                string data = ((int)(Math_Formulas.radian_to_degree(angle))).ToString().Replace(",", ".");
                int work = is_working ? 1 : 0;
                //Debug.WriteLine($"data: {data} {work}");
                controller.send($"{data} {work}");
                //controller.send(((-10)* Math.Round(Math_Formulas.radian_to_degree(Values.distance)),2).ToString().Replace(",", ".").Replace("(", "").Replace(")", "").Replace(" ", ""));
            }
            
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
            main_map.Heading = - Math_Formulas.radian_to_degree( slayer.value_for_rise_fun.beta);
            Values.compass = slayer.value_for_rise_fun.beta;
            //if (slayer.IsValidated())
            //{
              //  double[] dist = slayer.Invoke_main_fun();
               // Debug.WriteLine(dist[0]);
           // }
            // Adds the pushpin to the map.

        }

        private void main_map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("click");
        }

        private void angle_plus_Click(object sender, RoutedEventArgs e)
        {

            //prepare_fake_coordinates();
            if (angle_plus.Visibility != Visibility.Hidden)
            {
                angle_plus.Visibility = Visibility.Hidden;
            }
        }
        private void prepare_fake_coordinates()
        {

            string data = $"{Math_Formulas.radian_to_degree(Values.main_points[0])} {Math_Formulas.radian_to_degree(Values.main_points[1])} {Math_Formulas.radian_to_degree(slayer.value_for_rise_fun.beta)} ";
            data = data.Replace(",", ".");
            //sb.send(data);
            //Debug.WriteLine(data);
            sb.create_fake_coordinates(Values.current_point[0], Values.current_point[1], slayer.value_for_rise_fun.beta);
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void check_availabale_ports()
        {
            string[] ports = SerialPort.GetPortNames();
            satelite_port.Items.Clear();
            controller_port.Items.Clear();
            if (ports.Length > 0)
            {
                foreach (string port in ports)
                {
                    satelite_port.Items.Add(port);
                    controller_port.Items.Add(port);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Did not find any available ports!");
            }
            

        }
        private void Refresh_com_port_Click(object sender, RoutedEventArgs e)
        {
            check_availabale_ports();
        }

        private void satelite_port_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void controller_port_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private string convert(string val)
        {
            return val.Replace(".", ",");
        }

        private void choose_controller_port_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                Controller_port_name_textblock.Text = controller_port.SelectedItem.ToString();
                controller.set(Controller_port_name_textblock.Text, 9600);
            }
            catch
            {
                System.Windows.MessageBox.Show("Something went wrong, during connecting to controller's port");
            }
        }

        private void Choose_satelites_port_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Satelite_port_name_textblock.Text = satelite_port.SelectedItem.ToString();
                sb.set(Satelite_port_name_textblock.Text, 460800);
            }
            catch
            {
                System.Windows.MessageBox.Show("Something went wrong, during connecting to satellite's port");
            }
        }

        private void angle_minus_Click(object sender, RoutedEventArgs e)
        {
            // Calculate the points to make up a circle with radius of 200 miles

        }

        private void look_ahead_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void new_points_button_Click(object sender, RoutedEventArgs e)
        {
            //angle_plus.Visibility = Visibility.Visible;
            asnc_message();
            resume_button.IsEnabled = false;
            stop_button.IsEnabled = false;
            is_sent = true;
        }
        private void new_obj()
        {
            is_working = false;
            Values.num_track_line = 0;
            main_map.Children.Clear();
            list_MapPolyline.Clear();
            list_location_collection.Clear();
            load_values_from_memory();
            main_map.ZoomLevel = 18;
            main_map.Children.Add(main_pin);
            slayer = new Slayer(main_map, Values.shif_m);
        }
        private void asnc_message()
        {
            if(System.Windows.MessageBox.Show("Would you like to create new line?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {

                new_obj();
                if (!is_sent)
                {
                    //prepare_fake_coordinates();
                    is_sent = true;
                }
                a_point_button.IsEnabled = true;
            }
            else
            {
                a_point_button.IsEnabled = false;
                b_point_button.IsEnabled = false;
            }
        }

        private void a_point_button_Click(object sender, RoutedEventArgs e)
        {
            a_point_button.IsEnabled = false;
            b_point_button.IsEnabled = true;
            assign_start_point_A();
        }

        private void change_main_points()
        {
            Values.heading = false;
            Values.centerize = false;
            centerize_togglebutton.IsEnabled = false;
             
            main_map.Heading = 0;

            a_point_button.Visibility = Visibility.Visible;
            change_a_plus_button.Visibility = Visibility.Visible;
            change_a_minus_button.Visibility = Visibility.Visible;
            change_b_minus_button.Visibility = Visibility.Visible;
            change_b_plus_button.Visibility = Visibility.Visible;
            choose_point_to_change_val_button.Visibility = Visibility.Visible;
            //a_point_button.IsEnabled = true;
            main_map.Children.Remove(main_pin);
            main_map.Children.Add(a_pin);
            a_pin.Content = "A";
            a_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            main_map.Center = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            main_map.Children.Add(b_pin);
            b_pin.Content = "B";
            b_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
        }
        private void b_point_button_Click(object sender, RoutedEventArgs e)
        {
            b_point_button.IsEnabled = false;
            assign_start_point_B();
            if (System.Windows.MessageBox.Show("Would you like to coorect points?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                change_main_points();
                start_button.IsEnabled = true;
            }
            else
            {
                start_button.IsEnabled = true;
            }

        }

        private void zoom_plus_button_Click(object sender, RoutedEventArgs e)
        {
            main_map.ZoomLevel++;
        }

        private void zoom_minus_button_Click(object sender, RoutedEventArgs e)
        {
            main_map.ZoomLevel--;
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void choose_point_to_change_val_button_Checked(object sender, RoutedEventArgs e)
        {
            choose_point_to_change_val_button.Content = "A";
            if(!Values.centerize)
                main_map.Center = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
        }

        private void choose_point_to_change_val_button_Unchecked(object sender, RoutedEventArgs e)
        {
            choose_point_to_change_val_button.Content = "B";
            if (!Values.centerize)
                main_map.Center = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
           
            //start_button.Visibility = Visibility.Hidden;
            change_a_plus_button.Visibility = Visibility.Hidden;
            change_a_minus_button.Visibility = Visibility.Hidden;
            change_b_minus_button.Visibility = Visibility.Hidden;
            change_b_plus_button.Visibility = Visibility.Hidden;
            choose_point_to_change_val_button.Visibility = Visibility.Hidden;
            stop_button.Visibility = Visibility.Visible;

            if (!main_pin.IsLoaded)
            {
                main_map.Children.Add(main_pin);
            }
            if (a_pin.IsLoaded)
            {
                main_map.Children.Remove(a_pin);
            }
            if (main_map.IsLoaded)
            {
                main_map.Children.Remove(b_pin);
            }
            if (!Values.heading)
            {
                Values.heading = true;
            }
            if (!Values.centerize)
            {
                Values.centerize = true;
            }
            if (centerize_togglebutton.IsEnabled == false )
            {
                centerize_togglebutton.IsEnabled = true;
            }
            
            slayer.validation(ref Values.main_points, Values.line_distance_m, Values.earth_radius_m + Values.asl);
            is_working = true;
            start_button.IsEnabled = false;
            stop_button.IsEnabled = true;
            Lines.add_mappolyline(main_map, 0, ref list_location_collection, ref list_MapPolyline);

        }

        private void change_a_plus_button_Click(object sender, RoutedEventArgs e)
        {
            if (choose_point_to_change_val_button.IsChecked == true)
            {
                Values.main_points[0] += Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m+ Values.asl);
                a_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            }
            else
            {
                Values.main_points[2] += Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                b_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
            }
        }

        private void change_a_minus_button_Click(object sender, RoutedEventArgs e)
        {
            if (choose_point_to_change_val_button.IsChecked == true)
            {
                Values.main_points[0] -= Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                a_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            }
            else
            {
                Values.main_points[2] -= Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                b_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
            }
        }

        private void change_b_plus_button_Click(object sender, RoutedEventArgs e)
        {
            if (choose_point_to_change_val_button.IsChecked == true)
            {
                Values.main_points[1] += Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                a_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            }
            else
            {
                Values.main_points[3] += Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                b_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
            }
        }

        private void change_b_minus_button_Click(object sender, RoutedEventArgs e)
        {
            if (choose_point_to_change_val_button.IsChecked == true)
            {
                Values.main_points[1] -= Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                a_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[0]), Math_Formulas.radian_to_degree(Values.main_points[1]));
            }
            else
            {
                Values.main_points[3] -= Math_Formulas.distance_to_radian(0.1, Values.earth_radius_m + Values.asl);
                b_pin.Location = new Location(Math_Formulas.radian_to_degree(Values.main_points[2]), Math_Formulas.radian_to_degree(Values.main_points[3]));
            }
        }

        private void centerize_togglebutton_Unchecked(object sender, RoutedEventArgs e)
        {
            Values.centerize = false;
            Values.heading = false;
        }

        private void centerize_togglebutton_Checked(object sender, RoutedEventArgs e)
        {
            Values.centerize = true;
            Values.heading = true;
        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            is_working = false;
            resume_button.IsEnabled = true;
            stop_button.IsEnabled = false;
            
        }

        private void resume_button_Click(object sender, RoutedEventArgs e)
        {
            //list_location_collection.Add(new LocationCollection());
            //list_MapPolyline.Add(new MapPolyline());
            Values.num_track_line++;
            //add_mappolyline(Values.num_track_line);
            //Debug.WriteLine($"list_MapPolyline:{list_MapPolyline.Count}  Values.num_track_line:{ Values.num_track_line}");
            Lines.add_mappolyline(main_map, Values.num_track_line, ref list_location_collection, ref list_MapPolyline);
            //main_map.Children.Add(list_MapPolyline[Values.num_track_line]);
            is_working = true;

            resume_button.IsEnabled = false;
            stop_button.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void clean()
        {
            Values.num_track_line = 0;
            slayer = null;
            list_location_collection.Clear();
            list_MapPolyline.Clear();
        }

        private void save_file_button_Click(object sender, RoutedEventArgs e)
        {
            Saver_reader.save(list_location_collection, Values.main_points);
        }

        private void read_fie_button_Click(object sender, RoutedEventArgs e)
        {
            //angle_plus.Visibility = Visibility.Visible;
            if (System.Windows.MessageBox.Show("Would you like to read points?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                new_obj();
                if (!is_sent)
                {
                    //prepare_fake_coordinates();
                    is_sent = true;
                }
                Saver_reader.read(ref list_location_collection, ref Values.main_points, ref Values.num_track_line);
                //Debug.WriteLine($"Values.num_track_line after reading:{Values.num_track_line} {list_MapPolyline.Count}");
                for (int i = 0; i < Values.num_track_line; i++)
                {
                    Lines.add_mappolyline(main_map, i, ref list_MapPolyline);
                    list_MapPolyline[i].Locations = list_location_collection[i];
                }
                Values.num_track_line--;
                //Debug.WriteLine($"Values.num_track_line after creating new polyline:{Values.num_track_line} {list_MapPolyline.Count}");
                resume_button.IsEnabled = true;
                slayer.validation(ref Values.main_points, Values.line_distance_m, Values.earth_radius_m + Values.asl);

            }
            else
            {
             
            }
            
        }

      
    }
} 