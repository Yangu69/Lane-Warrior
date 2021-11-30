using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sentence 
{
    public Character character;
    public string sentenceEn;
    public string sentencePl;
    public string sentenceNl;
    public Sprite image;
}

public enum Character{
    Chuck, Other, None
}
