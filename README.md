# Lane Warrior

- **Engine:** Unity
- **Language:** C#
- **Target Platforms:** Android

## Description
**Lane Warrior** is a 2D sidescrolling game where the player gets through the levels by eliminating approaching enemies. This is done by tapping on one of the 3 lanes in the right moment.

I'm taking care of all the programming side of the project. [@DawidKay](https://github.com/DawidKay/) is responsible for the art.

![lanegif](https://user-images.githubusercontent.com/42221923/143774388-7021d6c5-4b03-41b2-8f9b-afacbdfc3522.gif)

The game offers two game modes:
- Story Mode - Dialogues and bosses! After defeating certain amount of enemies, a boss with mechanics unique to each level shows up. The boss needs to be beaten in order to unlock the next level.
- Free Mode - Achieve higher scores! This mode spawns enemies infinitely, so no bosses this time. This mode is used for practice and showing the leaderboards who's the boss.


The game integrates such SDKs as:
- **Google Play Games Services:** For leaderboards, achievements, and cloud saves.
- **AdMob:** For ad system integration. Users can willingly watch ads to earn some coins or gain extra life after dying.
- **Spine Unity Runtime:** For managing animation state and skins at runtime. Main character and the enemies were created and exported from Spine.

## Showcase
### [Dialogue System](https://github.com/YanguDev/Lane-Warrior/tree/main/DialogueSystem)
![dialoguegif](https://user-images.githubusercontent.com/42221923/143951062-47c03a24-f8c8-4d6c-a19a-bd3c87e3266f.gif)

Having conversations is important. We're using dialogues to provide some small information about the characters and the story behind the enemies we're fighting with.

### [Save System](https://github.com/YanguDev/Lane-Warrior/tree/main/SaveSystem)
User statistics and unlocks are saved on both the device and in the cloud, meaning it's always possible to continue where they left off.

### [Shop System](gogole)
![shopgif](https://user-images.githubusercontent.com/42221923/143946313-5e8bbabb-d317-4254-a303-cd5bc1dcc1c2.gif)

Games with customization options are always a nice find, so a shop system was created where the player can buy and equip different skins.

### [Parallax System](papa)
![parallaxgif](https://user-images.githubusercontent.com/42221923/143954425-0ae79a5d-ee53-467f-b0f8-94c9fb8257c6.gif)

Got the cool background art? Awesome! Now it's time to make it move.

### [Bosses System](asds)
![bossgif](https://user-images.githubusercontent.com/42221923/143960503-736006f0-d402-423a-9169-dfc9cecdb9c7.gif)

Bosses were programmed to use the lane fighting system as a nice addition to the regular fights with Chompers.
