using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Patterns;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Toggle[] toggles;

    public SavingLoadingData savingLoadingDataScript;

    public bool isPaused = false;
    private bool _firstLoad = false;

    void Start()
    {
        savingLoadingDataScript = GetComponent<SavingLoadingData>();
        for (int i = 0; i < toggles.Length; ++i)
        {
            toggles[i].isOn = false;
            toggles[i].GetComponentInChildren<Text>().text = savingLoadingDataScript.savesQueue.ElementAt(toggles.Length-i-1).name;
        }
    }

    void Update()
    {
        if (!_firstLoad)
        {
            OpenOrCloseWindow();
            _firstLoad = true;
        }
    }

    public void UpdateToggles()
    {
        for (int i = 0; i < toggles.Length; ++i)
        {
            toggles[i].GetComponentInChildren<Text>().text = savingLoadingDataScript.savesQueue.ElementAt(toggles.Length - i - 1).name;
        }
    }

    public void EnableNewToggle()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Toggle currentlySelected = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                if (toggle != currentlySelected)
                {
                    toggle.isOn = false;
                }
            }
        }
    }

    public void OpenOrCloseWindow()
    {
        GetComponent<Image>().enabled = !GetComponent<Image>().enabled;
        isPaused = GetComponent<Image>().enabled;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeInHierarchy);
        }
    }
}
