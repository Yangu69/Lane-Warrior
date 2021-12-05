using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class BossForest : Boss
{
    public GameObject projectile;
    public GameObject arrowSfx;
    private GameObject currentArrowSfx;
    public GameObject blockSfx;
    private int shotBullets = 0;
    public float posXOffset;
    private GameObject activePart;
    public bool activePartChanged = false;


    new public void Start(){
        base.Start();
    }
    override public void Spawn(){
        StartCoroutine(SpawnFull());
    }

    public override void Reset()
    {
        PhaseOne();
    }

    public override void PhaseOne(){
        if(dead)
            return;
        vulnerablePart = null;
        GameManager.Instance.shootPhase = false;
        ShowNotification("Watch out!", "Uwazaj!", "Pas op!");
        shotBullets = 0;
        if(GameManager.Instance.CollectedProjectiles != 0)
            GameManager.Instance.collectedProjectilesCount.SetActive(true);
        Invoke("Shoot", 1.5f);
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


    void Shoot(){
        if(dead || GameManager.Instance.end)
            return;
        if(GameManager.Instance.end){
            return;
        }
        GameManager.Instance.CreateProjectile(projectile);
        shotBullets++;
        if(shotBullets >= bulletsToShoot){
            shotBullets = 0;
        }else{
            Invoke("Shoot", 1f);
        }
    }

    override public void DestroyedBullet(){
        bulletsLeft--;
        if(bulletsLeft <= 0)
            ShootPhase();
    }

    override public void GetHit(GameObject part){
        if(dead)
            return;
        GameObject particles;
        if(part == vulnerablePart){
            StartCoroutine(TurnRed(part));
            health--;
            if(health <= 0){
                StartCoroutine(Die());
                GPSManager.Instance.UnlockAchievement("Stage 1");
                if(!SaveManager.Instance.saveData.unlockedLevels.Contains("Magma Valley"))
                    GPSManager.Instance.IncrementEvent("Boss 1 Defeated", 1);
            }else{
                activePart.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, activePart.GetComponentInParent<Lane>().altLaneType+"_Hit", true);
                activePart.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, activePart.GetComponentInParent<Lane>().altLaneType+"_Idle", true, 0);
                NewActivePart(true);
            }
            sfxAudioSrc.PlayOneShot(hitSound);   
            particles = Instantiate(hitParticles, part.transform);


            // Check HitBoss quest
            if(GameData.Instance)
                GameData.Instance.IncrementQuestProgress(QuestType.HitBossInRow);
            
        }else{
            if(GameData.Instance)
                GameData.Instance.ResetQuestProgress(QuestType.HitBossInRow);
            Instantiate(blockSfx, part.transform, false);
            sfxAudioSrc.PlayOneShot(shieldHit);
            particles = Instantiate(hitShieldParticles, part.transform);
        }
        Destroy(particles, 0.5f);
    }

    IEnumerator SpawnFull(){
        GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("HideLanes");
        Parallax.slowdown = true;
        fullBoss.SetActive(true);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("ShowLanes");
        Parallax.releaseSlowdown = true;
        yield return new WaitForSeconds(1.5f);
        for(int i=0; i<3; i++){
            parts[i] = Instantiate(parts[i], Vector3.zero, Quaternion.identity, GameManager.Instance.lanes[i].transform);
            StartCoroutine(SpawnLerp(parts[i], GameManager.Instance.bossSpawn.position.x, 0));
            GameManager.Instance.lanes[i].GetComponent<Lane>().bossPart = parts[i];
        }
        PhaseOne();
    }
    IEnumerator ActivePart(){
        float t = 0;
        bool endPhase = false;
        yield return new WaitForSeconds(1.15f);
        GameManager.Instance.shootPhase = true;
        while(t < 10){
            if(dead)
                break;
            float t2 = (Mathf.Abs(GameManager.Instance.lanes[0].GetComponent<Lane>().attackRange.transform.position.x - GameManager.Instance.bossSpawn.position.x)/(Parallax.speed * 24 * 1.3f)) + Random.Range(0.3f,0.7f);
            if(activePart)
                activePart.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, activePart.GetComponentInParent<Lane>().altLaneType+"_Idle", true);
            NewActivePart(false);
            activePart.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, activePart.GetComponentInParent<Lane>().altLaneType+"_Vulnerable", true);
            while(t2 > 0){
                t2 -= Time.deltaTime;
                t +=  Time.deltaTime;
                if(GameManager.Instance.CollectedProjectiles == 0){
                    yield return new WaitForSecondsRealtime(2f);
                    endPhase = true;
                    break;
                }
                if(activePartChanged){
                    activePartChanged = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if(endPhase)
                break;
            yield return new WaitForEndOfFrame();
        }
        if(!dead)
            activePart.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, activePart.GetComponentInParent<Lane>().altLaneType+"_Idle", true);
        Destroy(currentArrowSfx);
        vulnerablePart = null;
        PhaseOne();
    }
    
    public void NewActivePart(bool gotHit){
        if(dead)
            return;
        activePart = parts[Random.Range(0, parts.Length)];
        vulnerablePart = activePart;

        // Instantiate Arrow SFX leading to the active part
        if(currentArrowSfx)
            Destroy(currentArrowSfx);
        currentArrowSfx = Instantiate(arrowSfx, activePart.transform.parent, false);


        if(gotHit)
            activePartChanged = true;
    }
}
