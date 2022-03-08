using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float gameWidth = 480f;
    public float gameHeight = 640f;

    void Start()
    {
        // �A�X�y�N�g��(1����=LANDSCAPE�A1=�����`�A1���傫��=PORTRAIT
        float gameAspect = gameHeight / gameWidth;
        float screenAspect = (float)Screen.height / (float)Screen.width;
        float fixRatio;
        Camera cam = GetComponent<Camera>();

        if (gameAspect > screenAspect)
        {
            // �X�N���[���̍��E������
            fixRatio = screenAspect / gameAspect;
            cam.rect = new Rect(0.5f - fixRatio / 2f, 0f, fixRatio, 1f);
        }
        else
        {
            // �X�N���[���̏㉺������
            fixRatio = gameAspect / screenAspect;
            cam.rect = new Rect(0f, 0.5f - fixRatio / 2f, 1f, fixRatio);
        }
    }
}