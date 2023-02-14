using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CicrularVelocityTool : MonoBehaviour
{
    private static List<GameObject> celestial_bodies;
    private static GameObject mostMassive; 

    public static void findMostMassive(List<GameObject> list)
    {
        if (celestial_bodies.Count == 0)
        {
            return;
        }

        mostMassive = celestial_bodies[0];
        foreach (GameObject g in celestial_bodies)
        {
            if (g.GetComponent<BodyData>().mass > mostMassive.GetComponent<BodyData>().mass)
            {
                mostMassive = g;
            }
        }
    }

    public void applyMostForceful(List<GameObject> list, GameObject forcedBody)
    {
        celestial_bodies = list;

        float highest_force = 0;
        int highest_force_index = -1;
        
        
        List<float> force_list = new List<float>();
        List<int> force_index = new List<int>();

        foreach (GameObject g in celestial_bodies)
        {
            if (forcedBody != g)
            {
                float force = GravityScript.calculateGravity(g, forcedBody).magnitude;
                int index = celestial_bodies.IndexOf(g);
                force_list.Add(force);
                force_index.Add(index);
                
            }
        }

        for (int i = 0; i < force_list.Count; i++)
        {
            if (force_list[i] > highest_force)
            {
                highest_force = force_list[i];
                highest_force_index = force_index[i];
            }
        }
        
        forcedBody.GetComponent<CicrularVelocityTool>().setInitializationVelocity(list[highest_force_index]);
        
    }

    public static void updateList(List<GameObject> bodies)
    {
        celestial_bodies = bodies;
        findMostMassive(celestial_bodies);
    }

    public void setInitializationVelocity()
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
        this.gameObject.GetComponent<BodyData>().velocity = velocity + mostMassive.GetComponent<BodyData>().velocity;
        Debug.Log(neededVelocity);
    }
    
    public void setInitializationVelocity(GameObject mostForceful)
    {
        if (mostForceful == this.gameObject)
        {
            return;
        }

        float distance = Vector3.Distance(this.gameObject.transform.position, mostForceful.transform.position);
        float gravitationalForce = mostForceful.GetComponent<BodyData>().mass / (float)(distance * distance);
        
        double neededVelocity = Mathf.Sqrt((mostForceful.GetComponent<BodyData>().mass /(distance)))/6.7f;//6.7 seems to be what is needed to fine tune???
        //double neededVelocity = Mathf.Sqrt(gravitationalForce*distance);
        float x_dis = this.gameObject.transform.position.x - mostForceful.transform.position.x;
        float z_dis = this.gameObject.transform.position.z - mostForceful.transform.position.z;
        
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
