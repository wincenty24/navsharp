using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace navsharp
{
    class Saver_reader
    {
        private static string openfiledialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    return filePath;
                }
                else
                {
                    return "";
                }
            }
        }
        private static string savefiledialog()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    return filePath;
                }
                else
                {
                    return "";
                }
            }
        }
        public static void read(ref List<LocationCollection> list_of_loc_coll, ref double[] main_points, ref int num_of_loc_coll)
        {
            try
            {
                string file_name = openfiledialog();
                if(file_name != null || file_name != "") 
                {
                    string[] all_lines = File.ReadAllLines(file_name);
                    foreach (string line in all_lines)
                    {
                        string[] check_type = line.Split(' ');                   
                        if (check_type[0] == "points")
                        {                        
                            main_points[0] = Convert.ToDouble(check_type[1].Replace(".", ","));
                            main_points[1] = Convert.ToDouble(check_type[2].Replace(".", ","));
                            main_points[2] = Convert.ToDouble(check_type[3].Replace(".", ","));
                            main_points[3] = Convert.ToDouble(check_type[4].Replace(".", ","));                 
                            }
                        else if (check_type[0] == "start")
                        {
                            list_of_loc_coll.Add(new LocationCollection());
                            num_of_loc_coll++;
                        }
                        else if (check_type[0] == "loc")
                        {
                            double lon = Convert.ToDouble(check_type[1].Replace(".", ","));
                            double lat = Convert.ToDouble(check_type[2].Replace(".", ","));
                            list_of_loc_coll[num_of_loc_coll - 1].Add(new Location(lon, lat));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show($"Saver_reader-read:{e.ToString()}");
            }
        }
        public static void save(List<LocationCollection> list_of_loc_coll, double[] main_points)
        {
            try
            {
                string file_name = savefiledialog();
                using (StreamWriter text = new StreamWriter(file_name))
                {
                    text.WriteLine($"points {prepare_double_to_string(main_points)}");
                    foreach (LocationCollection loc_coll in list_of_loc_coll)
                    {
                        text.WriteLine($"start");
                        foreach (Location loc in loc_coll)
                        {
                            text.WriteLine($"loc {prepare_location_to_string(loc)}");
                        }
                        text.WriteLine($"end");
                    }
                    text.WriteLine($"finish");
                }
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show($"Saver_reader-read:{e.ToString()}");
            }
        }
        private static string prepare_location_to_string(Location loc)
        {
            string[] splited_loc = loc.ToString().Replace(",", ".").Split('.');
            return $"{splited_loc[0]}.{splited_loc[1]} {splited_loc[2]}.{splited_loc[3]}";
        }
        private static string prepare_double_to_string(double[] points)
        {
            string x = "";
            foreach (double point in points)
            {
                x += $"{point.ToString()} ";
            }
            string y = x.Replace(",", ".");
            return y;
        }
    }
}
