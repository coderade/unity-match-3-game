using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorPiece : MonoBehaviour {

    public enum ColorType {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    };

    [System.Serializable]
    public struct ColorSprite {
        public ColorType color;
        public Sprite sprite;
    };

    public ColorSprite[] colorSprites;

    private ColorType color;

    public ColorType Color {
        get { return color; }
        set { SetColor(value); }
    }


    public int NumColors{
        get { return colorSprites.Length; }
    }

    private SpriteRenderer sprite;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    void Awake() {

        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            //cheks if the dictionary contains my key - <ColorType
            if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor) {
        color = newColor;

        if (colorSpriteDict.ContainsKey(newColor)) {
            sprite.sprite = colorSpriteDict[newColor];
        }
    } 
	
	void Start () {
	
	}
	
	void Update () {
	
	}
}
