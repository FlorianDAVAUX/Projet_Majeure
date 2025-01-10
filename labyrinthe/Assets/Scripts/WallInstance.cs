using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class WallInstance : MonoBehaviour
{
    public GameObject plane; 
    void Start()
    {
        // Recupérer le scale du plan
        float scale = plane.transform.localScale.x/2;

        // Récupération du composant WallGenerator
        WallGenerator generator = GetComponent<WallGenerator>();

        // Chemin du fichier JSON
        // string filePath = Path.Combine(Application.dataPath, "Scripts/WallCoordinates.json");
        string filePath = Path.Combine(Application.dataPath, "Scripts/labyrinthe_3.json");

        // Lecture du fichier JSON
        string jsonContent = File.ReadAllText(filePath);
        WallData wallData = JsonUtility.FromJson<WallData>(jsonContent);

        // Épaisseur du mur
        float wallThickness = 0.1f;

        // Murs exterieurs
        List<Vector3> coords = new List<Vector3>
        {
            // Premier mur
            new Vector3(-5, 0, 5 + wallThickness),   
            new Vector3(-5, 0, -5 - wallThickness),

            // Deuxième mur
            new Vector3(-5 - wallThickness, 0, -5),
            new Vector3(5 + wallThickness, 0, -5),

            // Troisième mur
            new Vector3(5, 0, -5 - wallThickness),
            new Vector3(5, 0, 5 + wallThickness),

            // Quatrième mur
            new Vector3(5 + wallThickness, 0, 5),
            new Vector3(-5 - wallThickness, 0, 5)
        };

        // Récupération de la couleur et les coordonnées des murs
        List<string> colors = new List<string>();
        for (int i = 0; i < 4; i++) colors.Add("black");

        foreach (var wall in wallData.walls)
        {
            colors.Add(wall.color);
            foreach (var point in wall.points)
            {
                coords.Add(new Vector3(point.x * scale, point.y * scale, point.z * scale));
            }
        }

        // Génération des murs
        generator.GenerateWallsFromCoordinates(coords.ToArray(), colors.ToArray());
    }
}

[System.Serializable]
public class WallData
{
    public List<Wall> walls; 
}

[System.Serializable]
public class Wall
{
    public List<Point> points; 
    public string color; 
}

[System.Serializable]
public class Point
{
    public float x;
    public float y;
    public float z;
}