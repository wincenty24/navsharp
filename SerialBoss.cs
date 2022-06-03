using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace navsharp
{
 
    class SerialBoss
    {
        private Thread reader;
        private SerialPort serialPort;
        public string port { get; private set; }
        public int baud { get; private set; }

        private volatile string data_vtg;
        private volatile string data_gga;

        private bool is_prepared_gga = false;
        private bool is_prepared_vga = false;

        private static string last_Course_over_ground = "0";


        FakeCoordinates fakeCoordinates;

        public SerialBoss()
        {
            serialPort = new SerialPort();


            //set("COM3", 9600);

        }
        private double sep_lat(string ddmm_mm)
        {
            string degree_str = $"{ddmm_mm[0]}{ddmm_mm[1]}";
            string minues_str = ddmm_mm.Substring(2);
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            double degree = int.Parse(degree_str);// Convert.ToDouble(degree_str, provider);
            double minutes = Convert.ToDouble(minues_str, provider) / 60;
            return degree + minutes;
        }
        private double sep_long(string ddmm_mm)
        {
            string degree_str = $"{ddmm_mm[0]}{ddmm_mm[1]}{ddmm_mm[2]}";
            string minues_str = ddmm_mm.Substring(3);
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            double degree = int.Parse(degree_str);// Convert.ToDouble(degree_str, provider);
            double minutes = Convert.ToDouble(minues_str, provider) / 60;
            return degree + minutes;
        }
        public void set(string port, int baud)
        {
            serialPort.PortName = port;
            serialPort.BaudRate = baud;
            this.baud = baud;
            this.port = port;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            start();
        }

       public void start()
       {

            serialPort.DtrEnable = false;
            serialPort.DtrEnable = true;
            serialPort.Open();

            reader = new Thread(read)
            {
                IsBackground = true
            };
            reader.Start();

        }
        private void NMEA_VTG_spliter(string VTG_data)
        {
            string[] splitted_data = VTG_data.Split(',');
            string Course_over_ground = splitted_data[1];
            string speed_km_per_hour = splitted_data[7];
            if (Course_over_ground.Length > 0)
            {

                last_Course_over_ground = Course_over_ground;
            }
            else
            {

                Course_over_ground = last_Course_over_ground;
            }
            //Console.WriteLine(Course_over_ground.Length);
           // Console.WriteLine($"Course_over_ground:{Course_over_ground} speed_km_per_hour:{speed_km_per_hour}");
        }
        private void read()
        {

            //sender.Start();
            //Debug.WriteLine("asd");
            while (serialPort.IsOpen)
            {
                Console.WriteLine($"{is_prepared_vga} {is_prepared_gga}");
                try
                {
                    string data = serialPort.ReadLine();
                    if (data.Contains("VTG"))
                    {
                       
                        //NMEA_VTG_spliter(data);
                        data_vtg = data;
                        if (!is_prepared_vga)
                            is_prepared_vga = true;

                    }
                    else if (data.Contains("GGA"))
                    {
                        //Console.WriteLine(data);
                        //NMEA_VTG_spliter(data);
                        data_gga = data;
                        if (!is_prepared_gga)
                            is_prepared_gga = true;

                    }
                }
                catch
                {
                    string asd = serialPort.IsOpen.ToString();
                    MessageBox.Show(asd);
                    //serialPort.
                }

                    //Debug.WriteLine(a);

            }
        }
        public void send(string msg)
        {
            //serialPort.WriteLine(msg);

            //is_prepared = true;

        }
        public void create_fake_coordinates(double lat1, double lon1, double angle)
        {
            fakeCoordinates = new FakeCoordinates(lat1, lon1, angle);
        }

        public void stop()
        {
            reader.Abort();
        }
        public bool is_port_available()
        {
            return serialPort.IsOpen && is_prepared_gga == true && is_prepared_vga == true;//&& data_gga != null && data_vtg !=null;
        }
        public double[] prepared_data()
        {

            string[] splitted_data_vtg = data_vtg.Split(',');
            string Course_over_ground = splitted_data_vtg[1];
            string speed_km_per_hour = splitted_data_vtg[7];
            if (Course_over_ground.Length > 0)
            {

                last_Course_over_ground = Course_over_ground;
            }
            else
            {

                Course_over_ground = last_Course_over_ground;
            }

            string[] splitted_data_gga = data_gga.Split(',');
            string Latitude = splitted_data_gga[2];
            string Longitude = splitted_data_gga[4];
            string type_of_rtk = splitted_data_gga[6];


            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            double a1 = sep_lat(Latitude);
            double b1 = sep_long(Longitude);
            double angle = Convert.ToDouble(Course_over_ground, provider);
            double speed = Convert.ToDouble(speed_km_per_hour, provider);
            //Debug.WriteLine($"{a1} {b1} angle:{angle}");
            /*
             * fake coordinates
             * 
             */
            //fakeCoordinates.angle = prepare_compass(fakeCoordinates.angle);
            //double angle2 = fakeCoordinates.angle += (Math_Formulas.degree_to_radian(angle)/5);
            //double[] cd = fakeCoordinates.calculate_coordinates(Values.earth_radius_m + Values.asl, Math_Formulas.calculate_radian_using_radius_and_length(speed, Values.earth_radius_m + Values.asl));
            double[] ret = { a1, b1, angle, speed};
            return ret;
        }
        private double prepare_compass(double val)
        {

            val = val % (2 * Math.PI);
            if(val < 0)
            {
                return val + (2 * Math.PI);
            }
            //Debug.WriteLine($"{Math_Formulas.radian_to_degree(val)}");
            return val;
        }
    }
}
