using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ParallaxLayer2D : MonoBehaviour
{
    [Header("Refs")]
    public Camera targetCamera;
    [Tooltip("Child z SAMYM SpriteRendererem – pojedynczy segment t³a.")]
    public Transform segmentPrefab;

    [Header("Parallax")]
    [Range(0f, 1f)] public float parallaxFactor = 0.25f; // 0 = „bardzo daleko”, 1 = bez paralaksy

    [Header("Looping")]
    public bool loopX = true;
    [Tooltip("Jeœli 0 – szerokoœæ segmentu wyliczona z SpriteRenderer.bounds.size.x (w œwiecie).")]
    public float manualTileWidth = 0f;
    [Tooltip("Ile kafli „buforu” poza ekranem – 1 = to, o co prosi³eœ.")]
    public int marginTiles = 1;

    [Header("Anchoring")]
    public bool reanchorOnEnable = true;
    public int reanchorDelayFrames = 2;


    float startCamX;
    float startPosX;
    float tileWidthWorld;
    float halfTileWorld;

    float viewWidthWorld;

    readonly List<Transform> _segments = new();

    void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;

        if (!segmentPrefab)
        {
            if (transform.childCount > 0) segmentPrefab = transform.GetChild(0);
        }
        if (!segmentPrefab)
        {
            Debug.LogError($"[{name}] ParallaxLayer2D: brak segmentPrefab (child z SpriteRendererem).");
            enabled = false; return;
        }


        if (segmentPrefab.localPosition.sqrMagnitude > 0.0001f)
        {

            Vector3 offLocal = segmentPrefab.localPosition;
            Vector3 offWorld = Vector3.Scale(offLocal, transform.lossyScale);
            transform.position += new Vector3(offWorld.x, offWorld.y, 0f);
            segmentPrefab.localPosition = new Vector3(0f, 0f, segmentPrefab.localPosition.z);
        }

        if (manualTileWidth > 0f) tileWidthWorld = manualTileWidth;
        else
        {
            var sr = segmentPrefab.GetComponent<SpriteRenderer>();
            tileWidthWorld = (sr && sr.sprite) ? sr.bounds.size.x : 20f;
        }
        halfTileWorld = tileWidthWorld * 0.5f;


        _segments.Clear();
        _segments.Add(segmentPrefab);


        RecomputeViewWidth();
    }

    void OnEnable()
    {
        if (reanchorOnEnable) StartCoroutine(ReanchorDelayed());

        BuildInitialStrip();
    }

    System.Collections.IEnumerator ReanchorDelayed()
    {
        for (int i = 0; i < Mathf.Max(0, reanchorDelayFrames); i++)
            yield return null;
        Reanchor();
    }

    public void Reanchor()
    {
        if (!targetCamera) targetCamera = Camera.main;
        startCamX = targetCamera ? targetCamera.transform.position.x : 0f;
        startPosX = transform.position.x;
    }

    void RecomputeViewWidth()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (targetCamera)
            viewWidthWorld = 2f * targetCamera.orthographicSize * targetCamera.aspect;
        else
            viewWidthWorld = tileWidthWorld * 3f;
    }

    void BuildInitialStrip()
    {
        RecomputeViewWidth();


        int needed = Mathf.CeilToInt(viewWidthWorld / tileWidthWorld) + marginTiles * 2;
        needed = Mathf.Clamp(needed, 3, 50);

        for (int i = _segments.Count - 1; i >= 1; i--)
        {
            if (_segments[i]) Destroy(_segments[i].gameObject);
            _segments.RemoveAt(i);
        }

        for (int i = 1; i < needed; i++)
        {
            var seg = Instantiate(segmentPrefab.gameObject, transform).transform;
            seg.localPosition = Vector3.zero;
            seg.localRotation = segmentPrefab.localRotation;
            seg.localScale = segmentPrefab.localScale;
            _segments.Add(seg);
        }


        float start = -((needed - 1) * 0.5f) * tileWidthWorld;
        for (int i = 0; i < _segments.Count; i++)
        {
            var t = _segments[i];
            t.localPosition = new Vector3(start + i * tileWidthWorld, 0f, t.localPosition.z);
        }
    }

    void LateUpdate()
    {
        if (!targetCamera) return;


        float camX = targetCamera.transform.position.x;
        float camDelta = camX - startCamX;
        float offsetWorld = camDelta * (1f - Mathf.Clamp01(parallaxFactor));

        var p = transform.position;
        p.x = startPosX + offsetWorld;
        transform.position = p;

        if (!loopX) return;


        RecomputeViewWidth();
        float camLeft = camX - viewWidthWorld * 0.5f;
        float camRight = camX + viewWidthWorld * 0.5f;

        float leftThreshold = camLeft - marginTiles * tileWidthWorld;
        float rightThreshold = camRight + marginTiles * tileWidthWorld;

        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        Transform leftMost = null, rightMost = null;

        for (int i = 0; i < _segments.Count; i++)
        {
            var t = _segments[i];
            if (!t) continue;
            float wx = t.position.x;
            if (wx < minX) { minX = wx; leftMost = t; }
            if (wx > maxX) { maxX = wx; rightMost = t; }
        }


        for (int i = 0; i < _segments.Count; i++)
        {
            var t = _segments[i];
            if (!t) continue;

            float wx = t.position.x;


            if (wx + halfTileWorld < leftThreshold)
            {
                float target = (rightMost ? rightMost.position.x : maxX) + tileWidthWorld;
                Vector3 pos = t.position; pos.x = target; t.position = pos;


                rightMost = t; maxX = target;
                break;
            }
            else if (wx - halfTileWorld > rightThreshold)
            {
                float target = (leftMost ? leftMost.position.x : minX) - tileWidthWorld;
                Vector3 pos = t.position; pos.x = target; t.position = pos;

                
                leftMost = t; 
                minX = target;
                break;
            }
        }
    }
}
