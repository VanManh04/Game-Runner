using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ColorToSell
{
    public Color color;
    public int price;
}

public enum CollorType
{
    playerCollor,
    platformCollor
}

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI notifyText;
    [Space]

    [Header("Platform colors")]
    [SerializeField] private GameObject platformColorButton;
    [SerializeField] private Transform platformColorParent;
    [SerializeField] private Image platformDisplay;
    [SerializeField] ColorToSell[] platformColors;

    [Header("Player color")]
    [SerializeField] private GameObject playerColorButton;
    [SerializeField] private Transform playerColorParent;
    [SerializeField] private Image playerDisplay;
    [SerializeField] ColorToSell[] playerColor;
    
    void Start()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");

        for (int i = 0; i < platformColors.Length; i++)
        {
            Color color = platformColors[i].color;
            int price = platformColors[i].price;

            GameObject newButton = Instantiate(platformColorButton, platformColorParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString("#,#");

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseColor(color,price,CollorType.platformCollor));
        }
        

        for (int i = 0; i < playerColor.Length; i++)
        {
            Color color = playerColor[i].color;
            int price = playerColor[i].price;

            GameObject newButton = Instantiate(playerColorButton, playerColorParent);

            newButton.transform.GetChild(0).GetComponent<Image>().color = color;
            newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString("#,#");

            newButton.GetComponent<Button>().onClick.AddListener(() => PurchaseColor(color,price,CollorType.playerCollor));
        }

    }

    private void PurchaseColor(Color color , int price, CollorType collorType)
    {
        AudioManager.instance.PlaySFX(4);

        if (EnoughMoney(price))
        {
            if(collorType==CollorType.platformCollor)
            {
                GameManager.instance.platformColor = color;
                platformDisplay.color = color;
            }
            else if(collorType == CollorType.playerCollor)
            {
                GameManager.instance.player.GetComponent<SpriteRenderer>().color = color;
                GameManager.instance.SaveColor(color.r,color.g,color.b);
                playerDisplay.color = color;
            }
            StartCoroutine(Notify("Purchase successful", 1f));
            return;
        }
            StartCoroutine(Notify("not enough money!", 1f));
    }

    private bool EnoughMoney(int price)
    {
        int myCoins = PlayerPrefs.GetInt("Coins");

        if (myCoins > price)
        {
            int newAmountOfCoins = myCoins - price;

            PlayerPrefs.SetInt("Coins", newAmountOfCoins);

            coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");
            //Debug.Log("Purchase successful");
            return true;
        }else
        {
            //Debug.Log("not enough money");
            return false;
        }    
    }

    IEnumerator Notify(string text, float seconds)
    {
        notifyText.text = text;
        yield return new WaitForSeconds(seconds);
        notifyText.text = "Click to buy";
    }
}
