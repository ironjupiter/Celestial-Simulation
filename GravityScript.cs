using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GravityScript : MonoBehaviour
{
    //the gravitatinal constant, put at 1 for debug reasons
    static float G = 1f;//0.0000000000667408f this is the irl constant

    public static Vector3 applyAllGravity(GameObject object1, List<BodyData> objectList)
    {

        //set up the force vector
        Vector3 F21 = new Vector3(0, 0, 0);

        foreach (BodyData obj2 in objectList)
        {
            //make sure there is no repeat
            if (obj2.gameObject != object1)
            {
                //add new gravity vector to f21
                F21 += calculateGravity(obj2.gameObject, object1);
            }

        }
        //return force vector
        return F21;
    }

    public static Vector3 calculateGravity(GameObject pulling, GameObject pulled)
    {
        Vector3 r1 = pulling.transform.position;
        Vector3 r2 = pulled.transform.position;

        float m1 = pulling.GetComponent<BodyData>().mass;
        float m2 = pulled.GetComponent<BodyData>().mass;

        Vector3 F21;

        //start gravity calculation
        //FIND r21 g eeez i needed to seprate this to get it working right
        float r = Math.Abs(Vector3.Distance(r1, r2));

        Vector3 subtratedVector = (r2 - r1);

        double mag = Math.Sqrt((subtratedVector.x * subtratedVector.x) + (subtratedVector.y * subtratedVector.y) + (subtratedVector.z * subtratedVector.z));


        Vector3 r21 = ((r2 - r1) / (float)mag);
        //r21 is found

        //do final calulations
        F21 = -G * r21 * ((m1 * m2) / Math.Abs((r * r)));

        //return F21
        return F21;
    }
}
