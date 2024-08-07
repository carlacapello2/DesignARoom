using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwap : MonoBehaviour
{
    public GameObject image1; // Assegna questo dall'Editor
    public GameObject image2; // Assegna questo dall'Editor

    void Start()
    {
        StartCoroutine(SwapImageAfterDelay(3f)); // Avvia la coroutine con un ritardo di 3 secondi
    }

    IEnumerator SwapImageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Attende per il tempo specificato

        image1.SetActive(false); // Disattiva la prima immagine
        image2.SetActive(true);  // Attiva la seconda immagine
    }
}
