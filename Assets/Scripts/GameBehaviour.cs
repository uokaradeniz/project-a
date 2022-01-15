using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameBehaviour : MonoBehaviour
{
    bool godToggle;

    public int score;
    public int highScore;
    public int SA1NeededScore;
    public int SA2NeededScore;
    public int SA3NeededScore;

    public int scoreBySnitch;
    public int scoreByDummy;
    public int scoreByTurret;
    public int scoreByWanderer;

    AudioMixer audioMixer;
    
    Transform spawnPoint;

    GameObject player;

    public bool unlimitedScore;

    Scene activeScene;
    string sceneName;

    [HideInInspector] public TextMeshProUGUI scoreCounter;
    TextMeshProUGUI sa1Text;
    [HideInInspector] public TextMeshProUGUI sa2Text;

    [HideInInspector] public bool sa1Activate;
    [HideInInspector] public TextMeshProUGUI sa3Text;

    //Mining
    [HideInInspector] public GameObject ore;
    GameObject[] mineBots;
    public List<GameObject> mineBotList = new List<GameObject>();
    public bool miningActivated;
    [HideInInspector] public bool oreDepleted;

    [HideInInspector] public Image sa1Warn;
    [HideInInspector] public Image sa2Warn;
    [HideInInspector] public Image sa3Warn;
    [HideInInspector] public TextMeshProUGUI sa3Counter;
    [HideInInspector] public TextMeshProUGUI sa1TimeCounter;
    [HideInInspector] public TextMeshProUGUI highScoreCounter;
    [HideInInspector] public TextMeshProUGUI pudFM;
    [HideInInspector] public TextMeshProUGUI pudDI;

    public bool damageIncrease;
    public bool fasterMovement;

    GameObject creditsPanel;
    GameObject pauseMenu;
    GameObject optionsPanel;
    GameObject mainMenuPanel;

    public bool gameStopped;

    TMP_Dropdown graphicsDropdown;
    TMP_Dropdown resolutionDropdown;
    Slider volumeSlider;
    [HideInInspector]public Slider projectileSlider;

    Resolution[] resolutions;
    bool isFullscreenTemp;
    Toggle fullscreenToggle;
    [HideInInspector]public TextMeshProUGUI gunCDRText;
    [HideInInspector] public bool bossIsDead;

    public bool joystickConnected;

    private void Start()
    {
        audioMixer = (AudioMixer)Resources.Load("Sounds/General");

        resolutions = Screen.resolutions;
        List<string> resolutionList = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resOption = resolutions[i].width + "x" + resolutions[i].height;
            resolutionList.Add(resOption);
        }

        Application.targetFrameRate = 60;
        activeScene = SceneManager.GetActiveScene();
        sceneName = activeScene.name;

        if (sceneName != "MainMenu" && sceneName != "Commercial")
        {
            if (PlayerPrefs.HasKey("HighestScore"))
                highScore = PlayerPrefs.GetInt("HighestScore");

            if (PlayerPrefs.GetInt("QualityLevel") >= 1)
                GetComponent<PostProcessVolume>().enabled = true;
            else
                GetComponent<PostProcessVolume>().enabled = false;

            gunCDRText = GameObject.Find("GunCDRText").GetComponent<TextMeshProUGUI>(); 
            projectileSlider = GameObject.Find("ProjectileSlider").GetComponent<Slider>();
            pauseMenu = GameObject.Find("PauseMenu");
            pudFM = GameObject.Find("PowerupDisplayFM").GetComponent<TextMeshProUGUI>();
            pudDI = GameObject.Find("PowerupDisplayDI").GetComponent<TextMeshProUGUI>();
            sa1Warn = GameObject.Find("sa1Warn").GetComponent<Image>();
            sa2Warn = GameObject.Find("sa2Warn").GetComponent<Image>();
            sa3Warn = GameObject.Find("sa3Warn").GetComponent<Image>();
            highScoreCounter = GameObject.Find("HighScoreCounter").GetComponent<TextMeshProUGUI>();
            sa2Text = GameObject.Find("SA2").GetComponent<TextMeshProUGUI>();
            sa3Text = GameObject.Find("SA3").GetComponent<TextMeshProUGUI>();
            sa1Text = GameObject.Find("SA1").GetComponent<TextMeshProUGUI>();
            sa1TimeCounter = GameObject.Find("SA1Timer").GetComponent<TextMeshProUGUI>();
            sa3Counter = GameObject.Find("SA3Counter").GetComponent<TextMeshProUGUI>();
            scoreCounter = GameObject.Find("ScoreCounter").GetComponent<TextMeshProUGUI>();

            player = GameObject.Find("Player");
            if (sceneName == "Tutorial")
            GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/AmbientMusic"));

            if (GameObject.Find("SpawnPoint") != null)
                spawnPoint = GameObject.Find("SpawnPoint").transform;
        } else if (sceneName == "MainMenu")
        {
            fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();
            creditsPanel = GameObject.Find("CreditsMenu");
            volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
            resolutionDropdown = GameObject.Find("ResolutionsDropdown").GetComponent<TMP_Dropdown>();
            graphicsDropdown = GameObject.Find("GraphicsDropdown").GetComponent<TMP_Dropdown>();
            optionsPanel = GameObject.Find("OptionsMenu");
            mainMenuPanel = GameObject.Find("MainMenu");

            if(PlayerPrefs.HasKey("Volume"))
            {
                audioMixer.SetFloat("GeneralVolume", PlayerPrefs.GetFloat("Volume"));
                volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            }

            if (PlayerPrefs.HasKey("QualityLevel"))
            {
                int qualityLevel = PlayerPrefs.GetInt("QualityLevel");
                graphicsDropdown.value = qualityLevel;
                QualitySettings.SetQualityLevel(qualityLevel);
            }

            if(PlayerPrefs.HasKey("ResolutionWidth") && PlayerPrefs.HasKey("ResolutionHeight"))
            {
                int resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth");
                int resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight");

                Screen.SetResolution(resolutionWidth, resolutionHeight, Screen.fullScreen);
            }

            if(PlayerPrefs.HasKey("IsFullscreen"))
            {
                isFullscreenTemp = PlayerPrefs.GetInt("IsFullscreen") == 1 ? true : false;
                Screen.fullScreen = isFullscreenTemp;
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionList);
            resolutionDropdown.captionText.text = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
            fullscreenToggle.isOn = isFullscreenTemp;
        }
    }

    private void Update()
    {
        string[] tempControllerInfo = Input.GetJoystickNames();

        if (tempControllerInfo.Length > 0)
        {
            for (int i = 0; i < tempControllerInfo.Length; ++i)
            {
                if (!string.IsNullOrEmpty(tempControllerInfo[i]))
                    joystickConnected = true;
                else
                    joystickConnected = false;
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (!gameStopped)
            {
                gameStopped = true;
                Time.timeScale = 0;
                pauseMenu.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                gameStopped = false;
                Time.timeScale = 1;
                pauseMenu.transform.localScale = new Vector3(0, 0, 0);
            }
        }

        ore = GameObject.FindGameObjectWithTag("Ore");

        if (sceneName != "MainMenu" && sceneName != "Commercial")
        {
            if (player.GetComponent<PlayerHealth>().playerIsDead)
                projectileSlider.GetComponent<Animator>().SetBool("FadeIn", false);

            if (sceneName == "Tutorial")
            {
                if (bossIsDead)
                    Invoke("EndLevel", 1);

                if(!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/AmbientMusic"));
            }

            if (score < 0)
                score = 0;

            if (highScore <= score && !unlimitedScore)
            {
                highScore = score;
                if(!Application.isEditor)
                PlayerPrefs.SetInt("HighestScore", highScore);
            }

            highScoreCounter.text = "High Score:" + highScore.ToString();

            mineBots = GameObject.FindGameObjectsWithTag("MineBot");

            if (unlimitedScore)
            {
                score = 99999;
                scoreCounter.text = "Score:Infinite";
            }
            else
                scoreCounter.text = "Score:" + score.ToString();

            int sa1MinusNeeded = SA1NeededScore - score;
            int sa2MinusNeeded = SA2NeededScore - score;
            int sa3MinusNeeded = SA3NeededScore - score;

            if (score >= SA2NeededScore && mineBotList.Count < 5)
            {
                sa2Text.text = "Ready";
                sa2Text.fontSize = 17;
            }
            else
            {
                sa2Text.text = "Need " + sa2MinusNeeded.ToString() + " Score";
                sa2Text.fontSize = 17;
            }

            if (player.GetComponent<PlayerCombat>().sa1Timer > 0.5f)
            {
                int sa1Time = Mathf.RoundToInt(player.GetComponent<PlayerCombat>().sa1Timer);
                sa1TimeCounter.text = sa1Time.ToString();
            }
            else
                sa1TimeCounter.text = "-";

            if (score >= SA1NeededScore && player.GetComponent<PlayerCombat>().sa1Timer <= 0)
                sa1Text.text = "Ready";
            else if (player.GetComponent<PlayerCombat>().sa1Timer > 0)
                sa1Text.text = "Cooldown";
            else if (score < SA1NeededScore && player.GetComponent<PlayerCombat>().sa1Timer <= 0)
                sa1Text.text = "Need " + sa1MinusNeeded.ToString() + " Score";

            if (score >= SA3NeededScore && player.GetComponent<PlayerLocomotion>().dashCount != 3)
                sa3Text.text = "Ready";
            else if (score <= SA3NeededScore && player.GetComponent<PlayerLocomotion>().dashCount != 3)
                sa3Text.text = "Need " + sa3MinusNeeded.ToString() + " Score";
            else if (player.GetComponent<PlayerLocomotion>().dashCount == 3)
                sa3Text.text = "Cooldown";

            if (player.GetComponent<PlayerLocomotion>().dashCount == 0)
                sa3Counter.text = "-";
            else
                sa3Counter.text = player.GetComponent<PlayerLocomotion>().dashCount.ToString();

            foreach (var mineBot in mineBots)
            {
                if (!mineBotList.Contains(mineBot) && !mineBot.GetComponent<Bots>().destroyBot)
                    mineBotList.Add(mineBot);
                if (oreDepleted)
                    mineBot.GetComponent<Bots>().destroyBot = true;
            }

            if (!player.GetComponent<PlayerHealth>().playerIsDead)
            {
                if (!oreDepleted && miningActivated)
                {
                    if (mineBotList.Count < 5)
                    {
                        if (Input.GetButtonDown("SpecialAttack2") && score >= SA2NeededScore && !gameStopped)
                        {
                            sa2Warn.enabled = true;
                            Invoke("CloseUsingSkillWarn", .5f);
                            score -= SA2NeededScore;
                            Instantiate(Resources.Load("Prefabs/Characters/MineBot"), spawnPoint.transform.position, spawnPoint.transform.rotation);
                        }
                    }
                    else
                    {
                        sa2Text.text = "Bot Limit Achieved";
                        sa2Text.fontSize = 12.89f;
                    }
                }
                else
                    sa2Text.text = "No Ores Found";
            }
        }
    }

    public void CloseUsingSkillWarn()
    {
        sa2Warn.enabled = false;
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void GodMode()
    {
        if (!godToggle)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().godMode = true;
            godToggle = true;
        } else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().godMode = false;
            godToggle = false;
        }
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    public void UnlimitedScore()
    {
        if (!unlimitedScore)
        {
            unlimitedScore = true;
        }
        else
        {
            score = score - 99999;
            unlimitedScore = false;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ResumeGame()
    {
        if (!gameStopped)
        {
            gameStopped = true;
            Time.timeScale = 0;
        }
        else
        {
            pauseMenu.transform.localScale = new Vector3(0, 0, 0);
            gameStopped = false;
            Time.timeScale = 1;
        }
    }

    public void OptionsMenu()
    {
        mainMenuPanel.transform.localScale = new Vector3(0, 0, 0);
        optionsPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void CloseOptionsMenu()
    {
        mainMenuPanel.transform.localScale = new Vector3(1, 1, 1);
        optionsPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);

        if (PlayerPrefs.GetInt("QualityLevel") >= 1)
            GetComponent<PostProcessVolume>().enabled = true;
        else
            GetComponent<PostProcessVolume>().enabled = false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution newRes = resolutions[resolutionIndex];
        PlayerPrefs.SetInt("ResolutionWidth", newRes.width);
        PlayerPrefs.SetInt("ResolutionHeight", newRes.height);
        Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreen);
    }

    public void SetAudioVolume (float volumeValue)
    {
        if (Application.isPlaying)
        {
            audioMixer.SetFloat("GeneralVolume", volumeValue);
            PlayerPrefs.SetFloat("Volume", volumeValue);
        }
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
    }

    public void CreditsMenu()
    {
        mainMenuPanel.transform.localScale = new Vector3(0, 0, 0);
        creditsPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void CloseCreditsMenu()
    {
        mainMenuPanel.transform.localScale = new Vector3(1, 1, 1);
        creditsPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    public void EndLevel()
    {
        Camera.main.GetComponent<Animator>().SetTrigger("LevelEnd");
        GameObject.Find("HUD").GetComponent<Animator>().SetTrigger("LevelEndHud");
        GameObject.Find("SystemSettings").GetComponent<GameBehaviour>().projectileSlider.GetComponent<Animator>().SetBool("FadeIn", false);
        player.GetComponent<PlayerCombat>().enabled = false;
        player.GetComponent<PlayerLocomotion>().enabled = false;
        player.GetComponent<PlayerHealth>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
    }
}