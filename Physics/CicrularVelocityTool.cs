using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CicrularVelocityTool : MonoBehaviour
{
    private static List<GameObject> celestial_bodies;
    private static GameObject mostMassive; 

    public static void setBodyList(List<GameObject> list)
    {
        celestial_bodies = list;
        mostMassive = celestial_bodies[0];
        foreach (GameObject g in celestial_bodies)
        {
            if (g.GetComponent<BodyData>().mass > mostMassive.GetComponent<BodyData>().mass)
            {
                mostMassive = g;
            }
        }
    }

    public void setVelocity()
    {
        if (mostMassive == this.gameObject)
        {
            return;
        }

        float distance = Vector3.Distance(this.gameObject.transform.position, mostMassive.transform.position);
        float gravitationalForce = mostMassive.GetComponent<BodyData>().mass / (float)(distance * distance);
        
        double neededVelocity = Mathf.Sqrt((mostMassive.GetComponent<BodyData>().mass /(distance)))/6.7f;//6.7 seems to be what is needed to fine tune???
        //double neededVelocity = Mathf.Sqrt(gravitationalForce*distance);
        float x_dis = this.gameObject.transform.position.x - mostMassive.transform.position.x;
        float z_dis = this.gameObject.transform.position.z - mostMassive.transform.position.z;
        
        Vector3 velocity;
        if (x_dis != 0 || z_dis != 0)
        {
            Vector3 fullVec = new Vector3(x_dis*-1,0,z_dis*-1);
            float mag = (float)distance;
            Vector3 unit = fullVec / mag;
            Vector3 inverse_unit = new Vector3(unit.z,0,unit.x*-1);//points at the largest obj

            velocity = inverse_unit * (float)neededVelocity;
        }
        else if (x_dis == 0)
        {
            velocity = new Vector3(0, 0, (float)neededVelocity);
        }
        else
        {
            velocity = new Vector3((float)neededVelocity, 0, 0);
        }
        this.gameObject.GetComponent<BodyData>().velocity = velocity;
        Debug.Log(neededVelocity);
    }
}
