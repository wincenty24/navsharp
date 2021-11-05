using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace navsharp
{
 
    class SerialBoss
    {
        private Thread reader;
        private bool is_available_bool = false;
        private SerialPort serialPort;
        public string port { get; private set; }
        public int baud { get; private set; }

        public volatile string data;

        FakeCoordinates fakeCoordinates;

        public SerialBoss()
        {
            serialPort = new SerialPort();
            //set("COM3", 9600);

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

       }
        private void read()
        {
            
            //sender.Start();
            while (serialPort.IsOpen)
            {
                string a = serialPort.ReadLine();
                if (a != null)
                {
                    is_available_bool = true;
                }
                data = a;
                //Debug.WriteLine(a);

            }
        }
        public void send(string msg)
        {
            serialPort.WriteLine(msg);
            reader = new Thread(read);
            reader.IsBackground = true;
            reader.Start();

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
            return is_available_bool;
        }
        public double[] prepared_data()
        {

            string[] splitted_data = data.Split(' ');
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            double a1 = Convert.ToDouble(splitted_data[0], provider);
            double b1 = Convert.ToDouble(splitted_data[1], provider);
            double angle = Convert.ToDouble(splitted_data[2], provider);
            double speed = Convert.ToDouble(splitted_data[3], provider);

            /*
             * fake coordinates
             * 
             */
            fakeCoordinates.angle = prepare_compass(fakeCoordinates.angle);
            double angle2 = fakeCoordinates.angle += (Math_Formulas.degree_to_radian(angle)/5);
            double[] cd = fakeCoordinates.calculate_coordinates(Values.earth_radius_m + Values.asl, Math_Formulas.calculate_radian_using_radius_and_length(speed, Values.earth_radius_m + Values.asl));
            double[] ret = { cd[0], cd[1], angle2 };
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
