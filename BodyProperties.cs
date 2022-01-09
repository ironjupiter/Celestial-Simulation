using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodyProperties : MonoBehaviour
{
    public int id = 0;
    private static int id_setup = 0;

    public float mass;
    public float denstity;
    private float radius;

    public long rotation_period;
    public Vector3 velocity;
    public Vector3 momentuem;
    public Vector3 spin_vector;

    Transform obj_transform;

    public List<GameObject> touching_bodies = new List<GameObject>();
    public List<BodyDataStorage> frozen_body_data = new List<BodyDataStorage>();

    public static ArrayList gravitationalObjectList = new ArrayList(); //list of all gravitational objects within simulation

    PhsyicsScripts phsyics;

    private Vector3 Impulse;


    // Start is called before the first frame update
    void Start()
    {
        id = id_setup;
        id_setup++;

        if (denstity == 0)
            denstity = mass;
        phsyics = GameObject.Find("physics").GetComponent<PhsyicsScripts>();
        obj_transform = this.gameObject.transform;
        obj_transform.localScale = phsyics.findRadius(mass, denstity);
        this.gameObject.GetComponent<Rigidbody>().mass = mass;
        gravitationalObjectList.Add(this.gameObject);

        Debug.Log(this.id + "P before = " + (this.mass * this.velocity));
    }

    void FixedUpdate()
    {
        //recalculate momentuem
        float f1 = (velocity.x / Time.deltaTime) * mass;
        float f2 = (velocity.y / Time.deltaTime) * mass;
        float f3 = (velocity.z / Time.deltaTime) * mass;
        momentuem = new Vector3(f1, f2, f3);

        //change position for time
        //test

        Impulse = phsyics.changeInVelocity(this);

        velocity += Impulse;

        this.gameObject.transform.position += velocity * Time.deltaTime;

        frozen_body_data.Clear();
        foreach (GameObject g1 in touching_bodies) 
        {
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

    private void OnCollisionEnter(Collision c_obj)
    {
        BodyProperties bp = c_obj.gameObject.GetComponent<BodyProperties>();
        touching_bodies.Add(c_obj.gameObject);
        frozen_body_data.Add(new BodyDataStorage() { id = bp.id, mass = bp.mass,
            denstity = bp.denstity,
            velocity = bp.velocity,
            momentuem = bp.momentuem,
            position = bp.gameObject.transform.position});
        Debug.Log(momentuem);
    } 

    private void OnCollisionExit(Collision c_obj)
    {
        frozen_body_data.Remove(new BodyDataStorage() { id = c_obj.gameObject.GetComponent<BodyProperties>().id });
        touching_bodies.Remove(c_obj.gameObject);
    
    }
}
