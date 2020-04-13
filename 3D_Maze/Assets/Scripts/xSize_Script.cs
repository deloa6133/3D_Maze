using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class xSize_Script : MonoBehaviour
{
    Text xSizeText;

    // Start is called before the first frame update
    void Start()
    {
        xSizeText = GetComponent<Text>();
    }

    public void TextUpdate(float value)
    {
        xSizeText.text = (Mathf.RoundToInt(value).ToString() + ": X");
    }
}
