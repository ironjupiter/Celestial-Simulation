using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CamreaController : MonoBehaviour
{
    private float speed = 25f;
    private Transform tt;
    
    private bool mouseButtonHeld = false;
    private Vector2 mousePos1;
    private Vector2 mousePos2;
    private float turnSpeed = 10f;
    
    void Start()
    {
        tt = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

        controller();
    }

    void controller()
    {
        if (Input.GetAxis("Sideways") != 0)
        {
         
            tt.position += tt.forward * Input.GetAxis("Sideways") * speed * Time.deltaTime;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
    
            tt.position += tt.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
      
            this.transform.Rotate(0, 0, Input.GetAxis("Vertical")*turnSpeed*Time.deltaTime*10, Space.Self);
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
                Vector2 difference = (mousePos2 - mousePos1)*Time.deltaTime*turnSpeed;
                //Debug.Log(difference);
                mousePos1 = mousePos2;
                this.transform.Rotate(difference.y, difference.x, 0.0f, Space.Self);
            }
        }
        else
        {
            mouseButtonHeld = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (this.transform.GetChild(0).GetComponent<Canvas>().enabled == true)
            {
                this.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            }
            else
            {
                this.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            this.transform.SetParent(null);
        }
    }

    public void setMovementSpeed(float input)
    {
        speed = input;
    }

    public void setRotationSpeed(float input)
    {
        turnSpeed = input;
    }

}
