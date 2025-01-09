using UnityEngine;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class CalibrationScript : MonoBehaviour
{
    Thread receiveThread; // Thread pour recevoir les données UDP
    UdpClient client; // Client UDP
    int port; // Port utilisé pour la communication

    Calibration calibrationData;

    // Start est appelé une seule fois avant la première exécution de Update
    void Start()
    {
        port = 5065; // Port configuré pour correspondre au serveur Python
        InitUDP(); // Initialisation de la communication UDP
    }

    private void InitUDP()
    {
        Debug.Log("Initialisation UDP");

        // Démarrage du thread de réception
        receiveThread = new Thread(new ThreadStart(ReceiveData))
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    // Mise à jour appelée à chaque frame
    void Update()
    {
        if (calibrationData != null)
        {
            UpdateCamera();
        }
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);

        while (true)
        {
            try
            {
                // Réception des données depuis le serveur Python
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                byte[] data = client.Receive(ref anyIP);

                // Conversion des données en chaîne JSON
                string text = Encoding.UTF8.GetString(data);
                Debug.Log("Données reçues : " + text);

                // Désérialisation des données JSON en objet Calibration
                calibrationData = Calibration.CreateFromJSON(text);
            }
            catch (Exception e)
            {
                Debug.LogError("Erreur lors de la réception des données : " + e);
            }
        }
    }

    private void UpdateCamera()
    {
        Vector3 T = new Vector3(
                calibrationData.T[0],
                -calibrationData.T[1],
                calibrationData.T[2]);

        Quaternion R = Quaternion.Euler(
            new Vector3(
                calibrationData.R[0],
                calibrationData.R[1],
                calibrationData.R[2]) * Mathf.Rad2Deg);
        
        Vector3 F = new Vector3(
            calibrationData.F[0],
            -calibrationData.F[1],
            calibrationData.F[2]);

        Vector3 U = new Vector3(
            calibrationData.U[0],
            -calibrationData.U[1],
            calibrationData.U[2]);

        Quaternion rot = Quaternion.LookRotation(F, U);

        Camera.main.transform.position = -(Quaternion.Inverse(rot)*T);
        Camera.main.transform.rotation = Quaternion.Inverse(rot);

        Vector2 fparams = new Vector2(calibrationData.M[0], calibrationData.M[4]);
        Vector2 resolution = new Vector2(calibrationData.M[2]*2, calibrationData.M[5]*2); 
        Camera.main.fieldOfView = 2.0f * Mathf.Atan(0.5f * resolution.y / fparams.y) * Mathf.Rad2Deg;
        Camera.main.aspect = resolution.x / resolution.y;
    }
}


[System.Serializable]
public class Calibration
{
    public int C; // Indice ou identifiant de la caméra
    public float[] M; // Matrice intrinsèque
    public float[] R; // Rotation
    public float[] T; // Translation
    public float[] F; // Forward
    public float[] U; // Up

    public static Calibration CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Calibration>(jsonString);
    }
}
