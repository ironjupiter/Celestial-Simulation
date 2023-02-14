using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;

public class DataSheetCompiler
{
    
        public static void compileNames()
        {
            string path = @"D:\Ben\Celestial Simulation\Assets\Datasets";

            List<string> paths = new List<string>();
            DataSheetCompiler dsc = new DataSheetCompiler();
            paths = dsc.pathAdder(path);
            
            
            
            //List<string> lines = new List<string>();
            Dictionary<string, string> lines = new Dictionary<string, string>();

            foreach (string file_name in paths)
            {
                if (File.Exists(file_name) && file_name ==  (path + @"\exoplanetlist.txt"))
                {
                    Debug.Log("exo planets");
                    string[] temp_storage = File.ReadAllLines(file_name);
                    for (int i = 0; i < temp_storage.Length; i++)
                    {
                        string[] temp_split = temp_storage[i].Split(',');
                        //Debug.Log(temp_split[1]);
                        string temp = temp_split[1];

                        try
                        {
                            lines.Add(temp, temp);
                        }
                        catch
                        {
                            Debug.Log("Duplicate found: " + temp);
                        }
                    }
                }
                else if (File.Exists(file_name) && (file_name == path + @"\minor-planet-names-alphabetical-list.txt"))
                {
                    Debug.Log("minor planets");
                    string[] temp_storage = File.ReadAllLines(file_name);
                    for (int i = 0; i < temp_storage.Length; i++)
                    {
                        string[] temp_split = temp_storage[i].Split(';');
                        //Debug.Log(temp_split[1]);
                        string temp = temp_split[1];
                        try
                        {
                            lines.Add(temp, temp);
                        }
                        catch
                        {
                            Debug.Log("Duplicate found: " + temp);
                        }
                    }
                }
                else if(File.Exists(file_name))
                {
                    Debug.Log("normal planets");
                    // Store each line in array of strings
                    string[] temp_storage = File.ReadAllLines(file_name);
                    for (int i = 0; i < temp_storage.Length; i++)
                    {
                        try
                        {
                            lines.Add(temp_storage[i], temp_storage[i]);
                        }
                        catch
                        {
                            Debug.Log("Duplicate found: " + temp_storage[i]);
                        }
                    }
                }
            }

            string file_to_write = @"D:\Ben\Celestial Simulation\Assets\Datasets\FinalNameList.ttx";
            File.WriteAllText(file_to_write, " ");
            
           using(StreamWriter writer = new StreamWriter(file_to_write))
            {

                foreach(KeyValuePair <string, string> ln in lines)
                {
                    writer.WriteLine(ln.Value);
                }
            }
            Debug.Log("done");
        }

        public List<string> pathAdder(string main_path)
    {
        List<string> new_paths = new List<string>();
        new_paths.Add( main_path+ @"\exoplanetlist.txt");
        new_paths.Add( main_path+ @"\GreekGods.txt");
        new_paths.Add( main_path+ @"\minor-planet-names-alphabetical-list.txt");
        new_paths.Add( main_path+ @"\roman_gods.txt");
        new_paths.Add( main_path+ @"\Starnames.txt");
        new_paths.Add( main_path+ @"\romangodnames.txt");
        new_paths.Add( main_path+ @"\NorseGods.txt");
        new_paths.Add( main_path+ @"\KerbalReferences.txt");
        new_paths.Add( main_path+ @"\RealPlanets.txt");
        return new_paths;
    }
}
