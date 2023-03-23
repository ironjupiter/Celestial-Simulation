using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;


public class SimulationUIManager : MonoBehaviour
{
    /// <summary>
    ///important for global control
    /// </summary>
    [Header("High Level Items")]
    public GameObject HighestParent;
    public GameObject planetPrefab;
    public GameObject starPrefab;

    public Canvas planetInfo;
    public Canvas planetTool;
    public Canvas escapeMenu;
    
    public GameObject camera;
    /// <summary>
    ///stuff on the planet info ui
    /// </summary>
    [Header("Planet Info UI")]
    public TMP_Text nameUI;
    public TMP_Text massUI;
    public TMP_Text radiusUI;
    public TMP_Text speedUI;
    public TMP_Text numberOfBodiesUI;
    
    public Button previousButton;
    public Button nextButton;
    public Button deselectButton;
    public Button randomButton;
    public Button diskButton;
    public Button clearButton;
    public Button planetMenuButton;
    public Button settingsMenuButton;
    
    
    public Slider timeMultiplier;
    public TMP_Text timeMultiplierText;

    /// <summary>
    /// stuff on planet tool menu
    /// </summary>
    [Header("Planet Tool Menu")]
    public TMP_InputField fieldOne;
    public TMP_InputField fieldTwo;
    public TMP_Text textFeildOne;
    public TMP_Text textFeildTwo;

    public TMP_Dropdown bodySelector;

    public Toggle placementToggle;
    public Toggle circularOrbitToggle;
    
    public Button leaveMenuButton;
    public Button switchEquationButton;
    public TMP_Text switchEquationText;
    
    private int equationTypeEnumeration = 0;
    private int bodyIndex = 0;
    private bool circularOrbits = true;
    private bool placementOn = true;

    /// <summary>
    /// stuff for escape menu
    /// </summary>
    [Header("Settings Menu")] 
    public GameObject escapeMenuPanel;
    public TMP_InputField movementSpeed;
    public TMP_InputField sensitivity;

    public TMP_Dropdown collisionAlgorithm;

    public Toggle gravityToggle;
    
    public Button saveGameButton;
    public Button leaveToMainMenu;
    public Button leaveGame;
    [FormerlySerializedAs("leaveMenus")] public Button leaveSettingsMenuButton;

    [Header("Save Name Panel")] 
    public GameObject fileNamePanel;

    public Button saveNameButton;
    public Button cancelSaveButton;
    public TMP_InputField nameInput;
    
    void Start()
    {

        previousButton.onClick.AddListener(PreviousButtonListener);
        nextButton.onClick.AddListener(NextButtonListener);
        deselectButton.onClick.AddListener(DeselectButtonListener);
        randomButton.onClick.AddListener(RandomButtonListener);
        diskButton.onClick.AddListener(DiskSpawner);
        clearButton.onClick.AddListener(WorldClear);
        planetMenuButton.onClick.AddListener(PlanetMenuListener);
        settingsMenuButton.onClick.AddListener(SettingsMenuListener);
        timeMultiplier.onValueChanged.AddListener(timeMultiplierListener);
        
        leaveMenuButton.onClick.AddListener(leavePlanetMenuListener);
        switchEquationButton.onClick.AddListener(equationSwitcher);
        bodySelector.onValueChanged.AddListener(bodyIndexSwitcher);
        placementToggle.onValueChanged.AddListener(placementToggleListener);
        circularOrbitToggle.onValueChanged.AddListener(orbitalToggleListener);
        
        leaveSettingsMenuButton.onClick.AddListener(LeaveSettingsMenuListener);
        leaveGame.onClick.AddListener(Application.Quit);
        leaveToMainMenu.onClick.AddListener(switchSceneMain);
        movementSpeed.onValueChanged.AddListener(updateSpeed);
        sensitivity.onValueChanged.AddListener(updateSensitivity);
        collisionAlgorithm.onValueChanged.AddListener(updateCollision);
        gravityToggle.onValueChanged.AddListener(updateGravity);
        saveGameButton.onClick.AddListener(saveGameListener);
        
        saveNameButton.onClick.AddListener(saveGame);
        cancelSaveButton.onClick.AddListener(cancelSaveListener);
    }

    void cancelSaveListener()
    {
        escapeMenuPanel.SetActive(true);
        fileNamePanel.SetActive(false);
    }

