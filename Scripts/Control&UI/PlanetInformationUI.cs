using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlanetInformationUI : MonoBehaviour
{
    public GameObject HighestParent;
    
    public GameObject nameUI;
    public GameObject massUI;
    public GameObject radiusUI;
    public GameObject speedUI;
    public GameObject camera;

    public GameObject previousButtonObj;
    private Button previousButton;
    public GameObject nextButtonObj;
    private Button nextButton;
    public GameObject DeselectButtonObj;
    private Button deselectButton;
    public GameObject RandomButtonObj;
    private Button randomButton;
    void Start()
    {
        previousButton = previousButtonObj.GetComponent<Button>();
        previousButton.onClick.AddListener(PreviousButtonListener);
        nextButton = nextButtonObj.GetComponent<Button>();
        nextButton.onClick.AddListener(NextButtonListener);
        deselectButton = DeselectButtonObj.GetComponent<Button>();
        deselectButton.onClick.AddListener(DeselectButtonListener);
        randomButton = RandomButtonObj.GetComponent<Button>();
        randomButton.onClick.AddListener(RandomButtonListener);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (camera.transform.parent.GetComponent<PlanetScript>() != null)
        {
            string body_type = camera.transform.parent.GetComponent<BodyData>().bodytype;
            nameUI.GetComponent<TMP_Text>().text =  body_type + " Name: \n" + camera.transform.parent.GetComponent<BodyData>().name;
            massUI.GetComponent<TMP_Text>().text = body_type + " Mass: \n" + camera.transform.parent.GetComponent<BodyData>().mass;
            radiusUI.GetComponent<TMP_Text>().text = body_type + " Radius: \n" + camera.transform.parent.GetComponent<BodyData>().radius;
            speedUI.GetComponent<TMP_Text>().text = body_type + " Speed: \n" + camera.transform.parent.GetComponent<BodyData>().velocity.magnitude;
        }
        else
        {
            nameUI.GetComponent<TMP_Text>().text = " Name: \n N/A";
            massUI.GetComponent<TMP_Text>().text = " Mass: \n N/A";
            radiusUI.GetComponent<TMP_Text>().text = " Radius: \n N/A";
            speedUI.GetComponent<TMP_Text>().text = " Speed: \n N/A";
        }
    }


    void NextButtonListener()
    {
        if (camera.transform.parent != HighestParent)
        {
            try
            {
                setCameraParent(camera.transform.parent.parent.GetChild(camera.transform.parent.GetSiblingIndex() + 1));
            }
            catch
            {
                setCameraParent(camera.transform.parent.parent.GetChild(0));
            }
        }

    }
    void PreviousButtonListener()
    {
        if (camera.transform.parent != HighestParent)
        {
            try
            {
                setCameraParent(camera.transform.parent.parent.GetChild(camera.transform.parent.GetSiblingIndex() - 1));
            }
            catch
            {
                setCameraParent(camera.transform.parent.parent.GetChild(camera.transform.parent.childCount-1));
            }
        }
    }
    void DeselectButtonListener()
    {
        if(camera.transform.parent != HighestParent)
            setCameraParent(camera.transform.parent.parent);
    }

    void RandomButtonListener()
    {
        setCameraParent(HighestParent.transform.GetChild(Random.Range(0, HighestParent.transform.childCount)));
    }

    void setCameraParent(Transform new_parent)
    {
        if (new_parent != HighestParent)
        {
            camera.transform.SetParent(new_parent);
            float distance_constant = .7f;
            camera.transform.position =
                new_parent.position + new Vector3(new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant, 
                    new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant, 
                    new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant);
            camera.transform.LookAt(new_parent);
        }
        else
        {
            camera.transform.SetParent(new_parent);
        }
    }
    
    public static void setCameraParent(GameObject camera,Transform new_parent)
    {
        camera.transform.SetParent(new_parent);
        float distance_constant = .7f;
        camera.transform.position =
            new_parent.position + new Vector3(new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant, 
                new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant, 
                new_parent.GetComponent<BodyData>().radius + new_parent.GetComponent<BodyData>().radius*distance_constant);
        camera.transform.LookAt(new_parent);

    }
}
