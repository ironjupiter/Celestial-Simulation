using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    //important scripts
    public BodyProperties body_data;
    Transform obj_transform;

    //gravitiy stuff
    public bool gravity_affected = true;
    public static List<BodyProperties> gravitational_bodies = new List<BodyProperties>();


    //object data for collisions
    public List<GameObject> touching_bodies = new List<GameObject>();
    public List<BodyDataStorage> frozen_body_data = new List<BodyDataStorage>(); //I think I can optmize this with the gravity list

    // Start is called before the first frame update
    void Start()
    {
        //set bodydata script
        body_data = this.gameObject.GetComponent<BodyProperties>();

        //physics stuff
        obj_transform = this.gameObject.transform;

        //planet redering (not fully devloped)
        obj_transform.localScale = findRadius(body_data.mass, body_data.denstity);
        this.gameObject.GetComponent<Rigidbody>().mass = body_data.mass;

        //gravity stuff
        if(gravity_affected == true)
            gravitational_bodies.Add(body_data);
    }

    //physics update stuff
    void FixedUpdate()
    {
        //update momentuem data
        body_data.momentuem = calculateMomentuem(body_data.velocity, body_data.mass);
        body_data.impulse = new Vector3(0, 0, 0);

        //calculate Impusle
        //gravity
        if(gravity_affected == true)
            body_data.impulse = GravityScript.applyAllGravity(this.gameObject, PlanetScript.gravitational_bodies) / body_data.mass;

        //contact forces
        body_data.impulse += CollisionScript.calculateAppliedForce(this) / body_data.mass;

        //add impulse to velocity
        body_data.velocity += body_data.impulse;

        //add velocity to posistion
        this.gameObject.transform.position += body_data.velocity * Time.deltaTime;


        //update list of bodies data
        frozen_body_data.Clear();
        foreach (GameObject g1 in touching_bodies)
        {
            //for each object in the dynamic list, update frozen list
            BodyProperties g = g1.gameObject.GetComponent<BodyProperties>();
            frozen_body_data.Add(new BodyDataStorage()
            {
                id = g.id,
                mass = g.mass,
                denstity = g.denstity,
                velocity = g.velocity,
                momentuem = g.momentuem,
                position = g.gameObject.transform.position
            });
        }

    }

    public Vector3 calculateMomentuem(Vector3 v, float M) 
    {
        //calculate momentuem
        float f1 = (v.x) * M;
        float f2 = (v.y) * M;
        float f3 = (v.z) * M;

        //return vector
        return new Vector3(f1, f2, f3);
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
        BodyProperties bp = c_obj.gameObject.GetComponent<BodyProperties>();

        //add a dynamic body type
        touching_bodies.Add(c_obj.gameObject);

        //add a frozen body type
        frozen_body_data.Add(new BodyDataStorage()
        {
            id = bp.id,
            mass = bp.mass,
            denstity = bp.denstity,
            velocity = bp.velocity,
            momentuem = bp.momentuem,
            position = bp.gameObject.transform.position
        });
    }

    //when colliding remove objects
    private void OnCollisionExit(Collision c_obj)
    {
        frozen_body_data.Remove(new BodyDataStorage() { id = c_obj.gameObject.GetComponent<BodyProperties>().id });
        touching_bodies.Remove(c_obj.gameObject);
    }
}
