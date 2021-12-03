using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

public class ShopManager : MonoBehaviour {

    public GameObject shopContent;
    public RectTransform scrollView;
    public Text moneyText;
    public Sprite equipSprite;
    public Sprite unlockSprite;
    public GameObject chainObject;
    public AudioClip equipSound;
    public AudioClip unlockSound;
    private AudioSource audioSrc;
    public List<SkinObject> skins;
    public List<EffectObject> effects;
    public GameObject skinPrefab;
    public GameObject effectPrefab;
    public Text priceTag;
    public Text skinName;
    public Button skinButton;
    public Text specialUnlockDescription;
    private GameObject lastObject;

    private int money{
        get{
            return SaveManager.Instance.saveData.money;
        }
        set{
            SaveManager.Instance.saveData.money = value;
            SaveManager.Instance.SerializeSaveData();
            GPSManager.Instance.SaveToCloud();
            moneyText.text = money.ToString();
        }
    }

    public void Start(){
        moneyText.text = money.ToString();
        audioSrc = gameObject.GetComponent<AudioSource>();
        if(skins.Count > 0)
            GenerateSkins();
        else if(effects.Count > 0)
            GenerateEffects();
    }

    public void AddMoney(int amount){
        money += amount;
        if(lastObject != null && skinButton.gameObject.activeSelf)
            ReloadButton(lastObject);
    }
    public void GenerateSkins(){
        // Populate the container with the skins
        foreach(SkinObject skin in skins){
            // Create placeholder and attach to Shop container
            var newSkin = Instantiate(skinPrefab, Vector3.zero, Quaternion.identity);
            newSkin.name = skin.characterName;
            newSkin.transform.SetParent(shopContent.transform, false);

            // Assign values for Skeleton
            SkeletonGraphic skel = newSkin.GetComponentInChildren<SkeletonGraphic>();
            skel.skeletonDataAsset = skin.skeletonData;
            skel.Skeleton.SetSkin(skin.skeletonSkin);

            GameObject chains = null;
            if(!SaveManager.Instance.saveData.unlockedSkins.Contains(skin.characterName)){
                // Make the skin appear as locked if it's not unlocked
                skel.color = Color.black;
                LockedSkin ls = newSkin.AddComponent<LockedSkin>();
                chains = Instantiate(chainObject, new Vector3(21,-43,0), Quaternion.identity);
                ls.chain = chains;
                chains.transform.SetParent(newSkin.transform, false);
                chains.SetActive(false);
            }

            // Assign values from ScriptableObject to Object
            SkinObjectComponent skinObj = newSkin.AddComponent<SkinObjectComponent>();
            skinObj.SkeletonSkin = skin.skeletonSkin;
            skinObj.SkinName = skin.characterName;
            skinObj.SkinNamePl = skin.characterNamePl;
            skinObj.SkinNameNl = skin.characterNameNl;
            skinObj.Skel = skin.skeletonData;
            skinObj.Price = skin.price;
            skinObj.PriceTag = priceTag;
            skinObj.SpecialUnlockPlayerPref = skin.specialUnlockPlayerPref;
            skinObj.SpecialUnlockDesc = skin.specialUnlockDesc;
            skinObj.SpecialUnlockDescPl = skin.specialUnlockDescPl;
            skinObj.SpecialUnlockDescNl = skin.specialUnlockDescNl;

            // Finalize skin update
            skel.Skeleton.SetSlotsToSetupPose();
            skel.LateUpdate();
        }
        if(shopContent.transform.childCount > 0){
            // Initialize UI for the first skin
            ReloadText(shopContent.transform.GetChild(0).gameObject);
            ReloadButton(shopContent.transform.GetChild(0).gameObject);
            scrollView.GetComponent<ScrollRectSnap>().Initialize();
            scrollView.GetComponent<ScrollRectSnap>().UpdateSkinInfo();
        }
    }

