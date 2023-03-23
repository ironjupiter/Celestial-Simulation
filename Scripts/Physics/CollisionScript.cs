using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    
    private static float elasticity = 1f;
    
    //note this only works for planets in this version
    public static Vector3 calculateAppliedForce(PlanetScript ps)
    {

        return billiardBallCollisions(ps);

    }

    public static void nonElasticCollision(List <GameObject> celestial_bodies) 
    {
        
        List<CelestialBodyEditingPackage> changed_object_list = new List<CelestialBodyEditingPackage>();
        List<GameObject> objects_to_destroy = new List<GameObject>();
        foreach (GameObject gb in celestial_bodies)
        {
            if (gb.GetComponent<PlanetScript>().to_destroy == false && !(gb.GetComponent<PlanetScript>().touching_bodies.Count == 0))
            {
                BodyData body = gb.GetComponent<BodyData>();
                PlanetScript ps = gb.GetComponent<PlanetScript>();
                float system_total_mass = body.mass;
                Vector3 momentuem = body.momentuem;
                float radius = 0;
                GameObject copy = gb.GetComponent<PlanetScript>().prefab_copy;
                Vector3 center_of_mass = Vector3.zero;
                foreach (GameObject cv in ps.touching_bodies)
                {
                    if (cv != null && cv.TryGetComponent(out BodyData body_data) == true)
                    {
                       
                        if (body_data.mass <= body.mass)
                        {
                            system_total_mass += body_data.mass;
                            momentuem += body_data.momentuem;
                            radius += body_data.radius;
                            cv.GetComponent<PlanetScript>().to_destroy = true;
                            objects_to_destroy.Add(cv);
                        }
                    }

                    
                }

                center_of_mass += (body.mass * gb.transform.position)/ system_total_mass;
                foreach (GameObject cv in ps.touching_bodies) 
                {
                    if (cv != null && cv.TryGetComponent(out BodyData trash) == true)
                    {
                        BodyData body_data = cv.GetComponent<BodyData>();
                        center_of_mass += (body_data.mass*cv.transform.position)/system_total_mass;
                    }
                    
                }
                
              
                changed_object_list.Add(new CelestialBodyEditingPackage(system_total_mass, radius, momentuem, center_of_mass, gb));

            }
        }

        for (int i = 0; i < objects_to_destroy.Count;)
        {
            GameObject g = objects_to_destroy[0];
            objects_to_destroy.Remove(g);
            if (g.transform.childCount != 0)
            {
                g.transform.GetChild(0).SetParent(g.transform.parent);
            }
            PlanetScript.destroyPlanet(g);
        }

        foreach (CelestialBodyEditingPackage pdp in changed_object_list)
        {
            PlanetScript.editPlanet(pdp.edited_object, pdp.mass, pdp.momentuem, pdp.position);
        }

        
    }


    static Vector3 billiardBallCollisions(PlanetScript ps) {
        float mass_ratio;
        Vector3 Force = new Vector3(0, 0, 0);
        BodyData A1 = ps.body_data;

        //go through touching bodies script to find anything that needs to be changed
        foreach (GameObject cv in ps.touching_bodies)
        {
            //find the collided w/ object
            BodyData A2 = cv.GetComponent<BodyData>();

            //M > m ||| find mass relationship to determine how much of p is turned into F, and related to that how much F is given to A1
            if (A1.mass > A2.mass)
            {
                //if A1 is more than A2 only p * m/M is sent back as third related force, all of A2s p is taken in the collision
                mass_ratio = A2.mass / A1.mass;
                Force = ((calculateAppliableForce(A1, A2)));
                Force = Force - (mass_ratio * ((calculateThirdApplication(A1, A2))));
                Force = 1 * angleAppliedForce(A1, A2, Force);
            }
            else if (A1.mass < A2.mass)
            {
                //if A1 is less than A2 all p is sent back as third related force, only p*m/M taken in the collision
                mass_ratio = A1.mass / A2.mass;
                Force = (mass_ratio * (calculateAppliableForce(A1, A2)));
                Force = Force - (((calculateThirdApplication(A1, A2))));
                Force = 1 * angleAppliedForce(A1, A2, Force);
            }
            else if (A1.mass == A2.mass)
            {
                //both are equal so nothing matters in this case
                Force = ((calculateAppliableForce(A1, A2))) * elasticity; // do * <float> for inelastic collision
                Force = Force - (((calculateThirdApplication(A1, A2))));
                Force = angleAppliedForce(A1, A2, Force);

            }
        }


        return Force;
    }

    static Vector3 angleAppliedForce(BodyData A1, BodyData A2, Vector3 force_vector) 
    {
        Vector3 normal_vector = (A1.transform.position - A2.transform.position);

        normal_vector = normalScaling(normal_vector);
        

        float px = vectorForceProjection(force_vector.x, normal_vector.x);
        float py = vectorForceProjection(force_vector.y, normal_vector.y);
        float pz = vectorForceProjection(force_vector.z, normal_vector.z);

        Vector3 adjusted_force = new Vector3(
            forceVectorAdjustment(px, normal_vector.x),
            forceVectorAdjustment(py, normal_vector.y),
            forceVectorAdjustment(pz, normal_vector.z)
            );

        return adjusted_force;
        
    }

    static Vector3 normalScaling(Vector3 normal) 
    {
        float x = Math.Abs(normal.x);
        float y = Math.Abs(normal.y);
        float z = Math.Abs(normal.z);

        float scale = x + y + z;
        return normal / scale;
    }

    static float forceVectorAdjustment(float p, float v) 
    {
        return v - (p);
    }

    static float vectorForceProjection(float v, float n)
    {
        if (v == 0 || n == 0)
            return 0;

        return ((v * n) / (-1* Math.Abs(n*n))) * n;
    }

    static Vector3 calculateThirdApplication(BodyData object1, BodyData object2)
    {
        float h1;
        float h2;
        float h3;

        //////////////////////////////////////////////
            
        h1 = addToThirdVector(object1.momentuem.x,  object1.transform.position.x, object2.transform.position.x);
        //////////////////////////////////////////////

        h2 = addToThirdVector(object1.momentuem.y,  object1.transform.position.y, object2.transform.position.y);
        //////////////////////////////////////////////

        h3 = addToThirdVector(object1.momentuem.z,  object1.transform.position.z, object2.transform.position.z);

        return new Vector3(h1, h2, h3);
    }

    static float addToThirdVector(float p1, float r1, float r2)
    {
        
        float v = 0;
        if ((p1 > 0 && r1 < r2) || (p1 < 0 && r1 > r2))
        {
            v = p1;
        }
        return v;
    }

    static Vector3 calculateAppliableForce(BodyData object1, BodyData object2)
    {
        float h1;
        float h2;
        float h3;

        //////////////////////////////////////////////

        h1 = addToVector(object1.velocity.x, object2.velocity.x, object1.transform.position.x, object2.transform.position.x, object2.momentuem.x);
        //////////////////////////////////////////////
            
        h2 = addToVector(object1.velocity.y, object2.velocity.y, object1.transform.position.y, object2.transform.position.y, object2.momentuem.y);
        //////////////////////////////////////////////

        h3 = addToVector(object1.velocity.z, object2.velocity.z, object1.transform.position.z, object2.transform.position.z, object2.momentuem.z);

        return new Vector3(h1, h2, h3);
    }

    static float addToVector(float v1, float v2, float r1, float r2, float p2)
    {

        float v = 0;

        if (Math.Abs(v2) > Math.Abs(v1))
        {
            v = p2;
        }
        else if ((v1 > 0 && v2 < 0 && r1 < r2) || (v1 < 0 && v2 > 0 && r1 > r2))
        {
            v = p2;
        }

        return v;
    }

    public static void setElasticity(float x)
    {
        elasticity = x;
    }
}
    

