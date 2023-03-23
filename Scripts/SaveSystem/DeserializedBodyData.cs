using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeserializedBodyData
{
    public static List<string> all_names = new List<string>();
    private static int id_setup = 0;
    
    public int id = -1;
    public string name;
    public string bodytype = "";
    
    public float mass;
    public float radius;
    
    public Vector3 position_read;
    public Vector3 velocity = new Vector3 (0, 0, 0);
    public Vector3 momentuem;
    public Vector3 impulse = new Vector3(0, 0, 0);

    public GameObject cc;
    public Color star_color = Color.black;
}
