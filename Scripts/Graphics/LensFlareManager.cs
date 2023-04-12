using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LensFlareManager : MonoBehaviour
{
    public Color[] colors = BodyData.colors;
    public  LensFlareDataSRP[] lensFlares;

    public LensFlareComponentSRP lensFlareComponent;

    private float scale;
    private float occlusion_distance;
    private float attenuation_distance;

    public const int divisor_constant = 2;
    // Start is called before the first frame update
    void Start()
    {
        Color color = this.GetComponent<BodyData>().star_color;

        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i] == color)
            {
                lensFlareComponent.lensFlareData = lensFlares[i];
            }
        }

        scale = lensFlareComponent.maxAttenuationScale;
        occlusion_distance = lensFlareComponent.occlusionRadius;
        attenuation_distance = lensFlareComponent.maxAttenuationDistance;
    }

    // Update is called once per frame
    void Update()
    {
        lensFlareComponent.maxAttenuationScale = scale * (this.GetComponent<BodyData>().radius / divisor_constant);
        lensFlareComponent.occlusionRadius = occlusion_distance * (this.GetComponent<BodyData>().radius / divisor_constant);
        lensFlareComponent.maxAttenuationDistance = attenuation_distance * (this.GetComponent<BodyData>().radius / divisor_constant);
    }
}
