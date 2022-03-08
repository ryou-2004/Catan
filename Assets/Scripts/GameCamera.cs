using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float gameWidth = 480f;
    public float gameHeight = 640f;

    void Start()
    {
        // アスペクト比(1未満=LANDSCAPE、1=正方形、1より大きい=PORTRAIT
        float gameAspect = gameHeight / gameWidth;
        float screenAspect = (float)Screen.height / (float)Screen.width;
        float fixRatio;
        Camera cam = GetComponent<Camera>();

        if (gameAspect > screenAspect)
        {
            // スクリーンの左右が黒帯
            fixRatio = screenAspect / gameAspect;
            cam.rect = new Rect(0.5f - fixRatio / 2f, 0f, fixRatio, 1f);
        }
        else
        {
            // スクリーンの上下が黒帯
            fixRatio = gameAspect / screenAspect;
            cam.rect = new Rect(0f, 0.5f - fixRatio / 2f, 1f, fixRatio);
        }
    }
}