using UnityEngine;

public class CubeFollower : MonoBehaviour
{
    public Transform ball;  // La balle
    public Transform plane; // Le plan
    public float heightAboveBall = 3f; // Hauteur du cube au-dessus de la balle

    void Update()
    {
        // Récupérer la position de la balle
        Vector3 ballPosition = ball.position;

        // Ajuster la position du cube en fonction de la hauteur spécifiée
        Vector3 newPosition = ballPosition + Vector3.up * heightAboveBall;

        // Appliquer la rotation du plan au cube
        transform.position = newPosition;
        transform.rotation = plane.rotation;
    }
}