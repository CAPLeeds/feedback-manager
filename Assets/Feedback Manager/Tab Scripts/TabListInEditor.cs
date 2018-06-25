using UnityEngine;

[ExecuteInEditMode]
public class TabListInEditor : MonoBehaviour {

	public bool listVisibleInTab = true;

	public GameObject tickerListGO;

	public RectTransform tickerListRt;
	public RectTransform graphRt;
	public RectTransform titleRt;


	void OnValidate () {

		// do not run unless it has a transform parent
		if (transform.parent != null) {
			
			//RectTransform canvasRt = transform.parent.GetComponent<RectTransform> ();

			// toggle list visible or hidden
			if (listVisibleInTab) {
				tickerListGO.GetComponent<CanvasGroup> ().alpha = 1;
			} else {
				tickerListGO.GetComponent<CanvasGroup> ().alpha = 0;
			}

			// find new graph offset to accomodate for list
			float newOffsetRight;
			if (listVisibleInTab) {
				newOffsetRight = -(tickerListRt.sizeDelta.x);
			} else {
				newOffsetRight = 0f;
			}

			// set new graph and title right offset (width)
			graphRt.offsetMax = new Vector2 (newOffsetRight, graphRt.offsetMax.y);
			titleRt.offsetMax = new Vector2 (newOffsetRight, titleRt.offsetMax.y);
		}
	}
}
