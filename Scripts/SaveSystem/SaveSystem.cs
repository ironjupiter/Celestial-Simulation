using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;

public class SaveSystem
{
    private static string saveFilePath = Application.dataPath + @"\SaveFiles";
    private static string fileAppendage = ".sdata";

    public static void serializeSimulation(List<GameObject> bodies, string file_name)
    {
        List<string> json_objects = new List<string>();

        foreach (GameObject g in bodies)
        {
            BodyData data = g.GetComponent<BodyData>();
            data.position_read = g.transform.position;
            json_objects.Add(JsonUtility.ToJson(data));
            //Debug.Log("json string " + jsonString);
        }
    
        DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        string path = Application.dataPath + @"\SaveFiles";
        
        if (!Directory.Exists(path))
            Directory. CreateDirectory(path);
        
        //temp naming system
        
        path = path + @"\" + file_name + fileAppendage;
        //FileStream fs = File.Create(path);
        using StreamWriter file = new(path);
        for (int i = 0; i < json_objects.Count; i++)
        {
            file.WriteLine(json_objects[i]);
        }
        file.Close();
        //Debug.Log("save done");


    }

    public static string getSaveDirectory()
    {
        return saveFilePath;
    }

    public static List<DeserializedBodyData> deserializeData(string path)
    {
        using StreamReader file = new(path);
        string file_string = file.ReadToEnd();
        //Debug.Log("file contents: " + file_string);
        string[] json_obj_list = file_string.Split("\r\n");

        string [] file_name = path.Split(".");
        if (file_name[1] ==  "." + fileAppendage || file_name.Length > 2)
        {
            return null;
        }

        List<DeserializedBodyData> dsbd = new List<DeserializedBodyData>();
        for (int i = 0; i < json_obj_list.Length; i++)
        {
            if (json_obj_list[i] != "")
            {
                DeserializedBodyData new_body = JsonUtility.FromJson<DeserializedBodyData>(json_obj_list[i]);
                dsbd.Add(new_body);
                //Debug.Log("object: " + new_body.name);
            }
        }
        Debug.Log("objects num: " + dsbd.Count);
        file.Close();
        return dsbd;
    }
}
