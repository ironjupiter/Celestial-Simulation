using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class CamreaController : MonoBehaviour
{
    public float speed = 3000f;
    private Transform tt;
    
    private bool mouseButtonHeld = false;
    private Vector2 mousePos1;
    private Vector2 mousePos2;
    public float turnSpeed = 200f;

    public GameObject planetCopy;
    public GameObject starCopy;
    public int body_index;
    
    public GameObject objectList;
    public  bool can_place = true;
    private static float mass = 10;
    private static float radius = 2;
    private static bool circle_orbit = true;
    
    private bool placementOn = true;

    public GameObject quitButtonObj;
    private Button quitButton;
    void Start()
    {
        tt = this.gameObject.transform;
    }

    void quitListener()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.mousePosition);
        movement();
        controller();
    }

    void movement()
    {
        if (this.TryGetComponent<SimulationUIManager>(out SimulationUIManager s))
        {
            SimulationUIManager suim = this.GetComponent<SimulationUIManager>();
            if (suim.escapeMenu.enabled == true || suim.planetTool.enabled == true)
            {
                return;
            }
        }

        if (Input.GetAxis("Sideways") != 0)
        {
         
            tt.position += tt.forward * Input.GetAxis("Sideways") * Time.unscaledDeltaTime*speed;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
    
            tt.position += tt.right * Input.GetAxis("Horizontal") * Time.unscaledDeltaTime*speed;
        }

        if (Input.GetAxis("VerticalMov") != 0)
        {
            tt.position += tt.up * Input.GetAxis("VerticalMov") * Time.unscaledDeltaTime * speed;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
      
            this.transform.Rotate(0, 0, Input.GetAxis("Vertical")*Time.unscaledDeltaTime*200, Space.Self);
        }

        if (Input.GetAxis("RMB") > 0)
        {
            //Debug.Log("RMB");
            //Debug.Log(Input.mousePosition);

            if (!mouseButtonHeld)
            {
                mouseButtonHeld = true;
                mousePos1 = Input.mousePosition;
            } 
            else if (mouseButtonHeld)
            {
                mousePos2 = Input.mousePosition;
                Vector2 difference = (mousePos2 - mousePos1)*Time.unscaledDeltaTime*turnSpeed;
                //Debug.Log(difference);
                mousePos1 = mousePos2;
                this.transform.Rotate(difference.y, difference.x, 0.0f, Space.Self);
            }
        }
        else
        {
            mouseButtonHeld = false;
        }
    }

    void controller()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (this.transform.GetChild(0).GetComponent<Canvas>().enabled)
            {
                //this.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            }
            else
            {
                //this.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("hop up");
            this.transform.SetParent(this.transform.parent.transform.parent.transform);
        }
        
        //&& this.transform.GetChild(0).GetComponent<Canvas>().transform.GetChild(1).GetComponent<Toggle>().value
        //this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<Toggle>().value
        
        //used as a fail safe
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
        }
    }

    

    public void setPlaceTool(bool onOff)
    {
        can_place = onOff;
    }

    public void setMovementSpeed()
    {
        //speed = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(speed);
    }

    public void setRotationSpeed()
    {
        //turnSpeed = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(turnSpeed);
    }

    public void setRadius()
    {
        //radius = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(radius);
    }

    public void setMass()
    {
        
        //mass = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(mass);
    }

    public void setOrbit()
    {
        if (circle_orbit)
        {
            circle_orbit = false;
        }
        else
        {
            circle_orbit = true;
        }
    }
    
    public void setBodyType()
    {
        //body_index = this.transform.GetChild(0).GetChild(0).GetChild(10).GetChild(1).GetComponent<TMP_Dropdown>().value;
        Debug.Log(body_index);
    }

    


}
