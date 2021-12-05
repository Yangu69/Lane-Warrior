using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;

public class BossLavaland : Boss
{

    public Material fadeOffMaterial;
    public GameObject attackingPart;
    public GameObject fist;
    public GameObject head;
    public GameObject cloneParticles;
    public GameObject substitutionParticles;
    public int fistAttacks;
    int fistsLeft;
    public int phaseThreeAttacksNeeded;
    int phaseThreeAttacksLeft;
    bool attacked = false;
    bool initialAttack = true;

    int headsAttacked = 0;

    new public void Start(){
        base.Start();
    }

    override public void Spawn(){
        StartCoroutine(SpawnFull());
    }

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

    void InitialSpawn(){
        // Instantiate head
        SpawnHead(1, 0.2f, true);

        // Instantiate fists
        SpawnFist(0, 0.3f);
        SpawnFist(2, 0.3f);

        fistsLeft = fistAttacks;
        PhaseOne();
    }

    private void SpawnFist(int i, float delay){
        parts[i] = Instantiate(fist, Vector3.zero, Quaternion.identity, GameManager.Instance.lanes[i].transform);
        Lane targetLane = GameManager.Instance.lanes[i].GetComponent<Lane>();
        parts[i].GetComponent<Enemy>().associatedLane = targetLane;
        targetLane.currentEnemies.Add(parts[i]);
        StartCoroutine(SpawnLerp(parts[i], GameManager.Instance.bossSpawn.position.x, delay));

    }

    private void SpawnHead(int i, float delay, bool lerp){
        parts[i] = Instantiate(head, Vector3.zero, Quaternion.identity, GameManager.Instance.lanes[i].transform);
        parts[i].GetComponent<Enemy>().associatedLane = GameManager.Instance.lanes[i].GetComponent<Lane>();
        GameManager.Instance.lanes[i].GetComponent<Lane>().currentEnemies.Add(parts[i]);
        if(lerp){
            StartCoroutine(SpawnLerp(parts[i], GameManager.Instance.bossSpawn.position.x, delay));
        }else{
            StartCoroutine(SpawnPart(parts[i], GameManager.Instance.bossSpawn.position.x, delay));
        }
    }

    private void SpawnParticles(GameObject part, GameObject particles, bool detached){

        GameObject par = Instantiate(particles, part.transform.position, Quaternion.Euler(-90, 0, 0));
        if(!detached)
            par.transform.SetParent(part.transform, false);
        else
            par.transform.position = part.transform.position;
    }

    public override void Reset()
    {
        fistsLeft = fistAttacks;
        InitialSpawn();
        
    }
    override public void PhaseOne(){
        if(dead)
            return;
        if(parts[1] == null)
            SpawnHead(1, 0, true);
        StartCoroutine(Attack());
    }

    public void PhaseTwo(){
        if(dead || GameManager.Instance.end)
            return;
        StartCoroutine(HeadAttack());
    }

    public void PhaseThree(){
        if(dead)
            return;
        phaseThreeAttacksLeft = phaseThreeAttacksNeeded;
        
    }

    public override void PartAttacked(GameObject part){
        //StartCoroutine(TurnRed(part));
        attacked = true;
        if(!part.GetComponent<EnemyLavalandHead>().toHit){
            //If wrong part was attacked
            part.GetComponent<Enemy>().Attack();
            foreach(GameObject p in parts){
                p.GetComponent<Enemy>().Die(false);
                p.GetComponent<Enemy>().associatedLane.currentEnemies.Remove(p);
            }
            attacked = false;
            PhaseTwo();
        }else{
            //If correct part was attacked
            Enemy partEnm = part.GetComponent<Enemy>();
            partEnm.enabled = false;
            foreach(GameObject p in parts){
                if(p != part){
                    p.GetComponent<Enemy>().Die(false);
                    p.GetComponent<Enemy>().associatedLane.currentEnemies.Remove(p);
                }
            }
            AnimateHead(part, "Head_Vulnerable", true, false);
            part.transform.position = partEnm.associatedLane.attackRange.transform.position;
            partEnm.associatedLane.currentEnemies.Add(part);
            attacked = false;
            PhaseThree();
        }
    }

    public void HeadDied(){
        if(attacked)
            return;

        if(++headsAttacked == 3){
            headsAttacked = 0;
            PhaseTwo();
        }
    }

    public override void GetHit(GameObject part)
    {
        if(dead)
            return;
        
        if(--phaseThreeAttacksLeft <= 0){
            part.GetComponent<Enemy>().associatedLane.currentEnemies.Remove(part);
            if(--health <= 0){
                StartCoroutine(Die(part));
            }else{
                SpawnParticles(part, cloneParticles, true);
                SpawnParticles(part, substitutionParticles, true);
                part.GetComponent<Enemy>().Die(false);
                fistsLeft = fistAttacks;
                SpawnFist(0, 0.3f);
                SpawnFist(2, 0.3f);
                PhaseOne();
            }
        }
    }

    protected IEnumerator Die(GameObject lastAttackedPart){
        dead = true;
        AnimateHead(lastAttackedPart, "Head_Dead", false, false);
        GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("Win");
        GPSManager.Instance.UnlockAchievement("Stage 2");
        if(!SaveManager.Instance.saveData.unlockedLevels.Contains("Bamboo Forest"))
            GPSManager.Instance.IncrementEvent("Boss 2 Defeated", 1);
        yield return new WaitForSeconds(3f);
        TapPhaseInit();
    }

