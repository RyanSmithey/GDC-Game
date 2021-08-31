using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetValues : MonoBehaviour
{
    public List<TextMeshProUGUI> Values;

    public void SetValue(int index, string text)
    {
        Values[index].text = text;
    }
}
