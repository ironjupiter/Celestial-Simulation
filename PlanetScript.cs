using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetScript : MonoBehaviour
{
    //important scripts
    public BodyData body_data;
    Transform obj_transform;

    //gravitiy stuff
    public bool gravity_affected = true;
    public static List<BodyData> gravitational_bodies = new List<BodyData>();


    //object data for collisions
    public List<GameObject> touching_bodies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //set bodydata script
        body_data = this.gameObject.GetComponent<BodyData>();

        //physics stuff
        obj_transform = this.gameObject.transform;

        //planet redering (not fully devloped)
        obj_transform.localScale = findRadius(body_data.mass, body_data.denstity);
        this.gameObject.GetComponent<Rigidbody>().mass = body_data.mass;

        //gravity stuff
        if(gravity_affected == true)
            gravitational_bodies.Add(body_data);

        PhysicsSynchronizer.addCelestialBody(this.gameObject) ;
    }

    //find radius of a sphere given the mass and density
    public Vector3 findRadius(float m, float D)
    {
        //find volume
        double volume = m / D;

        //cube volume
        double volume3 = volume * volume * volume;

        //absolute shit show to calculate radius
        float radius = ((float)((Mathf.PI * 100) * volume3) / 100);

        //return
        return new Vector3(radius, radius, radius);
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
}
