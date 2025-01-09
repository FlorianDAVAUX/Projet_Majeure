using UnityEngine;

public class Orientation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    
    // Update est appelé une fois par frame
    void Update()
    {
        Vector3 rotationPoint = transform.position;

        if (Input.GetKey(KeyCode.A)) // Q pour Q sur AZERTY
        {
            transform.RotateAround(rotationPoint, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) // D pour D sur AZERTY
        {
            transform.RotateAround(rotationPoint, Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W)) // W pour Z sur AZERTY
        {
            transform.RotateAround(rotationPoint, Vector3.right, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.RotateAround(rotationPoint, Vector3.right, -rotationSpeed * Time.deltaTime);
        }
    }
}
