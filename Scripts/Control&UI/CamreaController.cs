using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class CamreaController : MonoBehaviour
{
    private float speed = 3000f;
    private Transform tt;
    
    private bool mouseButtonHeld = false;
    private Vector2 mousePos1;
    private Vector2 mousePos2;
    private float turnSpeed = 200f;

    public GameObject planetCopy;
    public GameObject starCopy;
    public int body_index;
    
    public GameObject objectList;
    private static bool can_place = true;
    private static float mass = 10;
    private static float radius = 2;
    private static bool circle_orbit = true;
    
    private bool placementOn = true;

    public GameObject quitButtonObj;
    private Button quitButton;
    void Start()
    {
        tt = this.gameObject.transform;

        quitButton = quitButtonObj.GetComponent<Button>();
        quitButton.onClick.AddListener(quitListener);
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
      
            this.transform.Rotate(0, 0, Input.GetAxis("Vertical")*Time.unscaledDeltaTime*turnSpeed, Space.Self);
        }

        if (Input.GetAxis("RMB") > 0)
        {
            Debug.Log("RMB");
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
                this.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            }
            else
            {
                this.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("hop up");
            this.transform.SetParent(this.transform.parent.transform.parent.transform);
        }
        
        //&& this.transform.GetChild(0).GetComponent<Canvas>().transform.GetChild(1).GetComponent<Toggle>().value
        //this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<Toggle>().value
        if (Input.GetKeyDown(KeyCode.Mouse0) && !this.transform.GetChild(0).GetComponent<Canvas>().enabled && (can_place))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                createBody((hit));
                
            }
        }
        //used as a fail safe
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_InputField>().text = "1";
        }
    }

    private void createBody(RaycastHit hit)
    {
        GameObject new_body = Instantiate(getObject());
        new_body.transform.parent = objectList.transform;
                
        new_body.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
        
        if (circle_orbit)
            new_body.GetComponent<CicrularVelocityTool>().setInitializationVelocity();
        
        new_body.GetComponent<BodyData>().mass = mass;
        new_body.GetComponent<BodyData>().changeRadi(radius);
        new_body.GetComponent<BodyData>().cc = this.gameObject;

        if (body_index == 0)
        {
            Color[] star_colors = {
                new Color(157/225, 180/225, 255/225),  
                new Color(162/225, 185/225, 255/225),  
                new Color(167/225, 188/225, 255/225),
                new Color(228/225, 232/225, 255/225), 
                new Color(237/225, 238/225, 255/225),  
                new Color(251/225, 248/225, 255/225),
                new Color(255/225, 241/225, 223/225),  
                new Color(255/225, 235/225, 209/225),  
                new Color(255/225, 215/225, 174/225),
                new Color(255/225, 187/225, 123/225),  
                new Color(255/225, 187/225, 123/225) };
            
            
            Color color = star_colors[Random.Range(0, star_colors.Length-1)];
            new_body.GetComponent<Light>().color = color;
            new_body.GetComponent<Light>().intensity = Mathf.Pow(radius, 2)*100;
            new_body.GetComponent<Light>().range = radius*1000;
            Material material = new_body.GetComponent<MeshRenderer>().material;
            material.SetColor("_BaseColor", color);
            material.SetColor("_EmissionColor", color * .7f);
            Debug.Log(color);

        }
    }

    public void setPlaceTool(bool onOff)
    {
        can_place = onOff;
    }

    public void setMovementSpeed()
    {
        speed = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(speed);
    }

    public void setRotationSpeed()
    {
        turnSpeed = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(turnSpeed);
    }

    public void setRadius()
    {
        radius = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(0).GetComponent<TMP_InputField>().text);
        Debug.Log(radius);
    }

    public void setMass()
    {
        
        mass = float.Parse(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(0).GetComponent<TMP_InputField>().text);
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
        body_index = this.transform.GetChild(0).GetChild(0).GetChild(10).GetChild(1).GetComponent<TMP_Dropdown>().value;
        Debug.Log(body_index);
    }

    private GameObject getObject()
    {
        switch (body_index)
        {
            case 0:
                return starCopy;
            case 1:
                return planetCopy;
            case 2:
                return planetCopy;
        }

        return null;
    }


}
