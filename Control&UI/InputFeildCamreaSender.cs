using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputFeildCamreaSender : MonoBehaviour
{
    private float inputf;
    private TMP_InputField input;
    private CamreaController cc;
    public GameObject camObj;
    public bool isSpeed = false;

    private void Start()
    {
        input = this.gameObject.GetComponent<TMP_InputField>();
        cc = camObj.GetComponent<CamreaController>();
        inputf = float.Parse(input.text);
        if (isSpeed)
        {
            cc.setMovementSpeed(inputf);
        }
        else
        {
            cc.setRotationSpeed(inputf);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!(float.Parse(input.text) == inputf))
        {
            if (isSpeed)
            {
                cc.setMovementSpeed(inputf);
            }
            else
            {
                cc.setRotationSpeed(inputf);
            }

            inputf = float.Parse(input.text);
        }
    }
}
