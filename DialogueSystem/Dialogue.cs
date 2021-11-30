using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Dialogue_", menuName="ScriptableObjects/Dialogue", order=3)]
public class Dialogue : ScriptableObject
{
    [Range(0.5f,2)]
    public float pitch;
    public GameObject character;
    public Sentence[] sentences;

    public bool changeScene = false;
    public int sceneID;

    public bool unlocks = false;
    public Unlock[] unlocksList;
}
