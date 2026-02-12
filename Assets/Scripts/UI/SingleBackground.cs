using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SingleBackgroundFitter : MonoBehaviour
{
    public enum FitMode { Cover, FitHeight, FitWidth }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private FitMode fitMode = FitMode.Cover; 

    private SpriteRenderer sr;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
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
        if (!sr || !sr.sprite || !targetCamera) return;
        if (!targetCamera.orthographic)
        {
            return;
        }

        float camH = targetCamera.orthographicSize * 2f;
        float camW = camH * targetCamera.aspect;

        Vector2 size = sr.sprite.bounds.size;
        float sx = camW / size.x;
        float sy = camH / size.y;

        float s = fitMode switch
        {
            FitMode.Cover => Mathf.Max(sx, sy), 
            FitMode.FitHeight => sy,            
            FitMode.FitWidth => sx,             
            _ => Mathf.Max(sx, sy)
        };

        transform.localScale = new Vector3(s, s, 1f);

        
        transform.position = new Vector3(
            targetCamera.transform.position.x,
            targetCamera.transform.position.y,
            transform.position.z
        );
    }
}