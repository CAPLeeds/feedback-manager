using UnityEngine;
using System.Collections.Generic;

public class TabManager : MonoBehaviour
{

	public bool tabCycleOn = true;
	public int secondsTabVisible = 10;

	// array of tabs
    [HideInInspector]
	public List<TabGraph> feedbackTabs;

	public GameObject tabLabelPrefab;

	private List<GameObject> tabLabelList = new List<GameObject> ();

	private GameObject tabLabelContainer;

	private int currentTabIndex;

    // The only feedback element to use Start
    // all tab elements are intitalized here using Init
    void Start ()
    {
        // Get feedback tabs
        feedbackTabs = new List<TabGraph>(transform.GetComponentsInChildren<TabGraph>());

		// initialize tabs
		foreach (TabGraph tab in feedbackTabs) {
            tab.Init();
		}

		// add tab labels
		PopulateTabLabels ();

        // set current tab
        currentTabIndex = 0;
		// setup for multiple tabs if applicable
		if (feedbackTabs.Count > 1) {
			MultiTabInit ();
		// if only one tab, ensure tab is visible
		} else if (feedbackTabs.Count == 1) {
			ShowCurrentTab ();
		} // if no tabs, do nothing
    }

    // management for multiple tabs
    void MultiTabInit ()
    {
		HideAllTabs ();
		ShowCurrentTab ();
		// begin cycling if applicable (tab cycle, and time is valid)
		if (tabCycleOn && (secondsTabVisible > 0) ) {
            InvokeRepeating("NextTab", secondsTabVisible, secondsTabVisible);
		}

	}

	void PopulateTabLabels ()
    {
		tabLabelContainer = GetComponentInChildren<TabLabelBarController>().container;
		foreach (TabGraph tab in feedbackTabs) {
			GameObject tabLabel = Instantiate (tabLabelPrefab).gameObject;
			tabLabelList.Add (tabLabel);
			tabLabel.transform.SetParent (tabLabelContainer.transform, false);
			tabLabel.GetComponent<TabLabelController> ().Init ();
			tabLabel.GetComponent<TabLabelController> ().tabTitle.text = tab.GetComponent<TabTitleInEditor> ().titleString;
		}
	}

	// sets tab to next tab in list, or to first tab if already at end
	// recursive function: also invokes itself
	void NextTab ()
    {
		// Debug.Log ("Next Tab!");
		HideCurrentTab ();
		// update current tab index
		if (currentTabIndex < (feedbackTabs.Count - 1)) {
			currentTabIndex++;
		} else {
			currentTabIndex = 0;
		}
		ShowCurrentTab ();
	}

	// hide all tabs
	void HideAllTabs ()
    {
		foreach (TabGraph tab in feedbackTabs) {
			tab.GetComponent<CanvasGroup>().alpha = 0;
		}
	}

	// show tab at current tab index
	void ShowCurrentTab ()
    {
		// Debug.Log (tabLabelList[currentTabIndex]);
		feedbackTabs [currentTabIndex].GetComponent<CanvasGroup>().alpha = 1;
		tabLabelList [currentTabIndex].GetComponent<TabLabelController> ().TurnOn ();
	}

	// hide tab at current tab index
	void HideCurrentTab ()
    {
		feedbackTabs [currentTabIndex].GetComponent<CanvasGroup>().alpha = 0;
		tabLabelList [currentTabIndex].GetComponent<TabLabelController> ().TurnOff ();

	}
				
	public GameObject CurrentTab { get { return feedbackTabs[currentTabIndex].gameObject; }}

}
