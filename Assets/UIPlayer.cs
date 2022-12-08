using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIPlayer : MonoBehaviour
{
    public TMP_Text Name, Score;

    public void SetPlayer(Player player)
    {
        Name.text = player.data.Name;
        Score.text = player.data.Score.ToString();
    }
}
