using UnityEngine;
using System.Collections;
using TMPro;

public class Hole : MonoBehaviour
{
    public TextMeshProUGUI winText;   // Texte pour afficher le message de victoire
    public TextMeshProUGUI loseText;   // Texte pour afficher le message de défaite
    public TextMeshProUGUI chronoText; // Texte pour afficher le chronomètre
    public float chrono;  // Durée du chronomètre
    public GameObject ball;   // Référence à la balle
    private float fallSpeed = 6f;  // Vitesse à laquelle la balle descend dans le trou
    private bool isGameOver = false;   // Variable pour savoir si la partie est terminée
    private float holesize = 0.5f; // Taille du trou

    void Start()
    {
        winText.gameObject.SetActive(false); // Cacher le texte de victoire au début
        loseText.gameObject.SetActive(false); // Cacher le texte de défaite au début
        
        // On récupère le diamètre du trou
        float holeScaleX = transform.localScale.x;
        // Set the position of the hole to (5, -5)
        transform.position = new Vector3(5-holeScaleX*8, transform.position.y, -5+holeScaleX*8);
    }

    void Update()
    {
        // Si la partie est terminée, on return pour ne pas incrémenter le chronomètre
        if (isGameOver)
        return;

        // Réduire le chronomètre
        chrono -= Time.deltaTime;
        
        // Si la valeur du chronomètre est inférieure à 0, on affiche le texte de défaite et on désactive la balle 
        if (chrono < 0)
        {
            chrono = 0;
            loseText.gameObject.SetActive(true);
            ball.SetActive(false);
            isGameOver = true;
        }

        // Minutes
        int minutes = Mathf.FloorToInt(chrono/60);
        // Secondes
        int seconds = Mathf.FloorToInt(chrono - minutes * 60);
        // Afficher le chronomètre
        chronoText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Fonction appelée lorsqu'un objet entre en collision avec le trou, en l'occurrence la balle
    private void OnTriggerEnter(Collider other)
    {
        // Récupérer le Rigidbody de la balle
        Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

        // Désactiver les mouvements de la balle pour qu'elle tombe
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.useGravity = false;

        // Lancer une coroutine pour animer la chute (une coroutine dans Unity, c'est une fonction qui peut mettre en pause son exécution, puis reprendre plus tard)
        StartCoroutine(HandleBallFalling(other.transform));
    }

    private IEnumerator HandleBallFalling(Transform ballTransform)
    {
        float startTime = Time.time; // Start the timer
        float maxDuration = 0.5f; // Maximum duration of the animation

        // Animer la descente de la balle dans le trou
        while (Time.time - startTime < maxDuration)
        {
            ballTransform.position = Vector3.Lerp(
                ballTransform.position,
                new Vector3(transform.position.x, transform.position.y - holesize, transform.position.z),
                Time.deltaTime * fallSpeed
            );
            yield return null; // Attendre la prochaine frame
        }

        //Réactiver la gravité de la balle
        ballTransform.GetComponent<Rigidbody>().useGravity = true;

        // Afficher le texte de victoire
        isGameOver = true;
        winText.gameObject.SetActive(true);
    }
}
