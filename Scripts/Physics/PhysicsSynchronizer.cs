using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class PhysicsSynchronizer : MonoBehaviour
{
    private static List<GameObject> celestial_bodies = new List<GameObject>();

    private static int collisionAlgorithm = 0;
    private static bool gravityOn = true;

    void Start()
    {
        if(celestial_bodies.Count > 0)
            CicrularVelocityTool.findMostMassive(celestial_bodies);
        
        foreach (GameObject g in celestial_bodies)
        {
            g.GetComponent<CicrularVelocityTool>().setInitializationVelocity();
        }
    }


    private void FixedUpdate()
    {
        updateMomentuems();
        
        if(gravityOn)
            gravityCalculations();

        switch (collisionAlgorithm)
        {
            case 0:
                mergerCalculations();
                break;
            case 1:
                contactForceCalculations();
                break;
                
            default:
                Debug.LogError("NO VALID TYPE GIVEN FOR COLLISION: " + collisionAlgorithm);
                break;
        }
        //mergerCalculations();//merging
        applyCalculations();

        updateMomentuems();
        float a = 0;
        foreach (GameObject body in celestial_bodies) 
        {
            a += body.GetComponent<BodyData>().momentuem.magnitude;
            //Debug.Log(body.GetComponent<BodyData>().id);
        }
        //Debug.Log(a);
    }

    public static List<GameObject> getBodyList()
    {
        return celestial_bodies;
    }


    void mergerCalculations() 
    {
        CollisionScript.nonElasticCollision(celestial_bodies);
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
            body.velocity += (body.impulse / body.mass) * Time.fixedDeltaTime ;
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
            body_data.velocity += (body_data.impulse / body_data.mass) * Time.fixedDeltaTime ;
            body_data.impulse = new Vector3(0, 0, 0);
        }
        updateMomentuems();

    }

    void updateMomentuems() 
    {
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();

            body_data.momentuem = (body_data.velocity * body_data.mass) / Time.fixedDeltaTime ;
        }
    }

    public static void addCelestialBody(GameObject body)
    {
        celestial_bodies.Add(body);
        Debug.Log(celestial_bodies.Contains(body));
        CicrularVelocityTool.updateList(celestial_bodies);
    }

    public static void removeCelestialBody(GameObject body)
    {
        celestial_bodies.Remove(body);        
    }


    public void setCollisionType()
    {
        collisionAlgorithm = this.transform.GetChild(0).GetChild(0).GetChild(7).GetChild(1).GetComponent<TMP_Dropdown>().value;
        Debug.Log(collisionAlgorithm);
    }

    public void setElasticity()
    {
        float e = this.transform.GetChild(0).GetChild(0).GetChild(8).GetChild(0).GetComponent<Slider>().value;
        this.transform.GetChild(0).GetChild(0).GetChild(8).GetChild(2).GetComponent<TMP_Text>().text = Math.Round(e*100)/100 + "/1";
        CollisionScript.setElasticity((float)Math.Round(e*100)/100);
    }

    public void setGravity()
    {
        gravityOn = !gravityOn;
    }
}
