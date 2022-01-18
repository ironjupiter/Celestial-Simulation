using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSynchronizer : MonoBehaviour
{
    private static List<GameObject> celestial_bodies = new List<GameObject>();


    private void FixedUpdate()
    {
        updateMomentuems();
        gravityCalculations();
        contactForceCalculations();
        applyCalculations();
    }

    void applyCalculations()
    {
        foreach (GameObject body in celestial_bodies) 
        {
            BodyData properties = body.GetComponent<BodyData>();
            body.transform.position += properties.velocity;
        }
        updateMomentuems();

    }

    void gravityCalculations() {

        List<BodyData> g_list = PlanetScript.gravitational_bodies;
        foreach (BodyData body in g_list) 
        {
            body.impulse = GravityScript.applyAllGravity(body.gameObject, g_list);
            
        }

        
        foreach (BodyData body in g_list)
        {
            body.velocity += (body.impulse / body.mass) * Time.deltaTime;
            body.impulse = new Vector3(0, 0, 0);
        }

        updateMomentuems();

    }

    void contactForceCalculations() 
    { 
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();
            body_data.impulse = new Vector3(0, 0, 0);
            body_data.impulse = CollisionScript.calculateAppliedForce(body.GetComponent<PlanetScript>());
        }
        
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();
            body_data.velocity += (body_data.impulse / body_data.mass) * Time.deltaTime;
            body_data.impulse = new Vector3(0, 0, 0);
        }
        updateMomentuems();

    }

    void updateMomentuems() 
    {
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();

            body_data.momentuem = (body_data.velocity * body_data.mass) / Time.deltaTime;
        }
    }

    public static void addCelestialBody(GameObject body)
    {
        celestial_bodies.Add(body);
    }

}
