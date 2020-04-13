using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ySize_Script : MonoBehaviour
{
    Text ySizeText;

    // Start is called before the first frame update
    void Start()
    {
        ySizeText = GetComponent<Text>();
    }

    public void TextUpdate(float value)
    {
        ySizeText.text = (Mathf.RoundToInt(value).ToString() + ": Y");
    }
}
