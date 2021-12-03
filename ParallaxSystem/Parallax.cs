using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour {

	public float order = 1;
	public float multiplier = 0.8f;
	public bool cloneOnly = false;
	public static float speed;
	protected float speedForSlowdown;
	public static bool slowdown = false;
	public static bool releaseSlowdown = false;
	protected bool currentSpeedSet = false;
	protected float initialSpeed;

	private GameObject img1;
	private RawImage imgRenderer;
	private float rectX = 0;
	private RawImage secondImgRenderer; 

	void Start(){
		if(!cloneOnly){
			img1 = transform.GetChild(0).gameObject;
			imgRenderer = img1.GetComponent<RawImage>();
		}

		GameObject secondParallax = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
		secondParallax.GetComponent<Parallax>().enabled = false;
		secondParallax.transform.SetParent(transform.parent, false);
		secondParallax.transform.SetSiblingIndex(transform.GetSiblingIndex()+1);
		secondParallax.GetComponent<RectTransform>().anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition + new Vector2(-gameObject.GetComponent<RectTransform>().rect.x*2, 0);
		if(secondParallax.transform.childCount > 0 && secondParallax.transform.GetChild(0).childCount > 0){
			secondParallax.transform.GetChild(0).GetComponent<ParticlesParallax>().enabled = false;
			foreach(Transform t in secondParallax.transform.GetChild(0))
				Destroy(t.gameObject);
		}
		secondImgRenderer = secondParallax.GetComponentInChildren<RawImage>();

		slowdown = false;
	}

	void Update(){
		if(imgRenderer == null || cloneOnly)
			return;
		if(slowdown && !releaseSlowdown){
			if(!currentSpeedSet){
				initialSpeed = speed;
				speedForSlowdown = speed;
				currentSpeedSet = true;
			}
			if(speedForSlowdown > 0){
				speedForSlowdown = Mathf.Clamp(speedForSlowdown -= Time.deltaTime*order, 0, initialSpeed);
			}
		}
		if(releaseSlowdown){
			if(speedForSlowdown < initialSpeed)
				speedForSlowdown = Mathf.Clamp(speedForSlowdown += Time.deltaTime*order, 0, initialSpeed);
			else{
				speedForSlowdown = initialSpeed;
				releaseSlowdown = false;
				currentSpeedSet = false;
				slowdown = false;
			}
		}
		if(slowdown || releaseSlowdown)
			rectX += ((multiplier / order) * speedForSlowdown * Time.deltaTime);
		else
			rectX += ((multiplier / order) * speed * Time.deltaTime);

		if(rectX > 1)
			rectX -= 1;
		imgRenderer.uvRect = new Rect(rectX, 0, 1, 1);
		secondImgRenderer.uvRect = new Rect(rectX, 0, 1, 1);
	}
}
