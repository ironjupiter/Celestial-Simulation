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
    public GameObject planetPrefab;

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
    public GameObject DiskButtonObj;
    private Button diskButton;
    public GameObject ClearButtonObj;
    private Button clearButton;
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
        diskButton = DiskButtonObj.GetComponent<Button>();
        diskButton.onClick.AddListener(DiskSpawner);
        clearButton = ClearButtonObj.GetComponent<Button>();
        clearButton.onClick.AddListener(WorldClear);
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

    void WorldClear()
    {
        List<GameObject> all_bodies = PhysicsSynchronizer.getBodyList();
        for (int i = 0; i < all_bodies.Count;)
        {
            PlanetScript.destroyPlanet(all_bodies[i]);
        }
    }


    void DiskSpawner()
    {
        if (camera.transform.parent.gameObject == HighestParent)
        {
            return;
        }

        int spawn_num = 100;
        float child_parent_mass_ratio = .001f;
        float child_parent_radius_ratio = .01f;
        float radius_ratio = 10f;
        
        createBody(spawn_num, child_parent_mass_ratio, child_parent_radius_ratio, radius_ratio);

    }

    private void createBody(int spawn_num, float child_parent_mass_ratio, float child_parent_radius_ratio, float radius_ratio)
    {
        for (int i = 0; i < spawn_num; i++)
        {
        GameObject new_body = Instantiate(planetPrefab);
        new_body.transform.parent = HighestParent.transform;

        Vector2 temp_pos = setRelativePosition(radius_ratio);
        Vector3 relative_position = new Vector3(temp_pos.x, 0, temp_pos.y);
        new_body.transform.position = relative_position + camera.transform.parent.position;
        
        new_body.GetComponent<CicrularVelocityTool>().setInitializationVelocity(camera.transform.parent.gameObject);
        
        new_body.GetComponent<BodyData>().mass = camera.transform.parent.GetComponent<BodyData>().mass * child_parent_mass_ratio;
        new_body.GetComponent<BodyData>().changeRadi(camera.transform.parent.GetComponent<BodyData>().radius * child_parent_radius_ratio);
        new_body.GetComponent<BodyData>().cc = camera;
            
        }
    }

    private Vector2 setRelativePosition(float radius_ratio)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(
            (camera.transform.parent.GetComponent<BodyData>().radius +
            1),
            camera.transform.parent.GetComponent<BodyData>().radius * radius_ratio);

        if (angle == Mathf.PI && angle == 0 && angle == 2 * Mathf.PI)
        {
            return new Vector2(Mathf.Cos(angle) * distance, 0);
        }
        if (angle == Mathf.PI/2 && angle == (3*Mathf.PI)/2)
        {
            return new Vector2(0, Mathf.Sin(angle) * distance);
        }
        return new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);

        

        
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
