using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class UIButtonSfxBinder : MonoBehaviour
{
    public static UIButtonSfxBinder I { get; private set; }

    private readonly HashSet<Button> bound = new HashSet<Button>();
    [SerializeField] private float rescanInterval = 0.5f;
    private float t;

    private void Awake()
    {
        if(I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        BindAllButtonsInAllScenes();
    }

    private void Update()
    {
        t += Time.unscaledDeltaTime;
        if(t >= rescanInterval)
        {
            t = 0f;
            BindAllButtonsInAllScenes();
        }
    }

    private void BindAllButtonsInAllScenes()
    {
        var allButtons = Resources.FindObjectsOfTypeAll<Button>().Where(b => b.gameObject.activeInHierarchy && b.enabled);

        foreach(var b in allButtons)
        {
            if (bound.Contains(b)) continue;
            b.onClick.AddListener(PlayClick);
            bound.Add(b);
        }
    }

    private void PlayClick()
    {
        if (SfxManager.I != null)
            SfxManager.I.PlayUiClick();
    }
}
