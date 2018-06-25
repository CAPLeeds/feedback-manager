using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ListController : MonoBehaviour {

    public Color textColor = Color.white;
	public GameObject ContentPanel;
	public GameObject ListItemPrefab;
	private int numItemsVisible;
	private int dataCounter = 0;

	// creates a new list item
	GameObject InstantiateListItem ()  {
		GameObject newItem = Instantiate (ListItemPrefab);
		newItem.transform.SetParent (ContentPanel.transform, false);
		newItem.transform.localRotation = Quaternion.identity;
        newItem.GetComponentInChildren<Text>().color = textColor;
		return newItem;
	}

	///<summary>creates initial list items (number depending on tab height)</summary>
	public void Init () {
		// find max number of list items on screen at once
		numItemsVisible = Mathf.FloorToInt(GetComponent<RectTransform>().rect.height / ListItemPrefab.GetComponent<RectTransform>().rect.height);
		//create number of blank list items to fit in the space. these will be re-used.
		for (int i = 0; i < numItemsVisible; ++i) {
			InstantiateListItem();
		}
	}

	///<summary>adds a new data value to the list</summary>
	public void AddListDataItem (string value) {
		// increase data item count by one
		dataCounter++;
		// get item at bottom of list, move it to top and set value to new data value
		Transform newItemTranf = ContentPanel.transform.GetChild (ContentPanel.transform.childCount - 1);
		newItemTranf.SetAsFirstSibling();
		ListItemController itemController = newItemTranf.GetComponent<ListItemController> ();
		itemController.dataText.text = MathScientific.ToMetric((float)dataCounter) + ") " + value;
	}

	/// <summary>clears all text in all list items and sets data count to zero</summary>
	public void Clear() 
	{
		dataCounter = 0;
		foreach (Transform itemTransf in ContentPanel.transform) 
		{
			ListItemController itemController = itemTransf.GetComponent<ListItemController>();
			itemController.dataText.text = "";
		}
	}

}
