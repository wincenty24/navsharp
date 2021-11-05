using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class FakeCoordinates
    {
        private double lat1 = 0.0;
        private double lon1 = 0.0;
        public double angle = 0.0;
        public FakeCoordinates(double lat1, double lon1, double angle)
        {
            set_val(lat1, lon1, angle);
        }
        void set_val(double lat1, double lon1, double angle)
        {
            this.lat1 = lat1;
            this.lon1 = lon1;
            this.angle = angle;
        }
        public double[] calculate_coordinates(double earth_radius_m, double speed)
        {
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(speed) + Math.Cos(lat1) * Math.Sin(speed) * Math.Cos(angle));
            double lon2 = lon1 + Math.Atan2(Math.Sin(angle) * Math.Sin(speed) * Math.Cos(lat1), Math.Cos(speed) - Math.Sin(lat1) * Math.Sin(lat2));
      
            lat1 = lat2;
            lon1 = lon2;
            double[] ret = { lat2, lon2 };
            return ret;
        }
    }
}
