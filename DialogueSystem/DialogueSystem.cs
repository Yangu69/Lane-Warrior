using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class DialogueSystem : MonoBehaviour
{
    public Dialogue currentDialogue;
    public static DialogueSystem Instance;
    private Queue<Sentence> sentences = new Queue<Sentence>();
    public Animator chuckAnimator;
    public Animator otherCharacterAnimator;
    public Image chuckImage;
    public GameObject otherCharacter;

    private Image otherCharacterImage;

    public AudioSource audioSrc;
    public AudioClip[] chuckClips;
    public AudioClip[] otherCharacterClips;
    public Color inactiveCharacterColor = new Color(0.298f, 0.369f, 1f, 1f);
    public Text dialogueText;
    public GameObject arrow;
    public GameObject popupImage;
    private bool toFinish = false;
    private bool currentlySpeaking = false;

    void Awake(){
        // Singleton Pattern
        if(Instance == null){
            Instance = this;
        }else{
            if(gameObject != DialogueSystem.Instance.gameObject)
                Destroy(transform.parent.gameObject);
        }
    }

    public void LoadDialogue(Dialogue dialogue){
        // Initialize Dialogue System
        currentDialogue = dialogue;
        LoadOtherCharacter();
        gameObject.SetActive(true);
        sentences.Clear();
        LoadSentences();
    }

    private void LoadOtherCharacter(){
        // Initialize character Chuck will be talking with
        GameObject otherCharacterObject = currentDialogue.character;
        if(!otherCharacterObject)
            return;

        GameObject otherCharacterInstance = Instantiate(otherCharacterObject);
        otherCharacterInstance.transform.SetParent(gameObject.transform, false);

        otherCharacterInstance.transform.SetSiblingIndex(otherCharacter.transform.GetSiblingIndex());
        Destroy(otherCharacter);
        otherCharacter = otherCharacterInstance;
        otherCharacterImage = otherCharacter.GetComponentInChildren<Image>();
        otherCharacterAnimator = otherCharacter.GetComponentInChildren<Animator>();
    }

    private void LoadSentences(){
        // Make queue out of sentences and show the first one
        foreach(Sentence sentence in currentDialogue.sentences){
            this.sentences.Enqueue(sentence);
        }
        NextSentence();
    }

    public void NextSentence(){
        if(currentlySpeaking){
            // If user tapped while the text is still loading, load it immediately
            FinishCurrentText();
            return;    
        }

        if(popupImage.activeSelf)
            popupImage.SetActive(false);
        arrow.SetActive(false);
        if(sentences.Count == 0)
            EndDialogue();
        if(sentences.Count > 0){
            Sentence next = sentences.Dequeue();
            StartCoroutine(LoadText(next));
        }
    }

    public void FinishCurrentText(){
        toFinish = true;
    }

    private IEnumerator LoadText(Sentence currentSentence){
        toFinish = false;
        currentlySpeaking = true;
        if(currentSentence.character == Character.Chuck){
            // Make Chuck the active character if he's the one speaking
            StartCoroutine(SwitchSize(chuckAnimator.gameObject.transform, true));
            StartCoroutine(SwitchSize(otherCharacterAnimator.gameObject.transform, false));
            chuckAnimator.SetBool("Talking", true);
            chuckImage.color = Color.white;
            foreach(Transform img in chuckImage.transform)
                img.GetComponent<Image>().color = Color.white;
            otherCharacterImage.color = inactiveCharacterColor;
            foreach(Transform img in otherCharacterImage.transform)
                img.GetComponent<Image>().color = inactiveCharacterColor;
        }else if(currentSentence.character == Character.Other){
            // Make the other character the active character if they're the one speaking
            StartCoroutine(SwitchSize(chuckAnimator.gameObject.transform, false));
            StartCoroutine(SwitchSize(otherCharacterAnimator.gameObject.transform, true));
            otherCharacterAnimator.SetBool("Talking", true);
            chuckImage.color = inactiveCharacterColor;
            foreach(Transform img in chuckImage.transform)
                img.GetComponent<Image>().color = inactiveCharacterColor;
            otherCharacterImage.color = Color.white;
            foreach(Transform img in otherCharacterImage.transform)
                img.GetComponent<Image>().color = Color.white;
        }else{
            // Make both characters inactive if no one is speaking
            StartCoroutine(SwitchSize(chuckAnimator.gameObject.transform, false));
            StartCoroutine(SwitchSize(otherCharacterAnimator.gameObject.transform, false));
             chuckImage.color = inactiveCharacterColor;
            foreach(Transform img in chuckImage.transform)
                img.GetComponent<Image>().color = inactiveCharacterColor;
            otherCharacterImage.color = inactiveCharacterColor;
            foreach(Transform img in otherCharacterImage.transform)
                img.GetComponent<Image>().color = inactiveCharacterColor;
        }
        if(currentSentence.image){
            popupImage.GetComponent<Image>().sprite = currentSentence.image;
            popupImage.SetActive(true);
        }
        dialogueText.text = "";
        AudioClip[] usedClips = null;
        if(currentSentence.character == Character.Chuck){
            usedClips = chuckClips;
            audioSrc.pitch = 1;
        }else if(currentSentence.character == Character.Other){
            usedClips = otherCharacterClips;
            audioSrc.pitch = currentDialogue.pitch;
        }

        int a = 4;
        string sentence;
        // Load appropriate sentence depending on the language
        if(Application.systemLanguage == SystemLanguage.Polish)
            sentence = currentSentence.sentencePl;
        else if(Application.systemLanguage == SystemLanguage.Dutch)
            sentence = currentSentence.sentenceNl;
        else
            sentence = currentSentence.sentenceEn;
        for(int i = 0; i < sentence.Length; i++){
            // Show the sentence letter by letter
            if(toFinish){
                dialogueText.text = sentence;
                toFinish = false;
                break;
            }
            dialogueText.text += sentence[i];

            char x = sentence[i];
            x = Char.ToLower(x);
            if(usedClips != null){
                if(a == 4){
                    // Play sound every 5 letters
                    if(x - 97 >= 0 && x - 97 <= 26){
                        audioSrc.PlayOneShot(usedClips[x-97]);
                    }else{
                        audioSrc.PlayOneShot(usedClips[UnityEngine.Random.Range(0,26)]);
                    }
                    a = -1;
                }
                a++;
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return new WaitForSecondsRealtime(0.1f);
        if(currentSentence.character == Character.Chuck)
            chuckAnimator.SetBool("Talking", false);
        else if(currentSentence.character == Character.Other)
            otherCharacterAnimator.SetBool("Talking", false);
        arrow.SetActive(true);
        currentlySpeaking = false;
    }

    IEnumerator SwitchSize(Transform character, bool talking){
        float t = 0;
        while (t<1){
            float characterStartScale = character.localScale.x;
            float toLerp;
            if(talking)
                toLerp = Mathf.Lerp(characterStartScale,1,t);
            else
                toLerp = Mathf.Lerp(characterStartScale,0.9f,t);
            character.localScale = new Vector3(toLerp,toLerp, 1);
            t+= Time.deltaTime*3;
            yield return new WaitForEndOfFrame();
        }
    }

    public void EndDialogue(){
        // Disable the object and show unlocks if any
        if(!gameObject.activeSelf)
            return;
        gameObject.SetActive(false);
        toFinish = false;
        currentlySpeaking = false;
        if(currentDialogue.changeScene)
            LoadingControl.Instance.StartLoadingScene(currentDialogue.sceneID);
        if(currentDialogue.unlocks){
            Unlocker.Instance.Unlock(currentDialogue.unlocksList);
        }
    }

    void OnApplicationQuit(){
        if(currentDialogue != null && currentDialogue.unlocks){
            // If the user quits the game while dialogue is ongoing, immediately unlock the items that were supposed to be unlocked after the dialogue ends
            foreach(Unlock u in currentDialogue.unlocksList){
                switch(u.unlockType){
                    case UnlockType.Skin:
                        if(!SaveManager.Instance.saveData.unlockedSkins.Contains(u.toUnlock))
                            SaveManager.Instance.saveData.unlockedSkins.Add(u.toUnlock);
                        break;
                    
                    case UnlockType.Effect:
                        if(!SaveManager.Instance.saveData.unlockedEffects.Contains(u.toUnlock))
                            SaveManager.Instance.saveData.unlockedEffects.Add(u.toUnlock);
                        break;

                    case UnlockType.Stage:
                        if(!SaveManager.Instance.saveData.unlockedLevels.Contains(u.toUnlock))
                            SaveManager.Instance.saveData.unlockedLevels.Add(u.toUnlock);
                        break;
                }
                SaveManager.Instance.SerializeSaveData();
            }
        }
    }
}
