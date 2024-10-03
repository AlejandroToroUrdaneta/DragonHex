using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public int coins;
    public TMP_Text coinsActual;

    public int fluxMoney;
    public TMP_Text fluxActual;

    public GlobalAI globalAI;

    public PlayerProfile(int startingCoins)
    {
        coins = startingCoins;
    }

    public void UpdateText()
    {
        coinsActual.SetText(coins.ToString());
        fluxActual.SetText(fluxMoney.ToString());
    }
}
