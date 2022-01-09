using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhsyicsScripts : MonoBehaviour
{
    private float a = 1f;
    private float b = 1f;
    private float G = 1f; //0.0000000000667408f this is the irl constant

    public Vector3 applyAllGravity(GameObject object1, ArrayList objectList)
    {
        Vector3 F21 = new Vector3 (0,0,0);

        foreach (GameObject obj2 in objectList)
        {

            if (obj2 != object1)
            {
                F21 += calculateGravity(obj2, object1);

                //pullObject(object1, F21);
            }

        }

        return F21;
    }

    public Vector3 calculateGravity(GameObject pulling, GameObject pulled)
    {
        Vector3 obj1pos = pulling.transform.position;
        Vector3 obj2pos = pulled.transform.position;

        float m1 = pulling.GetComponent<BodyProperties>().mass;
        float m2 = pulled.GetComponent<BodyProperties>().mass;

        Vector3 F21;

        //start gravity calculation
        //FIND r21 g eeez i needed to seprate this to get it working right
        float r = Math.Abs(Vector3.Distance(obj1pos, obj2pos));

        Vector3 subtratedVector = (obj2pos - obj1pos);

        double mag = Math.Sqrt((subtratedVector.x * subtratedVector.x) + (subtratedVector.y * subtratedVector.y) + (subtratedVector.z * subtratedVector.z));


        Vector3 r21 = ((obj2pos - obj1pos) / (float)mag);
        //r21 is found

        //error fixed, it was so t=low due to big g being such a tiny number
        F21 = -G * r21 * ((m1 * m2) / Math.Abs((r * r)));


        //end gravity calculation ;-; geez that was weirdly hard

        //return F21
        return F21;
    }

    public void pullObject(GameObject m2, Vector3 accelration)
    {
        BodyProperties bp = m2.GetComponent<BodyProperties>();
        float mass = bp.mass;


        Vector3 velocity = m2.GetComponent<BodyProperties>().velocity += (accelration / mass) * Time.deltaTime;

        m2.transform.position += velocity * Time.deltaTime;
    }


    void updateSpin(Transform obj_transform, Vector3 spin_vector)
    {
        Quaternion q = Quaternion.Euler(spin_vector.x, spin_vector.y, spin_vector.z);

        obj_transform.rotation = obj_transform.rotation * q;
    }

    public Vector3 findRadius(float mass, float denstity)
    {
        double volume = mass / denstity;

        double volume3 = volume * volume * volume;
        //absolute shit show to calculate radius
        float radius = ((float)((Mathf.PI * 100) * volume3) / 100);

        return new Vector3(radius, radius, radius);
    }

    public Vector3 changeInVelocity(BodyProperties obj_bp)
    {
        //add velocities
        //change position
        //apply the third law
        float mass_ratio = 0;
        Vector3 Impulse = new Vector3(0,0,0);

        
        foreach (GameObject cv in obj_bp.touching_bodies)
        {
            BodyDataStorage obj_2_data = obj_bp.frozen_body_data[obj_bp.frozen_body_data.IndexOf(new BodyDataStorage() { id = cv.GetComponent<BodyProperties>().id })];

            BodyProperties obj2_bp = cv.GetComponent<BodyProperties>();
            if (obj_bp.mass > obj_2_data.mass)
            {
                mass_ratio = obj2_bp.mass / obj_bp.mass;
                Impulse = ((calculateAppliableForce(obj_bp, obj_2_data)));
                Impulse = Impulse - (mass_ratio * ((calculateThirdApplication(obj_bp, obj_2_data))));
            }
            else if (obj_bp.mass < obj_2_data.mass)
            {
                mass_ratio = obj_bp.mass / obj2_bp.mass;
                Impulse = (mass_ratio * (calculateAppliableForce(obj_bp, obj_2_data)));
                Impulse = Impulse - (((calculateThirdApplication(obj_bp, obj_2_data))));
            } 
            else if (obj_bp.mass == obj_2_data.mass) 
            {
                Impulse = ((calculateAppliableForce(obj_bp, obj_2_data)));
                Impulse = Impulse - (((calculateThirdApplication(obj_bp, obj_2_data))));
            }
        }
        //dampening
        //Impulse = .5f * Impulse;

        Impulse = Impulse + applyAllGravity(obj_bp.gameObject, BodyProperties.gravitationalObjectList);
        
        return (Impulse/obj_bp.mass) * Time.deltaTime;
        //apply exernal forces

    }

    Vector3 calculateThirdApplication(BodyProperties object1, BodyDataStorage object2)
    {
        float h1;
        float h2;
        float h3;

        //////////////////////////////////////////////
            
        h1 = addToThirdVector(object1.momentuem.x,  object1.transform.position.x, object2.position.x);
        //////////////////////////////////////////////

        h2 = addToThirdVector(object1.momentuem.y,  object1.transform.position.y, object2.position.y);
        //////////////////////////////////////////////

        h3 = addToThirdVector(object1.momentuem.z,  object1.transform.position.z, object2.position.z);

        return new Vector3(h1, h2, h3);
    }

    float addToThirdVector(float p1, float r1, float r2)
    {
        
        float v = 0;
        if ((p1 > 0 && r1 < r2) || (p1 < 0 && r1 > r2))
        {
            //Debug.Log("third");
            v = p1;
        }
        return v;
    }

    Vector3 calculateAppliableForce(BodyProperties object1, BodyDataStorage object2)
    {
        float h1;
        float h2;
        float h3;

        //////////////////////////////////////////////

        h1 = addToVector(object1.velocity.x, object2.velocity.x, object1.transform.position.x, object2.position.x, object2.momentuem.x);
        //////////////////////////////////////////////
            
        h2 = addToVector(object1.velocity.y, object2.velocity.y, object1.transform.position.y, object2.position.y, object2.momentuem.y);
        //////////////////////////////////////////////

        h3 = addToVector(object1.velocity.z, object2.velocity.z, object1.transform.position.z, object2.position.z, object2.momentuem.z);

        return new Vector3(h1, h2, h3);
    }

    float addToVector(float v1, float v2, float r1, float r2, float p2)
    {

        float v = 0;

        if (Math.Abs(v2) > Math.Abs(v1))
        {
            v = p2;
            //Debug.Log("1");
        }
        else if ((v1 > 0 && v2 < 0 && r1 < r2) || (v1 < 0 && v2 > 0 && r1 > r2))
        {
            v = p2;
            //Debug.Log("2");
        }

        return v;
    }
        
}
    

