using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class TimeScaleChanger : MonoBehaviour
{
    private float inputf;
    private TMP_InputField input;
    private void Start()
    {
        input = this.gameObject.GetComponent<TMP_InputField>();
    }


    // Update is called once per frame
    void Update()
    {
        inputf = float.Parse(input.text);
        Time.timeScale = inputf;
    }
}
