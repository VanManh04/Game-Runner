using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UI_Main ui;
    public Player player;
    public bool colorEntierPlatform;

    [Header("Sky box")]
    [SerializeField] private Material[] skyBoxMat;

    [Header("Purchased collor")]
    public Color platformColor;

    [Header("Score info")]
    public int coins;
    public float distance;
    public float score;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        SetupSkyBox(PlayerPrefs.GetInt("SkyBoxSetting"));
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    public void SetupSkyBox(int i)
    {
        if (i < 1)
            RenderSettings.skybox = skyBoxMat[i];
        else
            RenderSettings.skybox = skyBoxMat[Random.Range(0, skyBoxMat.Length)];

        PlayerPrefs.SetInt("SkyBoxSetting", i);
    }

    private void LoadColor()
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        Color color = new Color(PlayerPrefs.GetFloat("ColorR")
                                , PlayerPrefs.GetFloat("ColorG")
                                , PlayerPrefs.GetFloat("ColorB")
                                , PlayerPrefs.GetFloat("ColorA", 1));
        sr.color = color;
    }

    private void Update()
    {
        if(player.transform.position.x>distance)
            distance=player.transform.position.x;
    }

    public void UnlockPlayer() => player.playerUnlocked = true;

    public void RestartLever()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", coins + savedCoins);

        score = distance * coins;
        PlayerPrefs.SetFloat("LastScore", score);
        if (PlayerPrefs.GetFloat("HighScore") < score)
            PlayerPrefs.SetFloat("HighScore", score);
    }
    
    public void SaveColor(float r,float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }

    public void GameEnded()
    {
        SaveInfo();
        ui.OpenEndGameUI();
    }
}
