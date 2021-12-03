using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Spine.Unity.Editor;
using Spine.Unity;
using Spine;

[CustomEditor(typeof(SkinObject))]
public class SkinObjectEditor : Editor {

	SkinObject obj;
	List<string> skinNames = new List<string>();
	PreviewRenderUtility preview;
	GameObject previewObject;
	Vector3 centerPosition;
	int previewLayer;

	public void OnEnable(){
		obj = (SkinObject)target;
		SetupPreview();
		SetupObject();
	}

	void OnDisable ()
    {
        preview.Cleanup();
        preview = null;
		if(previewObject != null)
        	DestroyImmediate(previewObject);
		if(obj.characterName == "")
			obj.characterName = target.name;
    }

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField("Select character's skeleton:", EditorStyles.boldLabel);
		obj.skeletonData = (SkeletonDataAsset)EditorGUILayout.ObjectField(obj.skeletonData, typeof(SkeletonDataAsset), false);
		if(EditorGUI.EndChangeCheck() || obj.skeletonData != null){
			var _skins = obj.skeletonData.GetSkeletonData(true).Skins.ToArray();
			for(int i=0;i<_skins.Length;i++){
				skinNames.Add(_skins[i].Name);
			}
		}

		if(obj.skeletonData != null){
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.LabelField("Choose skin of that skeleton:");
			obj.index = EditorGUILayout.Popup(obj.index, skinNames.ToArray(), EditorStyles.popup);
			if(EditorGUI.EndChangeCheck()){
				obj.skeletonSkin = skinNames.ToArray()[obj.index];
				SetupObject();
				//SetupPreview();
			}
		}
	}

	void SetupPreview(){
		var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var propInfo = typeof(Camera).GetProperty ("PreviewCullingLayer", flags);
        previewLayer = (int)propInfo.GetValue (null, new object[0]);
		preview = new PreviewRenderUtility(false);
		preview.camera.fieldOfView = 50;
		preview.camera.cullingMask = 1 << previewLayer;
	}

	void SetupObject(){
		if(obj.skeletonData == null)
			return;
		DestroyImmediate(previewObject);

		//Creating Skeleton Animation for preview and assigning current object's data and skin to it.
		previewObject = EditorInstantiation.InstantiateSkeletonAnimation(obj.skeletonData, obj.skeletonSkin, false, false).gameObject;
		previewObject.hideFlags = HideFlags.HideAndDontSave; //Hide in scene
		SkeletonAnimation skel = previewObject.GetComponent<SkeletonAnimation>();
		skel.skeletonDataAsset = obj.skeletonData;
		skel.initialSkinName = obj.skeletonSkin;
		skel.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		skel.LateUpdate();
		previewObject.layer = previewLayer; //Changing layer for object and it's children to be seen only by preview camera
		foreach(Transform t in previewObject.transform)
			t.gameObject.layer = previewLayer;
		
		preview.AddSingleGO(previewObject);

		previewObject.GetComponent<Renderer>().enabled = false;
	}

	//Let editor know if you want preview
	public override bool HasPreviewGUI(){
		return obj.skeletonData != null;
	}

	//How preview should be handled
	public override void OnPreviewGUI (Rect r, GUIStyle background)
    {
		if(previewObject == null)
			return;
        var previewCamera = preview.camera;

        previewCamera.transform.position =
            previewObject.transform.position + Vector3.forward * -8 + Vector3.up * 2;
		previewCamera.transform.rotation = Quaternion.identity;

        preview.BeginPreview (r, background);

		previewObject.GetComponent<Renderer>().enabled = true;
        previewCamera.Render();

        preview.EndAndDrawPreview (r);

		previewObject.GetComponent<Renderer>().enabled = false ;
	}
}
