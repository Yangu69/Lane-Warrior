using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class SaveData 
{
    public int secondsPlayed;
    public int matchesPlayed;
    public List<string> unlockedLevels = new List<string>();
    public List<string> unlockedSkins = new List<string>();
    public List<string> unlockedEffects = new List<string>();
    public List<string> shownPrompts = new List<string>();
    public string equippedSkin = "Mighty Chuck";
    public string equippedEffect = "None";
    public int money = 0;
    public bool tutorialSeen = false;
    public int rank = 0;
    public int stars = 0;
    public int health = 3;
    public Dictionary<string,int> quests = new Dictionary<string, int>();
    public Dictionary<string,int> highscores = new Dictionary<string, int>();
    public Dictionary<string,int> highscoresEnemies = new Dictionary<string, int>();
    public Dictionary<string,int> highscoresCombo = new Dictionary<string, int>();
}