    void saveGame()
    {
        string name = nameInput.text;
        SaveSystem.serializeSimulation(PhysicsSynchronizer.getBodyList(), name);
        escapeMenuPanel.SetActive(true);
        fileNamePanel.SetActive(false);
    }

    void saveGameListener()
    {
        Debug.Log("switch menus");
        escapeMenuPanel.SetActive(false);
        fileNamePanel.SetActive(true);
        nameInput.text = "save"+ DateTime.UtcNow.Millisecond;
    }

    void updateGravity(bool value)
    {
        PhysicsSynchronizer.gravityOn = value;
    }

    void updateCollision(int value)
    {
        PhysicsSynchronizer.collisionAlgorithm = value;
    }

    void updateSpeed(string value)
    {
        camera.GetComponent<CamreaController>().speed = float.Parse(value);
    }

    void updateSensitivity(string value)
    {
        camera.GetComponent<CamreaController>().turnSpeed = float.Parse(value);
    }

    void Update()
    {
        timeMultiplierText.text = "Time Multiplier: x" + Time.timeScale;
        numberOfBodiesUI.text =  "Number of physics bodies in simulation: \n " + PhysicsSynchronizer.getBodyList().Count;
        
        if (camera.transform.parent.GetComponent<PlanetScript>() != null)
        {
            string body_type = camera.transform.parent.GetComponent<BodyData>().bodytype;
            nameUI.text =  body_type + " Name: \n" + camera.transform.parent.GetComponent<BodyData>().name;
            massUI.text = body_type + " Mass: \n" + camera.transform.parent.GetComponent<BodyData>().mass;
            radiusUI.text = body_type + " Radius: \n" + camera.transform.parent.GetComponent<BodyData>().radius;
            speedUI.text = body_type + " Speed: \n" + camera.transform.parent.GetComponent<BodyData>().velocity.magnitude;
            
        }
        else
        {
            nameUI.GetComponent<TMP_Text>().text = " Name: \n N/A";
            massUI.GetComponent<TMP_Text>().text = " Mass: \n N/A";
            radiusUI.GetComponent<TMP_Text>().text = " Radius: \n N/A";
            speedUI.GetComponent<TMP_Text>().text = " Speed: \n N/A";
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject() && placementOn)
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                createBody((hit));
                
            }
        }
    }

    void switchSceneMain()
    {
        WorldClear();
        SceneManager.LoadScene(0);
    }

    void orbitalToggleListener(bool value)
    {
        circularOrbits = value;
    }
    
    void placementToggleListener(bool value)
    {
        placementOn = value;
    }

    void bodyIndexSwitcher(int index)
    {
        bodyIndex = index;
    }

    private void createBody(RaycastHit hit)
    {
        GameObject new_body = Instantiate(getObject());
        new_body.transform.parent = HighestParent.transform;
                
        new_body.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
        
        if (circularOrbits)//place holder fix soon
            new_body.GetComponent<CicrularVelocityTool>().setInitializationVelocity();

        float mass = 0;
        float radius = 0;
        float density = 0;

        switch (equationTypeEnumeration)
        {
            case 0:// density = mass/volume
                mass = float.Parse(fieldOne.text);
                radius = float.Parse(fieldTwo.text);
                break;
            case 1://  mass = density*volume
                density = float.Parse(fieldOne.text);
                radius = float.Parse(fieldTwo.text);
                mass = density * radius;
                break;
            case 2://  volume = mass/density
                mass = float.Parse(fieldOne.text);
                density = float.Parse(fieldTwo.text);
                radius = mass / density;
                break;
        }
        
        
        
        new_body.GetComponent<BodyData>().mass = mass;
        new_body.GetComponent<BodyData>().changeRadi(radius);
        new_body.GetComponent<BodyData>().cc = this.gameObject;

        if (bodyIndex == 0)
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
            new_body.GetComponent<BodyData>().star_color = color;
            new_body.GetComponent<Light>().intensity = Mathf.Pow(radius, 2)*100;
            new_body.GetComponent<Light>().range = radius*1000;
            Material material = new_body.GetComponent<MeshRenderer>().material;
            material.SetColor("_BaseColor", color);
            material.SetColor("_EmissionColor", color * .7f);
            Debug.Log(color);

        }
    }

    private GameObject getObject()
    {
        switch (bodyIndex)
        {
            case 0:
                return starPrefab;
            case 1:
                return planetPrefab;
            case 2:
                return planetPrefab;
        }

        return null;
    }

    void equationSwitcher()
    {
        equationTypeEnumeration++;
        if (equationTypeEnumeration > 2)
        {
            equationTypeEnumeration = 0;
        }

        switch (equationTypeEnumeration)
        {
            //note radius is the volume, just a less stupid way of saying it for user experience
            case 0:// density = mass/volume
                textFeildOne.text = "Body Mass";
                textFeildTwo.text = "Body Radius";
                switchEquationText.text = "Switch Equation\nCurrent: p=m/v";
                break;
            case 1://  mass = density*volume
                textFeildOne.text = "Body Density";
                textFeildTwo.text = "Body Radius";
                switchEquationText.text = "Switch Equation\nCurrent: m=p*v";
                break;
            case 2://  volume = mass/density
                textFeildOne.text = "Body Mass";
                textFeildTwo.text = "Body Density";
                switchEquationText.text = "Switch Equation\nCurrent: v=m/p";
                break;
        }
    }

    void LeaveSettingsMenuListener()
    {
        planetInfo.enabled = true;
        escapeMenu.enabled = false;
        this.gameObject.GetComponent<CamreaController>().can_place = true;
    }

    void leavePlanetMenuListener()
    {
        planetInfo.enabled = true;
        planetTool.enabled = false;
        this.gameObject.GetComponent<CamreaController>().can_place = true;
    }

    void timeMultiplierListener(float scale)
    {
        scale = timeMultiplier.value * 10;
        scale = Mathf.Round(scale);
        scale = scale / 10;
        Time.timeScale = scale;
        timeMultiplierText.text = "Time Multiplier: x" + scale;
    }

    void SettingsMenuListener()
    {
        planetInfo.enabled = false;
        escapeMenu.enabled = true;
        this.gameObject.GetComponent<CamreaController>().can_place = false;
    }

    void PlanetMenuListener()
    {
        planetInfo.enabled = false;
        planetTool.enabled = true;
        this.gameObject.GetComponent<CamreaController>().can_place = false;
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

        int spawn_num = 1000;
        float child_parent_mass_ratio = .0001f;
        float child_parent_radius_ratio = .01f;
        float radius_ratio = 20f;
        
        createBody(spawn_num, child_parent_mass_ratio, child_parent_radius_ratio, radius_ratio);

    }

    private void createBody(int spawn_num, float child_parent_mass_ratio, float child_parent_radius_ratio, float radius_ratio)
    {
        for (int i = 0; i < spawn_num; i++)
        {
            GameObject new_body = Instantiate(planetPrefab);
            new_body.transform.parent = HighestParent.transform;

            Vector2 temp_pos = setRelativePosition(radius_ratio);
            //Debug.Log("temp_pos_" + temp_pos);
            Vector3 relative_position = new Vector3(temp_pos.x, 0, temp_pos.y);
            //Debug.Log("relative_position_" + relative_position);
            new_body.transform.position = relative_position + camera.transform.parent.position;
            //Debug.Log("new_body.transform.position_" + new_body.transform.position);
            
            new_body.GetComponent<CicrularVelocityTool>().setInitializationVelocity(camera.transform.parent.gameObject);
            
            new_body.GetComponent<BodyData>().mass = camera.transform.parent.GetComponent<BodyData>().mass * child_parent_mass_ratio;
            new_body.GetComponent<BodyData>().changeRadi(camera.transform.parent.GetComponent<BodyData>().radius * child_parent_radius_ratio);
            new_body.GetComponent<BodyData>().cc = camera;
            new_body.GetComponent<BodyData>().position_read = new_body.transform.position;

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
        if (this.transform.parent != HighestParent)
        {
            try
            {
                setCameraParent(this.transform.parent.parent.GetChild(this.transform.parent.GetSiblingIndex() + 1));
            }
            catch
            {
                setCameraParent(this.transform.parent.parent.GetChild(0));
            }
        }

    }
    
    void PreviousButtonListener()
    {
        if (this.transform.parent != HighestParent)
        {
            try
            {
                setCameraParent(this.transform.parent.parent.GetChild(this.transform.parent.GetSiblingIndex() - 1));
            }
            catch
            {
                setCameraParent(this.transform.parent.parent.GetChild(this.transform.parent.parent.childCount-1));
            }
        }
    }
    
    void DeselectButtonListener()
    {
        if(this.transform.parent != HighestParent)
            setCameraParent(this.transform.parent.parent);
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