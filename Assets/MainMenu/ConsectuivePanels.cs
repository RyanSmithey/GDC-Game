using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsectuivePanels : MonoBehaviour
{
    [SerializeField] private List<GameObject> Panels = null;
    [SerializeField] private Button Back = null;
    [SerializeField] private Button Next = null;

    private int CurrentPanel = 0;

    private void Start()
    {
        Next.onClick.AddListener(NextPanel);
        Back.onClick.AddListener(PreviousPanel);
    }

    private void OnEnable()
    {
        CurrentPanel = 0;

        ResetPanels();
        Panels[CurrentPanel].SetActive(true);
        
        Back.gameObject.SetActive(false);
        if (Panels.Count > 0)
        {
            Next.gameObject.SetActive(true);
        }
    }


    private void ResetPanels()
    {
        for (int i = 0;i < Panels.Count; i++)
        {
            Panels[i].SetActive(false);
        }
    }

    private void NextPanel()
    {
        Back.gameObject.SetActive(true);

        Panels[CurrentPanel].SetActive(false);
        CurrentPanel += 1;
        Panels[CurrentPanel].SetActive(true);
        if (CurrentPanel == Panels.Count - 1)
        {
            Next.gameObject.SetActive(false);
        }
    }
    private void PreviousPanel()
    {
        Next.gameObject.SetActive(true);

        Panels[CurrentPanel].SetActive(false);
        CurrentPanel -= 1;
        Panels[CurrentPanel].SetActive(true);
        if (CurrentPanel == 0)
        {
            Back.gameObject.SetActive(false);
        }
    }
}
