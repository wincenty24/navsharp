using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace navsharp
{
    class Lines
    {

        public double max_long;
        public double max_lat;
        public double min_long;
        public double min_lat;
        MapPolyline polyline = new MapPolyline();
        Map map;
        public Lines(double a1, double b1, double a2, double b2, Map map)
        {
            min_long = a1;
            min_lat = b1;
            max_long = a2;
            max_lat = b2;
            this.map = map;
        }
        public void draw_line()
        {
            polyline.Opacity = 0.7;
            polyline.Locations = new LocationCollection() {
            new Location(Math_Formulas.radian_to_degree(min_long), Math_Formulas.radian_to_degree(min_lat)),
            new Location(Math_Formulas.radian_to_degree(max_long), Math_Formulas.radian_to_degree(max_lat))};

            polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            map.Children.Add(polyline);
            
        }
    }
}
