using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace navsharp
{
    class Controller
    {
        private Thread reader;
        private bool is_available_bool = false;
        public string port { get; private set; }
        public int baud { get; private set; }

        private SerialPort serialPort;
        private bool is_port_availabale = false;
        public Controller()
        {

            //set("COM4", 9600);

        }
        public void send(string val)
        {
            serialPort.WriteLine(val);//napisz funkcję, która się zastopuje jak zabraknie połączenia
        }
        public void set(string port, int baud)
        {
            serialPort = new SerialPort();
            serialPort.PortName = port;
            serialPort.BaudRate = baud;
            this.baud = baud;
            this.port = port;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            start();
            is_port_availabale = true;
        }
        public void start()
        {
            serialPort.DtrEnable = false;
            serialPort.DtrEnable = true;
            serialPort.Open();
        }
        public bool is_port_open()
        {
            return is_port_availabale;
        }
    }
}
