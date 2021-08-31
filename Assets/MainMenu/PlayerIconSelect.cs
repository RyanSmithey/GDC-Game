using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconSelect : MonoBehaviour
{
    // Start is called before the first frame update

    public int i = 0; //Number of previous samples
    

    public void SetSize()
    {
        Image I = gameObject.GetComponent<Image>();
        RectTransform R = gameObject.GetComponent<RectTransform>();

        R.offsetMax = new Vector2(R.offsetMax.x, -(R.rect.width + (R.rect.width * 2 * i)));
        
        R.sizeDelta = new Vector2(R.sizeDelta.x, R.rect.width);
    }
}
