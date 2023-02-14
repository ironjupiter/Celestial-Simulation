using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBodyOctree : MonoBehaviour
{
    //Set up stuff
    List<Vector3> point_definitions = new List<Vector3>();
    List<Vector3> surface_centers = new List<Vector3>();

    List<GameObject> NBodies = new List<GameObject>();
    List<GameObject> subdivision = new List<GameObject>();


    int nbody_cap = 1;
    bool is_top = true;
    bool sub_divide = false;
    Vector3 position;
    Vector3 new_position;

    //This is size from 0 to one egde, so to get total length you would multiply by two
    public float size = 1f;

    public void Start()
    {
        position = this.gameObject.transform.position;
        new_position = this.gameObject.transform.position;
        createCube();
        surfaceCenterCreation();
        drawCube();

    }

    public void FixedUpdate()
    {
        new_position = this.gameObject.transform.position;
        //Debug.Log(surface_centers.Count);
        sizeAdjustment();
        drawCube();
        position = this.gameObject.transform.position;
    }

    void sizeAdjustment()
    {

        if ((size > Math.Abs(point_definitions[0].x - position.x) || size < Math.Abs(point_definitions[0].x - position.x) || position != new_position) && size > 0)
        {
            for (int i = 0; i < point_definitions.Count; i++)
            {
                float x_sign = (point_definitions[i].x - new_position.x) / Math.Abs(point_definitions[i].x - new_position.x);
                float y_sign = (point_definitions[i].y - new_position.y) / Math.Abs(point_definitions[i].y - new_position.y);
                float z_sign = (point_definitions[i].z - new_position.z) / Math.Abs(point_definitions[i].z - new_position.z);

                Vector3 new_v = new Vector3(
                    ((((size * x_sign)) + (point_definitions[i].x - new_position.x)) - (point_definitions[i].x - new_position.x)) + new_position.x,
                    ((((size * y_sign)) + (point_definitions[i].y - new_position.y)) - (point_definitions[i].y - new_position.y)) + new_position.y,
                    ((((size * z_sign)) + (point_definitions[i].z - new_position.z)) - (point_definitions[i].z - new_position.z)) + new_position.z);

                point_definitions[i] = new_v;
            }
            
            for (int i = 0; i < surface_centers.Count; i++)
            {
                float x_sign = 0;
                float y_sign = 0;
                float z_sign = 0;

                if (surface_centers[i].x - position.x != 0)
                {
                    x_sign = (surface_centers[i].x - new_position.x) / Math.Abs(surface_centers[i].x - new_position.x);
                }
                if (surface_centers[i].y - position.y != 0)
                {
                    y_sign = (surface_centers[i].y - new_position.y) / Math.Abs(surface_centers[i].y - new_position.y);
                }
                if (surface_centers[i].z - position.z != 0)
                {
                    z_sign = (surface_centers[i].z - new_position.z) / Math.Abs(surface_centers[i].z - new_position.z);
                }

                Vector3 new_v = new Vector3(((((size * x_sign) + (surface_centers[i].x - new_position.x)) - (surface_centers[i].x - new_position.x)) * Math.Abs(x_sign)) + new_position.x, 
                                            ((((size * y_sign) + (surface_centers[i].y - new_position.y)) - (surface_centers[i].y - new_position.y)) * Math.Abs(y_sign)) + new_position.y, 
                                            ((((size * z_sign) + (surface_centers[i].z - new_position.z)) - (surface_centers[i].z - new_position.z)) * Math.Abs(z_sign)) + new_position.z);
                surface_centers[i] = new_v; 
            }
            for (int i = 0; i < surface_centers.Count; i++)
            {
                Vector3 point = surface_centers[i];
                for (int j = 0; j < surface_centers.Count; j++)
                {
                    if (i != j && point == surface_centers[j])
                    {
                        Debug.Log("duplicate found: " + i + ", " + j);
                    }
                }
            }

        }
        
         
    }
    

    private void checkSubDivision() 
    {
        if (is_top && NBodies.Count > nbody_cap)
        {
            
        }
        else if (NBodies.Count > nbody_cap)
        {
            subDivide();
        }
        else if (sub_divide == true)
        {
            subDivide();
        }
    }

    private void createCube()
    {
        //8 points on a cube
        point_definitions.Add(new Vector3(position.x + 1, position.y +(-1), position.z + 1));
        point_definitions.Add(new Vector3(position.x + 1, position.y +(-1), position.z +(-1)));
        point_definitions.Add(new Vector3(position.x + 1, position.y + 1, position.z + 1));
        point_definitions.Add(new Vector3(position.x + 1, position.y + 1, position.z + (-1)));

        point_definitions.Add(new Vector3(position.x +(-1), position.y + 1, position.z + 1));
        point_definitions.Add(new Vector3(position.x +(-1), position.y + 1, position.z +(-1)));
        point_definitions.Add(new Vector3(position.x +(-1), position.y +(-1), position.z +(-1)));
        point_definitions.Add(new Vector3(position.x +(-1), position.y +(-1), position.z + (1)));

        foreach(Vector3 point in point_definitions)
        {
            Debug.Log(point);
        }
    }

    private void drawCube() 
    {
        Debug.DrawLine(point_definitions[0], point_definitions[1], Color.white, 1);
        Debug.DrawLine(point_definitions[0], point_definitions[2], Color.white, 1);
        Debug.DrawLine(point_definitions[0], point_definitions[7], Color.white, 1);
        Debug.DrawLine(point_definitions[7], point_definitions[4], Color.white, 1);
        Debug.DrawLine(point_definitions[7], point_definitions[6], Color.white, 1);
        Debug.DrawLine(point_definitions[4], point_definitions[2], Color.white, 1);
        Debug.DrawLine(point_definitions[4], point_definitions[5], Color.white, 1);
        Debug.DrawLine(point_definitions[3], point_definitions[1], Color.white, 1);
        Debug.DrawLine(point_definitions[3], point_definitions[5], Color.white, 1);
        Debug.DrawLine(point_definitions[5], point_definitions[6], Color.white, 1);
        Debug.DrawLine(point_definitions[6], point_definitions[1], Color.white, 1);
        Debug.DrawLine(point_definitions[3], point_definitions[2], Color.white, 1);


        for (int i = 0; i < surface_centers.Count; i++) 
        {
            //Redner.DrawCube(position, new Vector3(size / 20, size / 20, size / 20));
            Debug.DrawLine(surface_centers[i], (surface_centers[i]) * 2, Color.yellow, 1);
        }
    }

    private void subDivide() 
    { 
        
    }

    private void surfaceCenterCreation() 
    {
        //bottom
        surface_centers.Add(new Vector3(position.x + 0, position.y + - 1, position.z + 1));
        surface_centers.Add(new Vector3(position.x + - 1, position.y + -1, position.z + 0));
        surface_centers.Add(new Vector3(position.x + 0, position.y + -1, position.z + -1));
        surface_centers.Add(new Vector3(position.x + 1, position.y + -1, position.z + 0));

        //top
        surface_centers.Add(new Vector3(position.x + 0, position.y + 1, position.z + 1));
        surface_centers.Add(new Vector3(position.x  + - 1, position.y + 1, position.z + 0));
        surface_centers.Add(new Vector3(position.x + 0, position.y + 1, position.z + -1));
        surface_centers.Add(new Vector3(position.x + 1, position.y + 1, position.z + 0));

        //middle
        surface_centers.Add(new Vector3(position.x + - 1, position.y + 0, position.z + 1));
        surface_centers.Add(new Vector3(position.x + - 1, position.y + 0, position.z + -1));
        surface_centers.Add(new Vector3(position.x + 1, position.y + 0, position.z + -1));
        surface_centers.Add(new Vector3(position.x + 1, position.y + 0, position.z + 1));

        //surface centers
        surface_centers.Add(new Vector3(position.x + 1, position.y + 0, position.z + 0));
        surface_centers.Add(new Vector3(position.x + 0, position.y + 0, position.z + -1));
        surface_centers.Add(new Vector3(position.x + - 1, position.y + 0, position.z + 0));
        surface_centers.Add(new Vector3(position.x + 0, position.y + 0, position.z + 1));
        surface_centers.Add(new Vector3(position.x + 0, position.y + -1, position.z + 0));
        surface_centers.Add(new Vector3(position.x + 0, position.y + 1, position.z + 0));



    }
}
