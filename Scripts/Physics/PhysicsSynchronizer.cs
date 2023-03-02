using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using System.Threading;
using Object = System.Object;
using System.Text.Json;

public class PhysicsSynchronizer : MonoBehaviour
{
    private static List<GameObject> celestial_bodies = new List<GameObject>();

    private static int collisionAlgorithm = 0;
    private static bool gravityOn = true;
    private static int num_threads;

    private static List<Thread> threads = new List<Thread>();
    void Start()
    {
        
        
        if(celestial_bodies.Count > 0)
            CicrularVelocityTool.findMostMassive(celestial_bodies);
        
        foreach (GameObject g in celestial_bodies)
        {
            g.GetComponent<CicrularVelocityTool>().setInitializationVelocity();
        }
    }


    private void FixedUpdate()
    {
        updateMomentuems();
        
        if(gravityOn)
            gravityCalculations();

        switch (collisionAlgorithm)
        {
            case 0:
                mergerCalculations();
                break;
            case 1:
                contactForceCalculations();
                break;
                
            default:
                Debug.LogError("NO VALID TYPE GIVEN FOR COLLISION: " + collisionAlgorithm);
                break;
        }
        //mergerCalculations();//merging
        applyCalculations();

        updateMomentuems();
        float a = 0;
        foreach (GameObject body in celestial_bodies) 
        {
            a += body.GetComponent<BodyData>().momentuem.magnitude;
            //Debug.Log(body.GetComponent<BodyData>().id);
        }
        //Debug.Log(a);
    }

    public static List<GameObject> getBodyList()
    {
        return celestial_bodies;
    }


    void mergerCalculations() 
    {
        CollisionScript.nonElasticCollision(celestial_bodies);
    }

    void applyCalculations()
    {
        foreach (GameObject body in celestial_bodies) 
        {
            BodyData properties = body.GetComponent<BodyData>();
            body.transform.position += properties.velocity;
            body.GetComponent<BodyData>().position_read = body.transform.position;
            //Debug.Log(properties.velocity);
        }
        updateMomentuems();

    }

    void gravityCalculations() {

        List<BodyData> g_list = PlanetScript.gravitational_bodies;


        int thread_body_limit = 1;
        int thread_num = 0;

        thread_num = g_list.Count / thread_body_limit;
        Debug.Log(threads.Count + " threads");
        Debug.Log(g_list.Count + " Bodies");
        
        
        
        if (thread_num >= 1)
        {
            List<List<BodyData>> split_lists = new List<List<BodyData>>();
            for (int i = 0; i <= thread_num-num_threads; i++)
            {
                List<BodyData> temp_list = new List<BodyData>();
                if ( (thread_num * thread_body_limit) - (i * thread_body_limit) < thread_body_limit)
                {
                    temp_list.AddRange(g_list.GetRange((i * thread_body_limit), (g_list.Count)-(i * thread_body_limit)));
                    split_lists.Add(temp_list);
                    break;
                }

                temp_list.AddRange(g_list.GetRange(i*thread_body_limit, thread_body_limit));
                split_lists.Add(temp_list);
            }
            
            for (int i = 0; i < thread_num - threads.Count; i++)
            {
                threads.Add(new Thread(parallelCalculations));
            }
            
            Debug.Log((split_lists.Count == threads.Count));

            for(int i = 0; i < thread_num; i++)
            {
                ThreadPool.QueueUserWorkItem(parallelCalculations, new List<List<BodyData>> {split_lists[i], g_list});
            }
            
            for(int i = 0; i < thread_num; i++)
            {
                if (threads[i].IsAlive)
                    threads[i].Join();
            }

        }
        else
        {
            foreach (BodyData body in g_list) 
            {
                body.impulse = GravityScript.applyAllGravity(body, g_list);
            }
        }
        
        
        foreach (BodyData body in g_list)
        {
            //Debug.Log(body.velocity);
            body.velocity += (body.impulse / body.mass) * Time.fixedDeltaTime ;//something broke here
            //Debug.Log(body.impulse + " / " + body.mass + " = " + (body.impulse / body.mass));
            //Debug.Log(body.velocity);
            body.impulse = new Vector3(0, 0, 0);
            
            
            
        }

        updateMomentuems();

    }

    public static void parallelCalculations(Object data)
    {
        List<List<BodyData>> data_obj = (List<List<BodyData>>)data;
        List<BodyData> calculation_list = data_obj[0];
        List<BodyData> big_list = data_obj[1];
        //Debug.Log (calculation_list.Count + " : " + big_list.Count);
        
        foreach (BodyData body in calculation_list) 
        {
            body.impulse = GravityScript.applyAllGravity(body, big_list);
        }
        //Debug.Log("thread used and done: " + Thread.CurrentThread.Name);
        //calculationsFinishedEventHandler.CreateDelegate(This, calculationsFinished);
    }

    void contactForceCalculations() 
    { 
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();
            body_data.impulse = new Vector3(0, 0, 0);
            body_data.impulse = CollisionScript.calculateAppliedForce(body.GetComponent<PlanetScript>());
        }
        
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();
            body_data.velocity += (body_data.impulse / body_data.mass) * Time.fixedDeltaTime ;
            body_data.impulse = new Vector3(0, 0, 0);
        }
        updateMomentuems();

    }

    void updateMomentuems() 
    {
        foreach (GameObject body in celestial_bodies)
        {
            BodyData body_data = body.gameObject.GetComponent<BodyData>();

            body_data.momentuem = (body_data.velocity * body_data.mass) / Time.fixedDeltaTime ;
        }
    }

    public static void addCelestialBody(GameObject body)
    {
        celestial_bodies.Add(body);
        //Debug.Log(celestial_bodies.Contains(body));
        CicrularVelocityTool.updateList(celestial_bodies);
    }

    public static void removeCelestialBody(GameObject body)
    {
        celestial_bodies.Remove(body);        
    }


    public void setCollisionType()
    {
        collisionAlgorithm = this.transform.GetChild(0).GetChild(0).GetChild(7).GetChild(1).GetComponent<TMP_Dropdown>().value;
        //Debug.Log(collisionAlgorithm);
    }

    public void setElasticity()
    {
        float e = this.transform.GetChild(0).GetChild(0).GetChild(8).GetChild(0).GetComponent<Slider>().value;
        this.transform.GetChild(0).GetChild(0).GetChild(8).GetChild(2).GetComponent<TMP_Text>().text = Math.Round(e*100)/100 + "/1";
        CollisionScript.setElasticity((float)Math.Round(e*100)/100);
    }

    public void setGravity()
    {
        gravityOn = !gravityOn;
    }
}