    public void GenerateEffects(){
        // Populate the container with the effects
        foreach(EffectObject effect in effects){
            var newEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
            newEffect.name = effect.effectName;
            newEffect.transform.SetParent(shopContent.transform, false);
            newEffect.transform.GetChild(1).GetComponent<Image>().sprite = effect.shopImage;

            EffectObjectComponent eoc = newEffect.AddComponent<EffectObjectComponent>();
            eoc.SkinName = effect.effectName;
            eoc.SkinNamePl = effect.effectNamePl;
            eoc.SkinNameNl = effect.effectNameNl;
            eoc.Particles = effect.particles;
            eoc.ShopImage = effect.shopImage;
            eoc.Price = effect.price;
            eoc.PriceTag = priceTag;
            eoc.SpecialUnlockPlayerPref = effect.specialUnlockPlayerPref;
            eoc.SpecialUnlockDesc = effect.specialUnlockDesc;
            eoc.SpecialUnlockDescPl = effect.specialUnlockDescPl;
            eoc.SpecialUnlockDescNl = effect.specialUnlockDescNl;

            if(!SaveManager.Instance.saveData.unlockedEffects.Contains(effect.effectName)){
                newEffect.transform.GetChild(1).GetComponent<Image>().color = Color.black;
                LockedSkin ls = newEffect.AddComponent<LockedSkin>();
                GameObject chains = Instantiate(chainObject, new Vector3(21,-43,0), Quaternion.identity);
                ls.chain = chains;
                chains.transform.SetParent(newEffect.transform, false);
                chains.SetActive(false);
            }
        }
        ReloadText(shopContent.transform.GetChild(0).gameObject);
        ReloadButton(shopContent.transform.GetChild(0).gameObject);
        scrollView.GetComponent<ScrollRectSnap>().Initialize();
        scrollView.GetComponent<ScrollRectSnap>().UpdateSkinInfo();
    }

    public void RemoveSkins(){
        foreach(Transform t in shopContent.transform){
            Destroy(t.gameObject);
        }
    }

    public void ReloadText(GameObject skin){
        skinName.gameObject.GetComponent<Animator>().SetTrigger("Show");
        var skn = skin.GetComponent<SkinObjectComponent>();
        var eff = skin.GetComponent<EffectObjectComponent>();
        // Item name update
        if(Application.systemLanguage == SystemLanguage.Polish){
            if(skn)
                skinName.text = skn.SkinNamePl;
            else if(eff)
                skinName.text = eff.SkinNamePl;
        }else if(Application.systemLanguage == SystemLanguage.Dutch){
            if(skn)
                skinName.text = skn.SkinNameNl;
            else if(eff)
                skinName.text = eff.SkinNameNl;
        }else{
            if(skn)
                skinName.text = skn.SkinName;
            else if(eff)
                skinName.text = eff.SkinName;
        }
        // Price tag update
        if((skn && SaveManager.Instance.saveData.unlockedSkins.Contains(skin.name)) || (eff && SaveManager.Instance.saveData.unlockedEffects.Contains(skin.name))){
            priceTag.transform.parent.gameObject.SetActive(false);
        }else{
            priceTag.transform.parent.gameObject.SetActive(true);
            if(skn)
                priceTag.text = skn.Price.ToString();
            else if(eff)
                priceTag.text = eff.Price.ToString();
        }
    }

