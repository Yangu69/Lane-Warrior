using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnap : MonoBehaviour{

	private ScrollRect scroll;
	private RectTransform content;
	public Transform container;
	public AudioSource sfxSource;
	public AudioClip selectSound;
	private int screens = 0;
	private int startIndex = 0;
	public int index = 0;
	public ShopManager shopManager;
	public float snapSpeed = 6f;

	public bool lerp = false;

	private float[] points;
	public int target;

	private float centerPos;
	private bool justInitialized = false;

	void OnEnable(){
		scroll = gameObject.GetComponent<ScrollRect>();
		content = scroll.content;
		Initialize();
	}


	public void Initialize(){
		if(gameObject.activeInHierarchy == true)
			StartCoroutine(InitializeCoroutine());
	}
	private IEnumerator InitializeCoroutine(){
		yield return new WaitForEndOfFrame();
		screens = content.childCount;
		points = new float[screens];

		for(int i=0; i<screens;i++){
			points[i] = i/((float)screens-1);
		}
		// Enable blur for 1st index
		if(content.GetChild(0).childCount > 0)
			content.GetChild(0).GetChild(0).gameObject.SetActive(true);
		centerPos = scroll.viewport.transform.position.x;
		ScaleContent();
	}

	void Update(){
		if(lerp){
			scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, points[target], Time.deltaTime * snapSpeed);
			if(Mathf.Approximately(scroll.horizontalNormalizedPosition, points[target])){
				lerp = false;
			}
			ScaleContent();
		}
	}

	void MoveSkin(Vector2 m){
        if(justInitialized){
            justInitialized = false;
            return;
        }
		if(m.x < -0.5){
			if(index > 0){
				startIndex = index;
				target--;
				index--;
				lerp = true;
				sfxSource.PlayOneShot(selectSound);
				UpdateSkinInfo();
			}
		}else if (m.x > 0.5){
			if(index < points.Length-1){
				startIndex = index;
				target++;
				index++;
				lerp = true;
				sfxSource.PlayOneShot(selectSound);
				UpdateSkinInfo();
			}
		}
	}

	public void OnBeginDrag(){
		startIndex = index;
		if(content.GetChild(index).childCount > 0)
			content.GetChild(index).GetChild(0).gameObject.SetActive(false);

	}

	public void OnDrag(){
		lerp = false;
		ScaleContent();
		
	}

	public void OnEndDrag(){
		// Snap to the nearest skin when taking the finger off the screen
		target = FindNearestIndex(scroll.horizontalNormalizedPosition);
		lerp = true;
		if(content.GetChild(target).childCount > 0)
			content.GetChild(target).GetChild(0).gameObject.SetActive(true);
		UpdateSkinInfo();	
	}

	public void UpdateSkinInfo(){
		// Update skin name text and button for the selected one
		if(startIndex != index){
			if(content.GetChild(target).gameObject.GetComponent<LockedSkin>())
				if(!content.GetChild(target).gameObject.GetComponent<LockedSkin>().chain.activeSelf)
					content.GetChild(target).gameObject.GetComponent<LockedSkin>().chain.SetActive(true);
			shopManager.ReloadText(content.GetChild(target).gameObject);
			shopManager.ReloadButton(content.GetChild(target).gameObject);

		}
	}

	int FindNearestIndex(float f){
		float distance = Mathf.Infinity;
		int output = 0;

		for(int i=0; i<points.Length; i++){
			if(Mathf.Abs(points[i]-f) < distance){
				distance = Mathf.Abs(points[i]-f);
				output = i;
			}else{
				break;
			}
		}
		index = output;
		return output;
	}

	void ScaleContent(){
		// Scale skins around the center
		foreach(Transform t in content){
			RectTransform rect = t.GetComponent<RectTransform>();
			float newScale;
			if(t.position.x<centerPos){
				newScale = Mathf.Clamp(t.position.x/(centerPos * container.localScale.x), 0.6f, 1);
			}
			else
				newScale = Mathf.Clamp((centerPos * container.localScale.x)/t.position.x, 0.6f, 1);
			rect.localScale = new Vector3(newScale,newScale,0);
		}
	}

	public void ButtonAction(){
		shopManager.ButtonAction(index);
	}
}
