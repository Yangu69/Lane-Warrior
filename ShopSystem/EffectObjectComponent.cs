using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectObjectComponent : MonoBehaviour
{
    private string skinName;
	private string skinNamePl;
	private string skinNameNl;
    private GameObject particles;
    private Sprite shopImage;
	private int price;
	private string specialUnlockPlayerPref;
	private string specialUnlockDesc;
	private string specialUnlockDescPl;
	private string specialUnlockDescNl;
    private Text priceTag;

    public string SkinName {get;set;}
    public string SkinNamePl {get;set;}
    public string SkinNameNl {get;set;}
    public GameObject Particles {get;set;}
    public Sprite ShopImage {get;set;}
    public int Price {get;set;}
    public Text PriceTag {get;set;}
    public string SpecialUnlockPlayerPref {get;set;}
    public string SpecialUnlockDesc {get;set;}
    public string SpecialUnlockDescPl {get;set;}
    public string SpecialUnlockDescNl {get;set;}
}
