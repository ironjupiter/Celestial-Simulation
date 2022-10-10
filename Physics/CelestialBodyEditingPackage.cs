using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyEditingPackage : MonoBehaviour
{
    public GameObject edited_object;
    public float mass;
    public Vector3 momentuem;
    public Vector3 position;
    
    public CelestialBodyEditingPackage(float system_total_mass,float radius,Vector3 momentuemx,Vector3 center_of_mass, GameObject gb)
    {
        mass = system_total_mass;
        momentuem = momentuemx;
        position = center_of_mass;
        edited_object = gb;

    }
}
