using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSelection : MonoBehaviour
{
    
    public static Color SpriteColor;
    public static string SpritePath;

    [SerializeField] private TMP_InputField Red;
    [SerializeField] private TMP_InputField Green;
    [SerializeField] private TMP_InputField Blue;

    [SerializeField] private GameObject OBJSample;
    [SerializeField] private GameObject OBJContainer;

    [SerializeField] private Image DisplayPlayer;

    private List<GameObject> AllObjects;

    private void Start()
    {
        StartCoroutine("PlaceOptions");
    }

    private void OnEnable()
    {
        try
        {
            UnityEngine.Object Tex = Resources.Load("Skins/789_Lorc_RPG_icons/" + SpritePath, typeof(Texture2D));
            DisplayPlayer.sprite = Sprite.Create((Texture2D)Tex, new Rect(0.0f, 0.0f, ((Texture2D)Tex).width, ((Texture2D)Tex).height), new Vector2(0.5f, 0.5f));
        }
        catch
        {
            SpritePath = "Icon.1_01";
            UnityEngine.Object Tex = Resources.Load("Skins/789_Lorc_RPG_icons/Icon.1_01", typeof(Texture2D));
            DisplayPlayer.sprite = Sprite.Create((Texture2D)Tex, new Rect(0.0f, 0.0f, ((Texture2D)Tex).width, ((Texture2D)Tex).height), new Vector2(0.5f, 0.5f));
        }
    }

    public void UpdateColor()
    {
        float R;
        float G;
        float B;

        try
        {
            if (Red.text[Red.text.Length - 1] != '.' && !Red.text.Contains(".")) { Red.text += "."; }
            if (Green.text[Green.text.Length - 1] != '.' && !Green.text.Contains(".")) { Green.text += "."; }
            if (Blue.text[Blue.text.Length - 1] != '.' && !Blue.text.Contains(".")) { Blue.text += "."; }

            R = Single.Parse(Red.text);
            G = Single.Parse(Green.text);
            B = Single.Parse(Blue.text);
        }
        catch
        {
            R = 1.0f;
            G = 1.0f;
            B = 1.0f;
        }

        SpriteColor = new Color(R, G, B);

        DisplayPlayer.color = SpriteColor;
    }

    public void UpdateImage(string Path)
    {
        UnityEngine.Object Tex = Resources.Load("Skins/789_Lorc_RPG_icons/" + Path, typeof(Texture2D));

        SpritePath = Path;
        DisplayPlayer.sprite = Sprite.Create((Texture2D)Tex, new Rect(0.0f, 0.0f, ((Texture2D)Tex).width, ((Texture2D)Tex).height), new Vector2(0.5f, 0.5f));
    }

    private IEnumerator PlaceOptions()
    {
        AllObjects = new List<GameObject>();

        UnityEngine.Object[] Textures = Resources.LoadAll("Skins/789_Lorc_RPG_icons", typeof(Texture2D));

        int i = 0;
        foreach(UnityEngine.Object Texture in Textures)
        {
            i++;

            GameObject Temp = Instantiate(OBJSample, OBJContainer.transform);

            Image Im = Temp.GetComponent<Image>();

            Im.sprite = Sprite.Create((Texture2D)Texture, new Rect(0.0f, 0.0f, ((Texture2D)Texture).width, ((Texture2D)Texture).height), new Vector2(0.5f, 0.5f));

            //Subscribe event to click
            var Test = Temp.transform.GetChild(0).GetComponent<Button>();
            Test.onClick.AddListener(delegate { UpdateImage(Texture.name); });

            AllObjects.Add(Temp);
            if (i % 10 == 0)
            {
                i = 0;
                yield return null;
            }
        }
    }
}