    public override void Enrage(GameObject part){
        
    }

    public void AnimateFist(GameObject obj, string animationName, bool loop, bool returnToIdle){
        obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, animationName, loop);
        if(returnToIdle){
            obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, "Spike_Idle", true, 0);
        }
    }
    public void AnimateHead(GameObject obj, string animationName, bool loop, bool returnToIdle){
        obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, animationName, loop);
        if(returnToIdle){
            obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, "Head_Idle", true, 0);
        }
    }
    public void AnimateHeadAdd(GameObject obj, string animationName, bool loop, bool returnToIdle){
        obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, animationName, loop, 0);
        if(returnToIdle){
            obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, "Head_Idle", true, 0);
        }
    }

    IEnumerator Attack(){
        if(GameManager.Instance.end)
            yield break;
        if(fistsLeft-- > 0){
            if(initialAttack){
                initialAttack = false;
                yield return new WaitForSeconds(1f);
            }else{
                yield return new WaitForSeconds(0.4f);
            }
            // Choose fist
            int b = Random.Range(0,2);
            if(b == 1){
                b++;
            }
            attackingPart = parts[b];
            if(attackingPart != null){
                SkeletonGraphic sg = attackingPart.GetComponentInChildren<SkeletonGraphic>();
                parts[b].GetComponent<RandomFloat>().PauseFloat();
                AnimateFist(parts[b], "Spike_Anticipation", true, false);

                int blinks = Random.Range(3, 7);
                for (int i = 0; i < blinks; i++){
                    yield return StartCoroutine(Blink(attackingPart, 0.2f));
                }
                attackingPart.GetComponent<EnemySniping>().enabled = true;
                AnimateFist(parts[b], "Spike_Attack", true, false);
                SpawnFist(b, 0.2f);
                PhaseOne();
            }
        }else{
            PhaseTwo();
        }
    }

    IEnumerator HeadAttack(){
        yield return new WaitForSeconds(2f);
        // Destroy Fists
        if(parts[0] != null){
            parts[0].GetComponent<Enemy>().Die(false);
            parts[0].GetComponent<Enemy>().associatedLane.currentEnemies.Remove(parts[0]);
        }
        if(parts[2] != null){
            parts[2].GetComponent<Enemy>().Die(false);
            parts[2].GetComponent<Enemy>().associatedLane.currentEnemies.Remove(parts[2]);
        }

        // Spawn Heads
        SpawnHead(0, 0, false);
        if(parts[1] == null)
            SpawnHead(1, 0, false);
        SpawnHead(2, 0, false);
        foreach(GameObject part in parts){
            SpawnParticles(part, cloneParticles, false);
        }

        yield return new WaitForSeconds(1.2f);
        foreach(GameObject part in parts){
            AnimateHead(part, "Head_AttackAnticipation", false, false);
        }
        yield return new WaitForSeconds(1.83f);
        int toHit = Random.Range(0,3);

        for(int i=0;i < parts.Length; i++){
            EnemyLavalandHead enm = parts[i].GetComponent<EnemyLavalandHead>();
            if(i == toHit)
                enm.toHit = true;
            parts[i].GetComponent<RandomFloat>().PauseFloat();
            
            enm.enabled = true;
            AnimateHead(parts[i], "Head_AttackLoop", true, false);
        }
    }

    private IEnumerator Blink (GameObject part, float duration){
        // Make the part glow repeatedly
        SkeletonGraphic sg = part.GetComponentInChildren<SkeletonGraphic>();
        Material originalMaterial = sg.material;
        sg.material = blinkMaterial;

        RectTransform rt = part.GetComponent<RectTransform>();
        float xChange = 0.2f;

        float t=0;
        while (t < duration/2){
            if(sg == null)
                yield break;
            sg.material.SetFloat("_Glow",Mathf.Lerp(0, 1, duration/2/t));
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + xChange, 0);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t < duration/2){
            if(sg == null)
                yield break;
            sg.material.SetFloat("_Glow",Mathf.Lerp(1, 0, duration/2/t));
            t += Time.deltaTime;
            yield return null;
        }
        if(sg == null)
                yield break;
        sg.material = originalMaterial;
    }

    public IEnumerator FadeOff (GameObject part, float duration){
        // Fade out fake heads
        SkeletonGraphic sg = part.GetComponentInChildren<SkeletonGraphic>();
        Image indicatorImg = part.GetComponent<Enemy>().indicator.GetComponent<Image>();
        sg.material = fadeOffMaterial;
        Color startColor = sg.color;
        Color targetColor = new Color(1, 1, 1, 0.2f);
        Color indicatorTargetColor = new Color(1,1,1,0);

        float t = duration;
        while(t >= 0){
            sg.color = Color.Lerp(targetColor, startColor, t/duration);
            indicatorImg.color = Color.Lerp(indicatorTargetColor, startColor, t/duration/3);
            sg.material.SetFloat("_FadeAmount",Mathf.Lerp(1, 0, t/duration));
            t-= Time.deltaTime;
            yield return null;
        }
        sg.color = targetColor;

    }
}
