﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshTop1 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop2 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop3 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom1 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom2 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom3 = null;

    //set text values
    public void SetTextboxText(string textTop1,string textTop2,string textTop3,string textBottom1,string textBottom2,string textBottom3)
    {
        textMeshTop1.text = textTop1;
        textMeshTop1.raycastTarget = false;
        textMeshTop2.text = textTop2;
        textMeshTop2.raycastTarget = false;
        textMeshTop3.text = textTop3;
        textMeshTop3.raycastTarget = false;
        textMeshBottom1.text = textBottom1;
        textMeshBottom1.raycastTarget = false;
        textMeshBottom2.text = textBottom2;
        textMeshBottom2.raycastTarget = false;
        textMeshBottom3.text = textBottom3;
        textMeshBottom3.raycastTarget = false;
    }

}
