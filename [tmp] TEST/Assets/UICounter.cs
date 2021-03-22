using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICounter : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = "0";
    }
    public void UpdateUI(string text)
    {
        tmp.text = text;
    }
}
