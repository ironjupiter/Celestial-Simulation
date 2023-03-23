using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuController : MonoBehaviour
{
    //temp for scene managment, 1 is simulation, 0 is main menu
    public int simulation_scene = 1;

    public Canvas mainCanvas;
    public Canvas savedCanvas;

    public TMP_Text splash_text;
    public GameObject saveFilePrefab;
    public GameObject saveFileContent;
    
    public GameObject startButtonObj;
    public GameObject savedButtonObj;
    public GameObject exitButtonObj;
    public GameObject backButtonObj;
    
    private Button startButton;
    private Button savedButton;
    private Button exitButton;
    private Button backButton;

    public Button deleteAllFilesButton;

    public GameObject saveFileAnchor;

    private string fileAppendage = ".sdata";
    // Start is called before the first frame update
    void Start()
    {
        startButton = startButtonObj.GetComponent<Button>();
        startButton.onClick.AddListener(startButtonAction);
        
        savedButton = savedButtonObj.GetComponent<Button>();
        savedButton.onClick.AddListener(savedButtonAction);
        
        exitButton = exitButtonObj.GetComponent<Button>();
        exitButton.onClick.AddListener(exitButtonAction);
        
        backButton = backButtonObj.GetComponent<Button>();
        backButton.onClick.AddListener(backButtonAction);
        
        deleteAllFilesButton.onClick.AddListener(deleteAllFiles);
        
        string []text_array = { "Some reassembly required!", "As seen on tv!", "Proving murphy's law",  "Finding the ultimate question", "Calculating the ultimate answer", "Untangling Orbits"};
        splash_text.text = text_array[Mathf.FloorToInt(Random.Range(0, text_array.Length))];
    }

    private void deleteAllFiles()
    {
        string[] files_path;
        try
        {
            files_path = getSaveFiles(SaveSystem.getSaveDirectory());
        }
        catch
        {
            Debug.Log("Save Directory Not Found");
            return;
        }

        List<string> files = new List<string>();

        for (int i = 0; i < files_path.Length; i++)
        {
            files.Add(files_path[i]);
        }

        //Destroy();
        int buttons_num = saveFileContent.transform.childCount;
        GameObject[] file_buttons = new GameObject[saveFileContent.transform.childCount];

        for (int i = 0; i < saveFileContent.transform.childCount; i++)
        {
            file_buttons[i] = saveFileContent.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < file_buttons.Length; i++)
        {
            Destroy(file_buttons[i]);
        }
        
        for (int i = 0; i < files.Count;)
        {
            System.GC.Collect(); 
            System.GC.WaitForPendingFinalizers(); 
            string file = files[i];
            files.Remove(file);
            File.Delete(file);
        }
    }

    private void startButtonAction()
    {
        SceneManager.LoadScene(1);
    }

    private void savedButtonAction()
    {
        mainCanvas.enabled = false;
        savedCanvas.enabled = true;
        setUpSaveList();
    }

    private void setUpSaveList()
    {
        string[] files_path;
        try
        {
            files_path = getSaveFiles(SaveSystem.getSaveDirectory());
        }
        catch
        {
            Debug.Log("Save Directory Not Found");
            return;
        }

        Dictionary<DateTime, string> files_and_dates = new Dictionary<DateTime, string>();
        DateTime[] dates = new DateTime[files_path.Length];
        for (int i = 0; i < files_path.Length; i++)
        {
            dates[i] = File.GetCreationTime(files_path[i]);
            try
            {
                files_and_dates.Add(File.GetCreationTime(files_path[i]), files_path[i]);
            }
            catch
            {
                //files_and_dates.Add(DateTime.UtcNow, "empty");
            }
        }
        //Debug.Log("date: " + dates[0]);
        Array.Sort(dates);
        Array.Reverse(dates);
        //Debug.Log("date2:" + dates[0]);
        
        for (int i = 0; i < files_path.Length; i++)
        {
            files_path[i] = files_and_dates[dates[i]];
        }

        saveFileContent.GetComponent<RectTransform>().sizeDelta = new Vector2(
            saveFileContent.GetComponent<RectTransform>().sizeDelta.x,
            saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y * files_path.Length);
        
        for (int i = 0; i < files_path.Length; i++)
        {

            StreamReader sr = new StreamReader(files_path[i]);
            string full_path = files_path[i];
            Debug.Log(sr.ReadToEnd());

            GameObject new_file_option = Instantiate(saveFilePrefab);
            new_file_option.transform.parent = saveFileContent.transform;

            string path = files_path[i].Replace(SaveSystem.getSaveDirectory() + @"\", "");
            
            new_file_option.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = path + "\n" + File.GetCreationTime(files_path[i]);

            if (files_path.Length > 1 && files_path.Length % 2 == 0)
            {
                new_file_option.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, saveFileContent.GetComponent<RectTransform>().anchorMax.y +
                                   (saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y * (files_path.Length / 2) -
                                    (saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y / 2)) -
                                   saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y * i);
            } 
            else if (files_path.Length > 1 && files_path.Length % 2 == 1)
            {
                new_file_option.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, saveFileContent.GetComponent<RectTransform>().anchorMax.y +
                                   (saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y * (files_path.Length / 2) -
                                   saveFilePrefab.GetComponent<RectTransform>().sizeDelta.y * i));
            }
            else if (files_path.Length == 1)
            {
                new_file_option.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, saveFileContent.GetComponent<RectTransform>().anchorMax.y);
            }

            //new_file_option.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(loadSimulation);
            new_file_option.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                loadSimulation(full_path));

            Debug.Log("file name: "+ files_path[i].Replace(SaveSystem.getSaveDirectory() + @"\", ""));
        }
    }

    private void loadSimulation(string path)
    {
        List<DeserializedBodyData> dsbd = SaveSystem.deserializeData(path);

        if (dsbd == null)
        {
            return;
        }

        PhysicsSynchronizer.bodiesToCreate = dsbd;
        Debug.Log(path);
        startButtonAction();
    }

    private void deleteSaveList()
    {
        for (int i = 0; i < saveFileContent.transform.childCount; i++)
        {
            Destroy(saveFileContent.transform.GetChild(i).gameObject);
        }
    }

    private string []  getSaveFiles(string path)
    {

        try
        {
            Directory.Exists(path);
        }
        catch
        {
            Debug.Log("Save Directory Not Found");
            return null;
        }

        return  Directory.GetFiles(path);
    }

    private void exitButtonAction()
    {
        Application.Quit();
    }
    
    private void backButtonAction()
    {
        mainCanvas.enabled = true;
        savedCanvas.enabled = false;
        deleteSaveList();
    }
}
