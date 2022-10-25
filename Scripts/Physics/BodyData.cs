﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.UIElements.MouseButton;


public class BodyData : MonoBehaviour
{
    public int id = 0;
    private static int id_setup = 0;

    public float mass;
    public float radius;

    public long rotation_period;
    public Vector3 velocity = new Vector3 (0, 0, 0);
    public Vector3 momentuem;
    public Vector3 spin_vector;

    public Vector3 impulse = new Vector3(0, 0, 0);

    public GameObject cc;

    // Start is called before the first frame update
    void Awake()
    {
        id = id_setup;
        id_setup++;

        if (radius == 0)
            radius = mass;
        
    }

    public void changeRadi(float r) 
    {
        radius = r;
        this.GetComponent<PlanetScript>().findRadius(r);
        this.GetComponent<TrailRenderer>().widthMultiplier = r / 4;
    }

    public void OnMouseDown()
    {
        Debug.Log("pew");
        cc.transform.SetParent(this.transform);
        cc.transform.position = this.transform.position + new Vector3(this.radius+1, 0 ,0);
        cc.transform.LookAt(this.transform);
    }
}