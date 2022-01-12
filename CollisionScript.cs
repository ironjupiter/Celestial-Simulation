using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    
    //note this only works for planets in this version
    public static Vector3 calculateAppliedForce(PlanetScript ps)
    {
        float mass_ratio;
        Vector3 Force = new Vector3(0,0,0);
        BodyProperties A1 = ps.body_data;

        //go through touching bodies script to find anything that needs to be changed
        foreach (GameObject cv in ps.touching_bodies)
        {
            //find the collided w/ object
            BodyDataStorage A2 = ps.frozen_body_data[ps.frozen_body_data.IndexOf(new BodyDataStorage() { id = cv.GetComponent<BodyProperties>().id })];

            //M > m ||| find mass relationship to determine how much of p is turned into F, and related to that how much F is given to A1
            if (A1.mass > A2.mass)
            {
                //if A1 is more than A2 only p * m/M is sent back as third related force, all of A2s p is taken in the collision
                mass_ratio = A2.mass / A1.mass;
                Force = ((calculateAppliableForce(A1, A2)));
                Force = Force - (mass_ratio * ((calculateThirdApplication(A1, A2))));
            }
            else if (A1.mass < A2.mass)
            {
                //if A1 is less than A2 all p is sent back as third related force, only p*m/M taken in the collision
                mass_ratio = A1.mass / A2.mass;
                Force = (mass_ratio * (calculateAppliableForce(A1, A2)));
                Force = Force - (((calculateThirdApplication(A1, A2))));
            } 
            else if (A1.mass == A2.mass) 
            {
                //both are equal so nothing matters in this case
                Force = ((calculateAppliableForce(A1, A2)));
                Force = Force - (((calculateThirdApplication(A1, A2))));
            }
        }

        return Force;

    }

    static Vector3 calculateThirdApplication(BodyProperties object1, BodyDataStorage object2)
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

    static float addToThirdVector(float p1, float r1, float r2)
    {
        
        float v = 0;
        if ((p1 > 0 && r1 < r2) || (p1 < 0 && r1 > r2))
        {
            //Debug.Log("third");
            v = p1;
        }
        return v;
    }

    static Vector3 calculateAppliableForce(BodyProperties object1, BodyDataStorage object2)
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

    static float addToVector(float v1, float v2, float r1, float r2, float p2)
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
    

