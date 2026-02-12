using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [Header("SpriteRenderers")]
    [SerializeField] private SpriteRenderer topBg;
    [SerializeField] private SpriteRenderer bottomBg;

    [Header("Bottom area")]
    [Range(0.1f, 0.6f)]
    [SerializeField] private float bottomHeightRatio = 0.28f;

    [Header("Top Fit")]
    [SerializeField] private bool topCoverWidth = false; 
    void OnEnable()
    {
        if (!targetCamera) targetCamera = Camera.main;
        Apply();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!targetCamera) targetCamera = Camera.main;
        Apply();
    }
#endif

    public void Apply()
    {
        if (!targetCamera || !targetCamera.orthographic) return;
        if (!topBg || !topBg.sprite || !bottomBg || !bottomBg.sprite) return;

        float camH = targetCamera.orthographicSize * 2f;
        float camW = camH * targetCamera.aspect;

        float bottomH = camH * bottomHeightRatio;
        float topH = camH - bottomH;

        FitBottomStretchXY(bottomBg, camW, bottomH);

        float camBottomY = targetCamera.transform.position.y - camH * 0.5f;
        float bottomWorldH = bottomBg.sprite.bounds.size.y * bottomBg.transform.localScale.y;

        bottomBg.transform.position = new Vector3(
            targetCamera.transform.position.x,
            camBottomY + bottomWorldH * 0.5f,
            bottomBg.transform.position.z
        );

        FitTopUniform(topBg, camW, topH, topCoverWidth);

        float topWorldH = topBg.sprite.bounds.size.y * topBg.transform.localScale.y;
        float bottomTopY = bottomBg.transform.position.y + bottomWorldH * 0.5f;

        topBg.transform.position = new Vector3(
            targetCamera.transform.position.x,
            bottomTopY + topWorldH * 0.5f,
            topBg.transform.position.z
        );
    }

    
    private void FitBottomStretchXY(SpriteRenderer sr, float targetW, float targetH)
    {
        Vector2 size = sr.sprite.bounds.size;

        float scaleX = targetW / size.x;
        float scaleY = targetH / size.y;

        sr.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private void FitTopUniform(SpriteRenderer sr, float targetW, float targetH, bool cover)
    {
        Vector2 size = sr.sprite.bounds.size;

        float sx = targetW / size.x;
        float sy = targetH / size.y;

        float s = cover ? Mathf.Max(sx, sy) : sy; // cover=false면 높이 기준
        sr.transform.localScale = new Vector3(s, s, 1f);
    }
}
