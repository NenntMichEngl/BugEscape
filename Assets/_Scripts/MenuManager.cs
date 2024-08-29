using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider musicSlider;
    public Animator startGameAnimator;
    public AudioClip menuClip;
    public EventSystem es;
    public GameObject resolutionObject;
    public GameObject audioPrefab;

    public List<Resolution> availableResolutions = new List<Resolution>();

    private int currentResolutionIndex = 0;
    private GameObject lastSelected;

    bool optionsMenu;

    public void toogleMenu()
    {
        optionsMenu = !optionsMenu;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Load volume settings
        if (PlayerPrefs.GetInt("started") == 0)
        {
            PlayerPrefs.SetInt("started", 1);
            PlayerPrefs.SetFloat("volume", 1);
            PlayerPrefs.SetFloat("musicvolume", 1);
        }
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        musicSlider.value = PlayerPrefs.GetFloat("musicvolume");

        // Initialize available resolutions
        InitializeResolutions();

        // Set the default resolution to the current screen resolution
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
        }
        else
        {
            currentResolutionIndex = GetDefaultResolutionIndex();
            PlayerPrefs.SetInt("resolutionIndex", currentResolutionIndex);
        }

        ApplyResolution();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public Button optionsDefault;
    public Button menuDefault;
    void Update()
    {
        if (lastSelected != es.currentSelectedGameObject)
        {
            lastSelected = es.currentSelectedGameObject;
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(menuClip, 0.7f, 1.3f, .5f * PlayerPrefs.GetFloat("volume"), true, false);
        }

        if (es.currentSelectedGameObject == resolutionObject)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                ChangeResolution(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                ChangeResolution(-1);
            }
        }
        if (optionsMenu)
        {
            if (es.currentSelectedGameObject == null)
            {
                Debug.Log("was null");
                optionsDefault.Select();
            }
        }
        else
        {
            if (es.currentSelectedGameObject == null)
            {
                Debug.Log("was null");
                menuDefault.Select();
            }
        }
    }
    public void StartGame()
    {
        StartCoroutine(startCourountine());
    }

    IEnumerator startCourountine()
    {
        startGameAnimator.SetTrigger("open");
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(1);
    }

    public void selectButton(Button buttonToSelect)
    {
        buttonToSelect.Select();
    }

    public void changeVolume(bool music)
    {
        if (music)
        {
            PlayerPrefs.SetFloat("musicvolume", musicSlider.value);
        }
        else
        {
            PlayerPrefs.SetFloat("volume", volumeSlider.value);
        }
    }

    private void InitializeResolutions()
    {
        availableResolutions.Clear();

        // Get all available screen resolutions
        UnityEngine.Resolution[] unityResolutions = Screen.resolutions;

        foreach (var res in unityResolutions)
        {
            availableResolutions.Add(new Resolution(res.width, res.height));
        }
    }

    private int GetDefaultResolutionIndex()
    {
        UnityEngine.Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].width == currentResolution.width &&
                availableResolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0; // Default to the first resolution if not found
    }
    public TMP_Text resolutionText;
    private void ChangeResolution(int direction)
    {
        currentResolutionIndex += direction;

        // Clamp the index to valid range
        if (currentResolutionIndex >= availableResolutions.Count)
        {
            currentResolutionIndex = availableResolutions.Count - 1;
        }
        else if (currentResolutionIndex < 0)
        {
            currentResolutionIndex = 0;
        }
        resolutionText.text = availableResolutions[currentResolutionIndex].width.ToString() + "x" + availableResolutions[currentResolutionIndex].height.ToString();

    }

    public void ApplyResolution()
    {
        int index = currentResolutionIndex;
        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, true); // Set to full screen

        // Update the resolution in PlayerPrefs
        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class Resolution
{
    public int width;
    public int height;

    public Resolution(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}
