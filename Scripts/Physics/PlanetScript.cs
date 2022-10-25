using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetScript : MonoBehaviour
{
    //important scripts
    public GameObject prefab_copy;
    public BodyData body_data;
    Transform obj_transform;

    //gravitiy stuff
    private bool gravity_affected = true;
    public bool to_destroy = false;
    public static List<BodyData> gravitational_bodies = new List<BodyData>();


    //object data for collisions
    public List<GameObject> touching_bodies = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("NEW OBJECT CREATED");

        //set bodydata script
        body_data = this.gameObject.GetComponent<BodyData>();

        //physics stuff
        obj_transform = this.gameObject.transform;

        //planet redering (not fully devloped)
        findRadius(body_data.radius);
        this.gameObject.GetComponent<Rigidbody>().mass = body_data.mass;

        //gravity stuff
        if(gravity_affected == true)
            gravitational_bodies.Add(body_data);

        PhysicsSynchronizer.addCelestialBody(this.gameObject);
        
    }

    private void FixedUpdate()
    {
        //List<int> index_to_remove = new List<int>();
        for (int i = 0; i < touching_bodies.Count; i++) 
        {
            if (touching_bodies[i] == null) 
            {
                touching_bodies.Remove(touching_bodies[i]);
                i--;
            }
        }
    }

    //find radius of a sphere given the mass and density
    public void findRadius(float radius)
    {
        obj_transform.localScale = new Vector3(radius, radius, radius);
    }

    //when coliding add object to the two lists
    private void OnCollisionEnter(Collision c_obj)
    {
        //make a shortcut
        BodyData bp = c_obj.gameObject.GetComponent<BodyData>();

        //add a dynamic body type
        touching_bodies.Add(c_obj.gameObject);

    }

    //when colliding remove objects
    private void OnCollisionExit(Collision c_obj)
    {
        touching_bodies.Remove(c_obj.gameObject);
    }

    public static void editPlanet(GameObject edited_body, float total_system_mass, Vector3 momentuem, Vector3 position) 
    {
        edited_body.transform.position = position;
        BodyData data = edited_body.GetComponent<BodyData>();

        float density = data.mass / ((float)Math.Pow(data.radius, 3.0f));
        data.mass += total_system_mass - data.mass;
        data.radius = data.mass / (density);
        edited_body.GetComponent<BodyData>().changeRadi((float)Math.Pow(data.radius, 1.0f / 3.0f));

        data.momentuem = momentuem;
        data.velocity = (momentuem / total_system_mass) * Time.deltaTime;
    }

    public static void destroyPlanet(GameObject old_body) 
    {
        PhysicsSynchronizer.removeCelestialBody(old_body);
        gravitational_bodies.Remove(old_body.GetComponent<BodyData>());
        Destroy(old_body);
    }
}
