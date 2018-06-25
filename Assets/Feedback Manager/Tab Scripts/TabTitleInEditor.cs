using UnityEngine;

[ExecuteInEditMode]
public class TabTitleInEditor : MonoBehaviour {
	
	public string titleString = "[Tab Title]";

	public GameObject titleGO;

	// Use this for initialization
	void Awake () 
	{
		titleGO.GetComponent<TitleController> ().titleText.text = titleString;
	}

	void OnValidate() 
	{
		titleGO.GetComponent<TitleController> ().titleText.text = titleString;
	}

}
