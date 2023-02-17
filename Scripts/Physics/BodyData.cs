using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using Random = UnityEngine.Random;


public class BodyData : MonoBehaviour
{
    public int id = 0;
    public string bodytype = "";
    private static int id_setup = 0;

    public float mass;
    public float radius;

    public long rotation_period;
    public Vector3 velocity = new Vector3 (0, 0, 0);
    public Vector3 momentuem;
    public Vector3 spin_vector;

    public Vector3 impulse = new Vector3(0, 0, 0);

    public GameObject cc;

    
    public static List<string> all_names = new List<string>();
    public string name;
    // Start is called before the first frame update
    void Awake()
    {
        id = id_setup;
        id_setup++;

        if (radius == 0)
            radius = mass;
        
        if (id == 0)
        {
            string directory = Path.GetDirectoryName(Application.dataPath);
            Debug.Log(directory);
            string [] temp = File.ReadAllLines(@"Assets\Scripts\Physics\FinalNameList.txt");
            foreach (string st in temp) {
                all_names.Add(st);
            }
        }

        int name_index = (int)Mathf.Floor(Random.value*all_names.Count);
        name = all_names[name_index];
        Debug.Log(name);
        
    }

    public void changeRadi(float r) 
    {
        radius = r;
        this.GetComponent<PlanetScript>().findRadius(r);
        this.GetComponent<TrailRenderer>().widthMultiplier = r / 4;
    }

    public void OnMouseDown()
    {
        Debug.Log("pew");
        PlanetInformationUI.setCameraParent(cc, this.transform);
    }
}