    public void ReloadButton(GameObject skin){
        skinButton.gameObject.SetActive(false);
        lastObject = skin;
        SkinObjectComponent skinObj = skin.GetComponent<SkinObjectComponent>();
        EffectObjectComponent effectObj = skin.GetComponent<EffectObjectComponent>();
        string specialUnlockPref = "";
        string specialUnlockDesc = "";
        string specialUnlockDescPl = "";
        string specialUnlockDescNl = "";
        int price = 0;
        if(skinObj){
            specialUnlockPref = skinObj.SpecialUnlockPlayerPref;
            specialUnlockDesc = skinObj.SpecialUnlockDesc;
            specialUnlockDescPl = skinObj.SpecialUnlockDescPl;
            specialUnlockDescNl = skinObj.SpecialUnlockDescNl;
            price = skinObj.Price;
        }else if(effectObj){
            specialUnlockPref = effectObj.SpecialUnlockPlayerPref;
            specialUnlockDesc = effectObj.SpecialUnlockDesc;
            specialUnlockDescPl = effectObj.SpecialUnlockDescPl;
            specialUnlockDescNl = effectObj.SpecialUnlockDescNl;
            price = effectObj.Price;
        }


        Text txt = skinButton.gameObject.GetComponentInChildren<Text>();
        specialUnlockDescription.gameObject.SetActive(false);

        // If unlocked
        if((skinObj && SaveManager.Instance.saveData.unlockedSkins.Contains(skin.name)) || (effectObj && SaveManager.Instance.saveData.unlockedEffects.Contains(skin.name))){
            skinButton.gameObject.GetComponent<Image>().sprite = equipSprite;

            if(Application.systemLanguage == SystemLanguage.Polish)
                txt.text = "UBIERZ";
            else if(Application.systemLanguage == SystemLanguage.Dutch)
                txt.text = "AAN DOEN";
            else
                txt.text = "EQUIP";
            if(skinObj){
                if(SaveManager.Instance.saveData.equippedSkin == skin.name){
                    skinButton.interactable = false;
                }else{
                    skinButton.interactable = true;
                }
            }else if(effectObj){
                if(SaveManager.Instance.saveData.equippedEffect == skin.name){
                    skinButton.interactable = false;
                }else{
                    skinButton.interactable = true;
                }
            }
            skinButton.gameObject.SetActive(true);
        }else if(specialUnlockPref != "" && ((skinObj && !SaveManager.Instance.saveData.unlockedSkins.Contains(skin.name)) || (effectObj && !SaveManager.Instance.saveData.unlockedEffects.Contains(skin.name)))){
            specialUnlockDescription.gameObject.SetActive(true);
            if(Application.systemLanguage == SystemLanguage.Polish)
                specialUnlockDescription.text = specialUnlockDescPl;
            else if(Application.systemLanguage == SystemLanguage.Dutch)
                specialUnlockDescription.text = specialUnlockDescNl;
            else
                specialUnlockDescription.text = specialUnlockDesc;
        }else{
            skinButton.gameObject.GetComponent<Image>().sprite = unlockSprite;

            if(Application.systemLanguage == SystemLanguage.Polish)
                txt.text = "KUP";
            else if(Application.systemLanguage == SystemLanguage.Dutch)
                txt.text = "ONTGRENDELEN";
            else
                txt.text = "UNLOCK";
            if(money >= price)
                skinButton.interactable = true;
            else
                skinButton.interactable = false;
            skinButton.gameObject.SetActive(true);
        }
        
    }
    public void ButtonAction(int index){
        if(SaveManager.Instance.saveData.unlockedSkins.Contains(shopContent.transform.GetChild(index).gameObject.name) || SaveManager.Instance.saveData.unlockedEffects.Contains(shopContent.transform.GetChild(index).gameObject.name))
            Equip(index);
        else
            Unlock(index);
    }
    public void Unlock(int index){
        GameObject skin = shopContent.transform.GetChild(index).gameObject;
        SkeletonGraphic selectedSkeleton = skin.GetComponentInChildren<SkeletonGraphic>();
        SkinObjectComponent skn = skin.GetComponent<SkinObjectComponent>();
        EffectObjectComponent effectObj = skin.GetComponent<EffectObjectComponent>();

        skin.GetComponent<LockedSkin>().chain.GetComponent<Animator>().SetTrigger("Unlock");
        Destroy(skin.GetComponent<LockedSkin>());
        if(skn){
            SaveManager.Instance.saveData.unlockedSkins.Add(skn.SkinName);
            selectedSkeleton.color = Color.white;
            selectedSkeleton.AnimationState.SetAnimation(0,"MidAttack1",false);
            selectedSkeleton.AnimationState.AddAnimation(0,"Idle", true, 0.3f);
            GPSManager.Instance.IncrementEvent("Skins Unlocked", 1);
            GPSManager.Instance.IncrementAchievement("10 Skins", 1);
        }
        if(effectObj){
            skin.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            SaveManager.Instance.saveData.unlockedEffects.Add(effectObj.SkinName);
            GPSManager.Instance.IncrementEvent("Effects Unlocked", 1);
            GPSManager.Instance.IncrementAchievement("10 Effects", 1);
        }

        if(unlockSound != null){
            audioSrc.clip = unlockSound;
            audioSrc.Play();
        }
        ReloadButton(skin);
        priceTag.transform.parent.gameObject.SetActive(false);
        if(skn)
            money -= skn.Price;
        else if(effectObj)
            money -= effectObj.Price;
        SaveManager.Instance.SerializeSaveData();
        GPSManager.Instance.SaveToCloud();
    }

    public void Equip(int index){
        GameObject skin = shopContent.transform.GetChild(index).gameObject;
        SkeletonGraphic selectedSkeleton = skin.GetComponentInChildren<SkeletonGraphic>();
        SkinObjectComponent skinObj = skin.GetComponent<SkinObjectComponent>();
        EffectObjectComponent effectObj = skin.GetComponent<EffectObjectComponent>();
        if(equipSound != null){
            audioSrc.clip = equipSound;
            audioSrc.Play();
        }
        if(skinObj){
            SaveManager.Instance.saveData.equippedSkin = skinObj.SkinName;
            GameData.Instance.equippedSkeleton = skinObj.Skel;
            GameData.Instance.equippedSkin = skinObj.SkeletonSkin;
        }
        if(effectObj){
            SaveManager.Instance.saveData.equippedEffect = effectObj.SkinName;
            GameData.Instance.equippedEffect = effectObj.Particles;
            GameData.Instance.equippedEffectName = effectObj.SkinName;
        }
        ReloadButton(skin);
    }
}
