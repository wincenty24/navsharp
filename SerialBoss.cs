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
        public SerialBoss(string port, int baud)
        {
            this.baud = baud;
            this.port = port;
            serialPort = new SerialPort();
            serialPort.PortName = port;
            serialPort.BaudRate = 9600;
            set();
        }
        public SerialBoss()
        {
            serialPort = new SerialPort();
            serialPort.PortName = "COM3";
            serialPort.BaudRate = 9600;

            set();
        }
        private void set()
        {
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
            fakeCoordinates.angle = fakeCoordinates.angle % 360;
            double angle2 = fakeCoordinates.angle += Math_Formulas.degree_to_radian(angle);
            double[] cd = fakeCoordinates.calculate_coordinates(Values.earth_radius_m + Values.asl, speed);
            double[] ret = { cd[0], cd[1], angle2 };
            return ret;
        }
    }
}
