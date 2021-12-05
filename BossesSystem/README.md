# Bosses System
![bossgif2](https://user-images.githubusercontent.com/42221923/144754909-1d0a3197-2f11-4117-871b-7a0ee44d5f6b.gif)

Bosses were created to add some additional mechanics to the lane system. Each stage in story mode features a boss that appears after a certain amount of enemies get defeated. Defeating the boss is needed to unlock the next stage.

## How it works
Boss script is used as a base for common methods and variables that all bosses are using. Each actual boss has an individual script created that inherits from the Boss script.

The scripts become active after player defeats the needed amount of enemies. It starts with the sequence of showing the appearing boss.

![bossgif](https://user-images.githubusercontent.com/42221923/144755626-4c9ff481-dc5c-4e22-b997-bfcedd10e5fd.gif)

The sequence involves hiding the lanes, stopping the background, and showing the full size boss with animations.
```c#
IEnumerator SpawnFull(){
    GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("HideLanes");
    Parallax.slowdown = true;
    fullBoss.SetActive(true);
    yield return new WaitForSeconds(3f);
    GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("ShowLanes");
    Parallax.releaseSlowdown = true;
    yield return new WaitForSeconds(1.5f);
    InitialSpawn();
}
```

Now that the boss presented itself to the player, it spawns its parts on the lanes, with custom mechanics responsible for these parts. For example, the first boss sends projectiles to the player. Player needs to get a GREAT in order to hijack the projectile which then can be sent back to the boss. The phase where player can shoot starts after the boss shoots certain amount of projectiles.

```c#
override public void DestroyedBullet(){
    bulletsLeft--;
    if(bulletsLeft <= 0)
        ShootPhase();
}

void ShootPhase(){
    if(GameData.Instance)
        GameData.Instance.ResetQuestProgress(QuestType.GetProjectilesInOnePhase);
    CancelInvoke("Shoot");
    if(dead || GameManager.Instance.end)
        return;
    ShowNotification("Shoot!", "Strzelaj!", "Schiet!");
    bulletsLeft = bulletsToShoot;

    // Give 1 projectile if player didn't manage to get any
    if(GameManager.Instance.CollectedProjectiles == 0)
        GameManager.Instance.CollectedProjectiles = 1;
    StartCoroutine(ActivePart());
}
```
