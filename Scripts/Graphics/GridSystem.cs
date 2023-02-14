using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    //private float length = 100000;
    //private int points = 500;//must be even!
    //private float line_width = 2;

    private Mesh mesh;


    public Mesh createMesh(float length, int points, float line_width)
    {
        float square_radius = length / 2;
        float point_half = points / 2;

        //create grid references
        List<Vector3> vertex_list = createGridReference(point_half, square_radius, length, points);
        
        //create verticies
        List<Vector3> polygon_verticies = createVerticies(vertex_list, line_width);

        mesh = new Mesh();
        Vector3[] polygon_verticies_array = polygon_verticies.ToArray();
        mesh.vertices = polygon_verticies_array;
        
        //create polygons
        List<Vector3> polygons = createPolys(polygon_verticies);
        

        List<int> int_polys = new List<int>();
        foreach (var v3 in polygons)
        {
            int_polys.Add((int)v3.x);
            int_polys.Add((int)v3.y);
            int_polys.Add((int)v3.z);
        }

        int[] a = int_polys.ToArray();
        
        //create mesh
        mesh.triangles = int_polys.ToArray();
        return mesh;
    }

    private List<Vector3> createPolys(List<Vector3> polygon_verticies)
    {
        List<Vector3> polygons = new List<Vector3>();
        int k = 0;
        int even_odd = 0;
        //create polygons
        while (k < polygon_verticies.Count)
        {
            if (polygon_verticies.Count - k != 8)
            {
                switch (even_odd)
                {
                    case 0:
                        polygons.Add(new Vector3(k + 0, k + 3, k + 1));
                        polygons.Add(new Vector3(k + 1, k + 3, k + 2));

                        polygons.Add(new Vector3(k + 4, k + 7, k + 5));
                        polygons.Add(new Vector3(k + 5, k + 7, k + 6));
                        even_odd = 1;
                        break;

                    case 1:
                        polygons.Add(new Vector3(k + 0, k + 1, k + 3));
                        polygons.Add(new Vector3(k + 2, k + 3, k + 1));

                        polygons.Add(new Vector3(k + 4, k + 5, k + 7));
                        polygons.Add(new Vector3(k + 5, k + 6, k + 7));
                        even_odd = 0;
                        break;

                    default:
                        Debug.LogError("how did you mess this up?");
                        break;
                }
            }
            else 
            {
                polygons.Add(new Vector3(k + 0, k + 3, k + 1));
                polygons.Add(new Vector3(k + 1, k + 3, k + 2));
                    
                polygons.Add(new Vector3(k + 4, k + 5, k + 7));
                polygons.Add(new Vector3(k + 5, k + 6, k + 7));
            }
            



            k += 8;
        }

        return polygons;
    }

    private List<Vector3> createVerticies(List<Vector3> vertex_list, float line_width)
    {
        List<Vector3> polygon_verticies = new List<Vector3>();
         int j = 0;
        while (j < vertex_list.Count)
        {
            if (vertex_list.Count - j == 4)
            {
                //create counter clockwise
                //num0
                polygon_verticies.Add(new Vector3(vertex_list[j].x, 0, vertex_list[j].z + (line_width/2)));
                //num1
                polygon_verticies.Add(new Vector3(vertex_list[j+1].x, 0, vertex_list[j+1].z + (line_width/2)));
                //num2
                polygon_verticies.Add(new Vector3(vertex_list[j+1].x , 0, vertex_list[j+1].z - (line_width/2)));
                //num3
                polygon_verticies.Add(new Vector3(vertex_list[j].x , 0, vertex_list[j].z - (line_width/2)));
                
                //num0
                polygon_verticies.Add(new Vector3(vertex_list[j+2].x + (line_width/2), 0, vertex_list[j+2].z ));
                //num1
                polygon_verticies.Add(new Vector3(vertex_list[j+3].x + (line_width/2), 0, vertex_list[j+3].z));
                //num2
                polygon_verticies.Add(new Vector3(vertex_list[j+3].x - (line_width/2), 0, vertex_list[j+3].z ));
                //num3
                polygon_verticies.Add(new Vector3(vertex_list[j+2].x - (line_width/2), 0, vertex_list[j+2].z ));
            }
            else
            {
                //create counter clockwise z
                //the 4 z blocks
                //num0
                polygon_verticies.Add(new Vector3(vertex_list[j].x, 0, vertex_list[j].z + (line_width/2)));
                //num1
                polygon_verticies.Add(new Vector3(vertex_list[j+1].x, 0, vertex_list[j+1].z + (line_width/2)));
                //num2
                polygon_verticies.Add(new Vector3(vertex_list[j+1].x , 0, vertex_list[j+1].z - (line_width/2)));
                //num3
                polygon_verticies.Add(new Vector3(vertex_list[j].x , 0, vertex_list[j+1].z - (line_width/2)));
                //num4
                polygon_verticies.Add(new Vector3(vertex_list[j+2].x, 0, vertex_list[j+2].z+ (line_width/2)));
                //num5
                polygon_verticies.Add(new Vector3(vertex_list[j+3].x, 0, vertex_list[j+3].z+ (line_width/2)));
                //num6
                polygon_verticies.Add(new Vector3(vertex_list[j+3].x , 0, vertex_list[j+3].z- (line_width/2)));
                //num7
                polygon_verticies.Add(new Vector3(vertex_list[j+2].x , 0, vertex_list[j+2].z- (line_width/2)));
                
                //create counter clockwise x
                //the 4 x blocks
                //num0
                polygon_verticies.Add(new Vector3(vertex_list[j+4].x+ (line_width/2), 0, vertex_list[j+4].z ));
                //num1
                polygon_verticies.Add(new Vector3(vertex_list[j+5].x+ (line_width/2), 0, vertex_list[j+5].z ));
                //num2
                polygon_verticies.Add(new Vector3(vertex_list[j+5].x  - (line_width/2), 0, vertex_list[j+5].z));
                //num3
                polygon_verticies.Add(new Vector3(vertex_list[j+4].x  - (line_width/2), 0, vertex_list[j+4].z));
                //num4
                polygon_verticies.Add(new Vector3(vertex_list[j+6].x+ (line_width/2), 0, vertex_list[j+6].z));
                //num5
                polygon_verticies.Add(new Vector3(vertex_list[j+7].x+ (line_width/2), 0, vertex_list[j+7].z));
                //num6
                polygon_verticies.Add(new Vector3(vertex_list[j+7].x  - (line_width/2), 0, vertex_list[j+7].z));
                //num7
                polygon_verticies.Add(new Vector3(vertex_list[j+6].x  - (line_width/2), 0, vertex_list[j+6].z));
            }

            j += 8;
        }

        return polygon_verticies;
    }

    private List<Vector3> createGridReference(float point_half, float square_radius, float length, int points)
    {
        List<Vector3> vertex_list = new List<Vector3>();
        //create reference points
        for (int i = 1; i < point_half+1; i++) {
            if (i == point_half)
            {
                vertex_list.Add(new Vector3(square_radius, 0, (square_radius) - (length * i / points))); // POS POS
                vertex_list.Add(new Vector3(-square_radius, 0, (square_radius) - (length * i / points))); // NEG POS

                vertex_list.Add(new Vector3((square_radius) - (length * i / points), 0, square_radius)); //POS POS
                vertex_list.Add(new Vector3((square_radius) - (length * i / points), 0, -square_radius)); //POS NEG
            }
            else
            {
                //z
                vertex_list.Add(new Vector3(square_radius, 0, (square_radius) - (length * i / points))); // POS POS
                vertex_list.Add(new Vector3(-square_radius, 0, (square_radius) - (length * i / points))); // NEG POS
                vertex_list.Add(new Vector3(square_radius, 0, -1 * ((square_radius) - (length * i / points)))); // POS NEG
                vertex_list.Add(new Vector3(-square_radius, 0, -1 * ((square_radius) - (length * i / points)))); // NEG NEG

                //x
                vertex_list.Add(new Vector3(-1 * ((square_radius) - (length * i / points)), 0, square_radius)); //NEG POS
                vertex_list.Add(new Vector3(-1 * ((square_radius) - (length * i / points)), 0, -square_radius)); //NEG NEG
                vertex_list.Add(new Vector3((square_radius) - (length * i / points), 0, square_radius)); //POS POS
                vertex_list.Add(new Vector3((square_radius) - (length * i / points), 0, -square_radius)); //POS NEG
            }
        }

        return vertex_list;
    }
}
