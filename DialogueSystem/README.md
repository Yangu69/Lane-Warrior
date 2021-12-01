# Dialogue System

![dialoguegif](https://user-images.githubusercontent.com/42221923/144132886-b9dd119b-f899-456a-bbee-58678610e8f4.gif)

**Lane Warrior** makes use of Dialogue System to introduce the player into Chuck's world.

## How it works
DialogueSystem is the base that holds information about current dialogue and translates it into UI. At first, Dialogue object needs to be created within the assets, as it's a Scriptable Object. Such Dialogue object may contain information about Sentences, and who's speaking these sentences. It's possible to select different prefab for other characters that will be used in the Dialogue besides Chuck.

Such Dialogue object is passed over after initiating a Story Mode level, or after returning to Main Menu by defeating a Boss. DialogueSystem then enqueues sentences from the dialogue, and prepares UI for used characters.

```c#
private void LoadSentences(){
    // Make queue out of sentences and show the first one
    foreach(Sentence sentence in currentDialogue.sentences){
        this.sentences.Enqueue(sentence);
    }
    NextSentence();
}
```
After loading a sentence, the system shows it letter by letter over time, in order to provide some animation and time for the characters to actually talk.

Characters have the following structure in order to animate them while they're talking, and show that they're alive by blinking.
![blinkgif](https://user-images.githubusercontent.com/42221923/144144840-11c998f0-54f1-49a4-a695-74c5035123ce.gif)
![image](https://user-images.githubusercontent.com/42221923/144144538-c5889ce7-f2c6-40ae-a866-784df3a1a205.png)

## Sound Effects
Apart from the animations, we've also added cool talking sound effects inspired by Animal Crossing.

https://user-images.githubusercontent.com/42221923/144146361-3fef3ab8-4662-4102-940e-2684f5d3284c.mp4

26 pronouncations of each letter were recorded and used as clips for this system. Every 5th letter that appears is followed by the pronounciation of the letter. If what appears is not a letter, random letter sound plays, for convenience.

```c#
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
```
## Future Improvements
- Node based dialogue objects, instead of Scriptable Objects edited in Inspector
- String database with localization, instead of having a variable for sentences in each applicable language
