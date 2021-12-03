using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Shop Effect", menuName="Shop Effect")]
public class EffectObject : ScriptableObject {

	public string effectName;
	public string effectNamePl;
	public string effectNameNl;
	public GameObject particles;
	public Sprite shopImage;
	public int price;
	public string specialUnlockPlayerPref;
	public string specialUnlockDesc;
	public string specialUnlockDescPl;
	public string specialUnlockDescNl;

}
