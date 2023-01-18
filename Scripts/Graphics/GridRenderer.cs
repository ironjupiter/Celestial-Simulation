using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    private GridSystem grid = new GridSystem();
    public GameObject Camera; 

    private float distanceFromPlane;
    private float distanceFromCenterAxis;
    private float ySign;
    private int current_quotient = 0;

    public bool update = false;

    private float magnitudeConstant = 10;
    private float current_magnitude;

    public float length = 500;
    private int points = 50;//must be even!
    private const float line_width = .05f;
    
    private void Start()
    {
        points = (int)(length / 10);
        
        //this.transform.AddComponent<>()
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 cam_pos = this.Camera.transform.position;
        distanceFromPlane = cam_pos.y;
        ySign = distanceFromPlane / Mathf.Abs(distanceFromPlane);
        distanceFromCenterAxis = Vector2.Distance(new Vector2(cam_pos.x, cam_pos.z), new Vector2(this.transform.position.x, this.transform.position.z));

        updateScale();
        updateRotation();
        updateLocation();
    }

    void updateLocation()
    {


        float grid_block = ((length / points) * current_quotient);
        Debug.Log(grid_block);
        
        
        if (distanceFromCenterAxis > grid_block && grid_block != 0)
        { 
            Vector3 cam_pos = this.Camera.transform.position;

            float x_move = Mathf.Floor((cam_pos.x - this.transform.position.x)/grid_block)*grid_block;
            float z_move = Mathf.Floor((cam_pos.z - this.transform.position.z)/grid_block)*grid_block;
            
            this.transform.position += (new Vector3(x_move* ySign, 0, z_move* ySign));
        }
    }


    void updateScale()
    {
        int magnitude_power = (int)Mathf.Floor(Mathf.Log10(Mathf.Abs(distanceFromPlane))/Mathf.Log10(magnitudeConstant));
        float quotient = MathF.Floor (distanceFromPlane / magnitudeConstant);

        if ((current_quotient != quotient && magnitude_power != current_magnitude) || update)
        {
            current_magnitude = magnitude_power;
            
            this.GetComponent<MeshFilter>().mesh = grid.createMesh((length* quotient),
                (int)((points)* 1), 
                (line_width* quotient));
            current_quotient = (int)quotient;
        }
    }

    void updateRotation()
    {
        //Debug.Log(ySign);
        switch (ySign)
        {
            
            case -1:
                this.transform.rotation = Quaternion.Euler(180, 0, 0);
                break;
            
            case 1:
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            
        }
    }
}
