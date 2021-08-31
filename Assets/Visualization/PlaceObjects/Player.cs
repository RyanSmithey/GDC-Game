using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int PlayerID = 0;

    public TextMeshPro Name;
    public TextMeshPro IPCs;
    private void Start()
    {
        try
        {
            SpriteRenderer SR = gameObject.GetComponent<SpriteRenderer>();

            Object Tex = Resources.Load("Skins/789_Lorc_RPG_icons/" + Abstract.AllPlayers[PlayerID].SkinName, typeof(Texture2D));
            SR.sprite = Sprite.Create((Texture2D)Tex, new Rect(0.0f, 0.0f, ((Texture2D)Tex).width, ((Texture2D)Tex).height), new Vector2(0.5f, 0.5f));
            SR.color = Abstract.AllPlayers[PlayerID].SkinColor;
        }
        catch
        {

        }
    }
    // Update is called once per frame
    void Update()
    {
        //SetHealth();
        SetText();

        uint[] Loc = Abstract.AllPlayers[PlayerID].Location;

        gameObject.transform.position = Abstract.Grid[Loc[0], Loc[1]].Location;

        if (Abstract.AllPlayers[PlayerID].Health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void SetHealth()
    {
        if (Abstract.AllPlayers[PlayerID].Health == 3) { gameObject.GetComponent<SpriteRenderer>().color = Color.green; }
        if (Abstract.AllPlayers[PlayerID].Health == 2) { gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; }
        if (Abstract.AllPlayers[PlayerID].Health == 1) { gameObject.GetComponent<SpriteRenderer>().color = Color.red; }
        if (PlayerID == Cursor.ActivePlayer) { gameObject.GetComponent<SpriteRenderer>().color = Color.white; }
    }

    void SetText()
    {
        Name.text = Abstract.AllPlayers[PlayerID].Name;
        IPCs.text = Abstract.AllPlayers[PlayerID].ActionPoints.ToString();
    }
}
