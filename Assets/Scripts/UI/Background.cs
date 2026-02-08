using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [Header("SpriteRenderers")]
    [SerializeField] private SpriteRenderer topBg;
    [SerializeField] private SpriteRenderer bottomBg;

    [Header("Bottom area")]
    [Range(0.1f, 0.6f)]
    [SerializeField] private float bottomHeightRatio = 0.28f; // 화면 높이 중 하단이 차지할 비율
    [SerializeField] private bool coverWidth = true; // 가로는 화면을 꽉 채우는 쪽으로

    void OnEnable()
    {
        if (!targetCamera) targetCamera = Camera.main;
        Apply();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying) Apply();
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

        FitSpriteToArea(bottomBg, camW, bottomH, coverWidth);
        float camBottomY = targetCamera.transform.position.y - camH * 0.5f;
        float bottomWorldH = bottomBg.sprite.bounds.size.y * bottomBg.transform.localScale.y;
        bottomBg.transform.position = new Vector3(
            targetCamera.transform.position.x,
            camBottomY + bottomWorldH * 0.5f,
            bottomBg.transform.position.z
        );

        FitSpriteToArea(topBg, camW, topH, coverWidth);

        float topWorldH = topBg.sprite.bounds.size.y * topBg.transform.localScale.y;
        float bottomTopY = bottomBg.transform.position.y + bottomWorldH * 0.5f;

        topBg.transform.position = new Vector3(
            targetCamera.transform.position.x,
            bottomTopY + topWorldH * 0.5f,
            topBg.transform.position.z
        );
    }

    private void FitSpriteToArea(SpriteRenderer sr, float targetW, float targetH, bool coverW)
    {
        Vector2 size = sr.sprite.bounds.size;
        float sx = targetW / size.x;
        float sy = targetH / size.y;

        float s = coverW ? Mathf.Max(sx, sy) : sy; 
        sr.transform.localScale = new Vector3(s, s, 1f);
    }
}
