using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitter : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        var cam = Camera.main;

        transform.localScale = Vector3.one;

        var size = sr.bounds.size; 
        float spriteW = size.x;
        float spriteH = size.y;

        float camH = cam.orthographicSize * 2f;
        float camW = camH * cam.aspect;

        float s = Mathf.Max(camW / spriteW, camH / spriteH);
        transform.localScale = new Vector3(s, s, 1f);

        transform.position = new Vector3(0f, 0f, 1f);
        sr.sortingLayerName = "Background"; 
        sr.sortingOrder = 0;
    }
}
