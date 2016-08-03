using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPiece : MonoBehaviour {

    public enum CharacterType   {
        LINK,
        IMPA,
        LANA,
        MANHANDLA,
        SHEIK,
        ZELDA,
        ANY,
        COUNT
    };

    [System.Serializable]
    public struct CharacterSprite {
        public CharacterType character;
        public Sprite sprite;
    };

    public CharacterSprite[] characterSprites;

    private CharacterType character;

    public CharacterType Character {
        get { return character; }
        set { SetCharacter(value); }
    }


    public int NumCharacters{
        get { return characterSprites.Length; }
    }

    private SpriteRenderer sprite;

    private Dictionary<CharacterType, Sprite> characterSpriteDict;

    void Awake() {

        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();

        characterSpriteDict = new Dictionary<CharacterType, Sprite>();

        for (int i = 0; i < characterSprites.Length; i++)
        {
            //cheks if the dictionary contains my key - <CharacterType
            if (!characterSpriteDict.ContainsKey(characterSprites[i].character))
            {
                characterSpriteDict.Add(characterSprites[i].character, characterSprites[i].sprite);
            }
        }
    }

    public void SetCharacter(CharacterType newCharacter) {
        character = newCharacter;

        if (characterSpriteDict.ContainsKey(newCharacter)) {
            sprite.sprite = characterSpriteDict[newCharacter];
        }
    } 
	
	void Start () {
	
	}
	
	void Update () {
	
	}
}
