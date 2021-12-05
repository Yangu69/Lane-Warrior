using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Spine.Unity;

public class Boss : MonoBehaviour
{
    [Header("Full Boss")]
    public GameObject fullBoss;
    public GameObject tapArea;
    public GameObject tapIcon;
    public GameObject menacingEyes;
    private Animator fullBossAnim;
    private int hits;
    private int hitAnimation;
    public GameObject zoomCanvas;
    public Text hitsText;
    private Animator hitsTextAnim;
    public ParticleSystem bossTapParticles;

    [Header("Audio")]
    public AudioSource mainAudioSrc;
    public AudioSource sfxAudioSrc;
    private AudioSource fullBossAudioSrc;
    public AudioClip bossBGM;
    public AudioClip interlude;
    public AudioClip hitSound;
    public AudioClip[] hitSounds;
    public AudioClip shieldHit;
    public AudioClip roar;

    [Header("Boss Stats")]
    public int health = 10;
    public int money = 60;
    public bool spawned = false;
    public bool dead = false;
    public int phase = 1;
    public int bulletsToShoot;
    public int bulletsLeft = 10;
    public GameObject notification;
    public Text notificationText;

    [Header("Parts - Top/Mid/Bottom")]
    public GameObject[] parts;
    public GameObject vulnerablePart;
    public GameObject hitParticles;
    public GameObject hitShieldParticles;
    public Material blinkMaterial;

    public void Start(){
        if(GameManager.Instance.end)
            return;
        spawned = true;
        fullBossAnim = fullBoss.GetComponentInChildren<Animator>();
        fullBossAudioSrc = fullBoss.GetComponentInChildren<AudioSource>();
        fullBossAudioSrc.enabled = false;
        hitsTextAnim = hitsText.GetComponent<Animator>();
        AudioManager.Instance.CrossfadeAudio(mainAudioSrc, bossBGM, 1.7f);
        Spawn();
    }

    public void StopCoroutines(){
        StopAllCoroutines();
    }
    
    public virtual void Spawn(){}

    public virtual void DestroyedBullet(){}
    public virtual void Reset(){}
    public virtual void PhaseOne(){}

    public virtual void GetHit(GameObject part){
        StartCoroutine(TurnRed(part));
        health--;
        if(health <= 0){
            StartCoroutine(Die());
        }
        sfxAudioSrc.PlayOneShot(hitSounds[Random.Range(0,hitSounds.Length)]);
    }
    public void DealDamage(){
        Hero.Instance.healthbar.ChangeHealth(-1);
    }

    public virtual void PartAttacked(GameObject part){}
    public virtual void Enrage(GameObject part){}

    public void TapPhaseInit(){
        StartCoroutine(TapPhase());
    }

    protected IEnumerator TapPhase(){
        // Show the boss
        Parallax.slowdown = true;
        fullBossAnim.SetTrigger("WeakEntry");
        GameManager.Instance.won = true;
        yield return new WaitForSeconds(3f);

        // Start the tapping sequence
        menacingEyes.SetActive(true);
        yield return new WaitForSeconds(2f);
        UICameraFocus.Instance.Focus(this.gameObject, 4, 10, 5f);
        tapArea.SetActive(true);
        tapIcon.SetActive(true);
        zoomCanvas.SetActive(true);
        yield return new WaitForSeconds(5f);

        // Stop the tapping sequence and make the boss disappear
        Hero.Instance.skeleton.AnimationState.AddAnimation(0,"Idle", true, 0.3f);
        tapArea.SetActive(false);
        tapIcon.SetActive(false);
        yield return new WaitForSeconds(1f);
        fullBossAnim.SetTrigger("Disappear");
        hitsTextAnim.SetTrigger("Center");
    }

    public void Tap(){
        // Play animation and add a hit;
        int generatedNumber;
        do
            generatedNumber = Random.Range(1,4);
        while(generatedNumber == hitAnimation);
        hitAnimation = generatedNumber;
        fullBossAnim.SetTrigger("Hit" + hitAnimation);
        hitsText.text = "HITS: " + ++hits;
        hitsTextAnim.SetTrigger("Add");
        AttackAnimation(generatedNumber);
        sfxAudioSrc.PlayOneShot(hitSounds[generatedNumber-1]);
        bossTapParticles.Emit(Random.Range(80,101));
    }

    void AttackAnimation(int num){
        string laneType = "Down";
        switch(num){
            case 1:
                laneType = "Down";
                break;
            case 2:
                laneType = "Top";
                break;
            case 3:
                laneType = "Mid";
                break;
        }
        Hero.Instance.skeleton.AnimationState.SetAnimation(0,laneType+"Attack1",false);
    }

    protected virtual IEnumerator Die(){
        dead = true;
        foreach(GameObject part in parts){
            AnimatePart(part, part.GetComponentInParent<Lane>().altLaneType, part.GetComponentInParent<Lane>().altLaneType + "_Death", false, false);
        }
        GameManager.Instance.collectedProjectilesCount.SetActive(false);
        GameManager.Instance.gameScreen.GetComponent<Animator>().SetTrigger("Win");
        yield return new WaitForSeconds(3f);
        TapPhaseInit();
    }

    protected IEnumerator TurnRed(GameObject part){
        Color target = Color.red;
        Color startColor = Color.white;
        float t = 0;
        while(t < 1){
            part.GetComponentInChildren<SkeletonGraphic>().color = Color.Lerp(startColor, target, t);
            t += Time.deltaTime * 10;
            yield return new WaitForEndOfFrame();
        }
        t = 0;
        while(t < 1){
            part.GetComponentInChildren<SkeletonGraphic>().color = Color.Lerp(target, startColor, t);
            t += Time.deltaTime * 10;
            yield return new WaitForEndOfFrame();
        }
        part.GetComponentInChildren<SkeletonGraphic>().color = startColor;
    }
    
    public void AnimatePart(GameObject obj, string laneType, string animationName, bool loop, bool returnToIdle){
        obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, animationName, loop);
        if(returnToIdle){
            obj.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, laneType + "_Idle", true, 0);
        }
    }

    protected IEnumerator SpawnPart(GameObject part, float targetX, float delay){
        RectTransform rt = part.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(900,0);
        yield return new WaitForSeconds(delay);
        rt.transform.position = new Vector2(targetX, rt.transform.position.y);
    }

    protected IEnumerator SpawnLerp(GameObject part, float targetX, float delay){
        float t = 0;
        RectTransform rt = part.GetComponent<RectTransform>();
        Vector2 startPos = new Vector2(900, 0);
        rt.anchoredPosition = startPos;
        Vector2 startWorldPos = rt.transform.position;
        yield return new WaitForSeconds(delay);
        Vector2 newPos = new Vector2(targetX, startWorldPos.y);
        while (t < 1){
            if(rt == null)
                yield break;
            rt.transform.position = Vector2.Lerp(startWorldPos, newPos, t);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rt.transform.position = newPos;
    }
    
    protected void ShowNotification(string en, string pl, string nl){
        notification.SetActive(true);
        if(Application.systemLanguage == SystemLanguage.Polish)
            notificationText.text = pl;
        else if(Application.systemLanguage == SystemLanguage.Dutch)
            notificationText.text = nl;
        else
            notificationText.text = en;
    }   
}
