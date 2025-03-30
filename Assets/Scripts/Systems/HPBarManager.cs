using UnityEngine;
using System.Collections.Generic;

public class HPBarManager : MonoBehaviour
{
    public static HPBarManager instance;

    [SerializeField, ReadOnly] private List<HPBar> allyHPBars = new List<HPBar>();
    [SerializeField, ReadOnly] private List<HPBar> enemyHPBars = new List<HPBar>();
    [SerializeField, ReadOnly] private HashSet<HPBar> hoveredHPBars = new HashSet<HPBar>();

    private bool showAlliesHP = false;
    private bool showEnemiesHP = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void RegisterHPBar(HPBar hpBar, bool isAlly)
    {
        if(isAlly)
        {
            allyHPBars.Add(hpBar);
        }
        else
        {
            enemyHPBars.Add(hpBar);
        }
    }

    public void SetHoveredState(HPBar hpBar, bool isHovered)
    {
        if(isHovered)
        {
            hoveredHPBars.Add(hpBar);
        }
        else
        {
            hoveredHPBars.Remove(hpBar);
        }
    }

    public void UnregisterHPBar(HPBar hpBar)
    {
        allyHPBars.Remove(hpBar);
        enemyHPBars.Remove(hpBar);
        hoveredHPBars.Remove(hpBar);
    }

    private void FixedUpdate()
    {
        showAlliesHP = Input.GetKey(KeyCode.Q);
        showEnemiesHP = Input.GetKey(KeyCode.E);

        foreach (HPBar hpBar in allyHPBars)
        {
            if (hpBar != null && !hoveredHPBars.Contains(hpBar))
            {
                if(showAlliesHP)
                {
                    hpBar.Show();
                }
                else
                {
                    hpBar.Hide();
                }
            }
        }

        foreach (HPBar hpBar in enemyHPBars)
        {
            if (hpBar != null && !hoveredHPBars.Contains(hpBar))
            {
                if (showEnemiesHP)
                {
                    hpBar.Show();
                }
                else
                {
                    hpBar.Hide();
                }
            }
        }
    }

}
