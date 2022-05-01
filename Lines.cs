using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace navsharp
{
    class Lines
    {

        public double max_long;
        public double max_lat;
        public double min_long;
        public double min_lat;
        public MapPolyline polyline;
        private Map map;
        public string color;
        public Lines(double a1, double b1, double a2, double b2, Map map, string color)
        {
            min_long = a1;
            min_lat = b1;
            max_long = a2;
            max_lat = b2;
            this.map = map;
            this.color = color;
            polyline = new MapPolyline();
        }
        public void draw_line()
        {
            polyline.Opacity = 0.7;
            polyline.Locations = new LocationCollection() {
            new Location(Math_Formulas.radian_to_degree(min_long), Math_Formulas.radian_to_degree(min_lat)),
            new Location(Math_Formulas.radian_to_degree(max_long), Math_Formulas.radian_to_degree(max_lat))};
            var col = (Color)ColorConverter.ConvertFromString($"{color}");
            polyline.Stroke = new SolidColorBrush(col);
            map.Children.Add(polyline);
            
        }
        public static void add_mappolyline(Map mapa, int num_pol, ref List<LocationCollection> list_of_loc_coll, ref List<MapPolyline> list_MapPolyline)
        {
            list_of_loc_coll.Add(new LocationCollection()); 
            list_MapPolyline.Add(new MapPolyline());
            Debug.WriteLine($"list_MapPolyline: {list_MapPolyline.Count} num_pol:{num_pol}");
            list_MapPolyline[num_pol].Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            list_MapPolyline[num_pol].StrokeThickness = 5;
            list_MapPolyline[num_pol].Opacity = 1;
            mapa.Children.Add(list_MapPolyline[num_pol]);
        }
        public static void add_mappolyline(Map mapa, int num_pol, ref List<MapPolyline> list_MapPolyline)
        {
            list_MapPolyline.Add(new MapPolyline());
            list_MapPolyline[num_pol].Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            list_MapPolyline[num_pol].StrokeThickness = 5;
            list_MapPolyline[num_pol].Opacity = 1;
            mapa.Children.Add(list_MapPolyline[num_pol]);
        }
    }
}
