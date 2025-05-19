using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class BackgroundChange : MonoBehaviour
{
    public GameObject nameTextBox;

    public List<CharacterColor> characterColors;

    private TextMeshProUGUI nombre;

    private Image background;

    private GameManager instance;

    private Transform character;

    private void Start()
    {
        nombre = nameTextBox.GetComponent<TextMeshProUGUI>();

        background = GetComponent<Image>();
    }

    private void Update()
    {
        foreach (CharacterColor c in characterColors)
        {
            if (nombre.text == c.name)
            {
                background.color = c.color;
            }
        }
    }

    public void SpriteChange()
    {
        if (instance == null) instance = GameManager.instance;

        foreach (CharacterColor characterColor in characterColors)
        {
            if (characterColor.name == "H27")
            {
                if (GameManager.instance.name.GetComponent<TextMeshProUGUI>().text == characterColor.name)
                {
                    GameObject go = GameManager.instance.Icon;
                    go.GetComponent<Image>().sprite = characterColor.sprite;
                }
            }
            else { 
                character = instance.characters.transform.Find(characterColor.name);

                if (character != null)
                {
                    if (GameManager.instance.name.GetComponent<TextMeshProUGUI>().text == characterColor.name) {
                    GameObject go = GameManager.instance.Icon;
                    go.GetComponent<Image>().sprite = characterColor.sprite;
                    }
                }
            }
        }

        character = null;
    }
}
