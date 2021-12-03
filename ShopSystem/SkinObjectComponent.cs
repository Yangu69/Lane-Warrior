using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class SkinObjectComponent : MonoBehaviour {

	private string skinName;
	private string skinNamePl;
	private string skinNameNl;
	private string skeletonSkin;
	private int price;
	private SkeletonDataAsset skel;
	private Text priceTag;
	private string specialUnlockPlayerPref;
	private string specialUnlockDesc;
	private string specialUnlockDescPl;
	private string specialUnlockDescNl;

	public string SkinName {get;set;}
	public string SkinNamePl {get;set;}
	public string SkinNameNl {get;set;}
	public string SkeletonSkin {get;set;}
	public int Price {get;set;}
	public SkeletonDataAsset Skel {get;set;}
	public Text PriceTag {get;set;}
	public string SpecialUnlockPlayerPref {get; set;}
	public string SpecialUnlockDesc {get;set;}
	public string SpecialUnlockDescPl {get;set;}
	public string SpecialUnlockDescNl {get;set;}
}
