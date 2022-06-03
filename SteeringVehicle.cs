using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class SteeringVehicle
    {
        private bool start = false;

        public SteeringVehicle()
        {

        }
        public static double calculate_steering(double target_a, double target_b, double current_a, double current_b, double shift_to_line, double ahead, double earth_radius, double vehicle_degree, double look_ahead_m, double ref_degree)
        {
            //remember to div shift_to_line by earth_radius
            double distance = Math.Abs(Math_Formulas.distance_radian(target_a, current_a, target_b - current_b));
            double L = Math_Formulas.calculate_radian_using_radius_and_length(look_ahead_m, earth_radius);
            double alpha = (ref_degree - vehicle_degree) - Math.Asin(shift_to_line / (distance));
            double k = (2 * shift_to_line) / Math.Pow(distance, 2);
            double r = Math.Pow(distance, 2) / (2 * shift_to_line);
            double theta = Math.Atan2(2 * L * Math.Sin(alpha), distance);
            return theta;
        }
        public static double steering()
        {
            return 0;
        }
        public void Start()
        {
            start = true;
        }
        public void Stop()
        {
            start = false;
        }
        public bool is_available()
        {
            return start;
        }
    }
}
