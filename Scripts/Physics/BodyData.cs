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
using System.Net.Mime;
using Random = UnityEngine.Random;


public class BodyData : MonoBehaviour
{
    public static Color [] colors = {
        new Color(157/225, 180/225, 255/225),
        new Color(228/225, 232/225, 255/225),
        new Color(255/225, 241/225, 223/225),
        new Color(255/225, 187/225, 123/225) };
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
    public Color star_color = Color.black;

    public GameObject cc;

    // Start is called before the first frame update
    void Awake()
    {
        if (id == -1)
        {
            id = id_setup;
            id_setup++;
        }

        if (radius == 0)
            radius = mass;
        
        if (id == 0)
        {
            Debug.Log(Application.dataPath);
            try
            {
                string filenameNoExt = "FinalNameList"; //.txt is removed
                TextAsset f = (TextAsset)Resources.Load(filenameNoExt);
                
                Debug.Log("break" + f.text);
                string[] temp = f.text.Split("\n");

                foreach (string st in temp)
                {
                    all_names.Add(st);
                }
            }
            catch
            {
                Debug.Log("file not readable");
            }
        }

        int name_index = (int)Mathf.Floor(Random.value*all_names.Count);
        name = all_names[name_index];
        Debug.Log(name);

    }

    private void FixedUpdate()
    {
        position_read = transform.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string jsonString = JsonUtility.ToJson(this);
            Debug.Log("json string " + jsonString);
        }
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
        SimulationUIManager.setCameraParent(cc, this.transform);
    }
}
