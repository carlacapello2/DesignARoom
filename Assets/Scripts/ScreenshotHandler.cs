using System.Collections;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private bool isProcessing = false;

    public void TakeScreenshot()
    {
        if (!isProcessing)
        {
            StartCoroutine(TakeScreenshotAndSave());
        }
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Salva l'immagine nella galleria
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "MyGallery", "Screenshot.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);

        // Elimina la texture per liberare memoria
        Destroy(ss);

        isProcessing = false;
    }
}
