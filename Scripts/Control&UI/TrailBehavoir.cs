using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailBehavoir : MonoBehaviour
{
    GameObject TO;
    public GameObject trailPreset;
    public GameObject trailPresetList;

    float timer = 0;
    float time = .05f;

    // Start is called before the first frame update
    void Start()
    {
        TO = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;
        //Debug.Log(timer);

        if (timer >= time) 
        {
            timer = 0;

            GameObject trail = Instantiate(trailPreset);

            trail.transform.position = TO.transform.position;

            trail.transform.SetParent(trailPresetList.transform);
        }
    }
}
