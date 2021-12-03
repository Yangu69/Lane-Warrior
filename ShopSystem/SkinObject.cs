using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[CreateAssetMenu(fileName="Shop Skin", menuName="Shop Skin")]
public class SkinObject : ScriptableObject {

	public string characterName;
	public string characterNamePl;
	public string characterNameNl;
	[HideInInspector]
	public SkeletonDataAsset skeletonData;
	[HideInInspector]
	public string skeletonSkin;
	[HideInInspector]
	public int index;
	public int price;
	public string specialUnlockPlayerPref;
	public string specialUnlockDesc;
	public string specialUnlockDescPl;
	public string specialUnlockDescNl;

}
