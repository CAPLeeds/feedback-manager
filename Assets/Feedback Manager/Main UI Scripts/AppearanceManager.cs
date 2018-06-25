using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceManager : MonoBehaviour {

    public Color mainBackgroundColor = Color.black;
    public Color textColor = Color.white;
    public Color graphBackgroundColor = Color.black;
    public Color graphDetailColor = Color.white;

    public Color tabLabelOffBackColor = Color.gray;
    public Color tabLabelOffTextColor = Color.black;
    public Color tabLabelOnBackColor = Color.green;
    public Color tabLabelOnTextColor = Color.white;

    public Image background;
    public TabLabelBarController labelBarController;
    public Transform TabContainerTransf;
    public List<GameObject> feedbackTabs;

    // Gets a list of the top layer gameobjects in the Tab Container (should all be tabs)
    List<GameObject> GetTabGOList()
    {
        List<GameObject> tabGOList = new List<GameObject>();

        foreach (Transform t in TabContainerTransf)
        {
            tabGOList.Add(t.gameObject);
        }
        return tabGOList;
    }

    // update the colors whenever they are changed in the editor
    void OnValidate()
    {
        // ensure that container has been linked (may be called on package load-in before container linked)
        if (TabContainerTransf != null)
        {
            // set background color
            background.color = mainBackgroundColor;
            // get the list of tabs
            feedbackTabs = GetTabGOList();
            // edit for each feedback tab
            foreach (GameObject tab in feedbackTabs)
            {
                // text objects
                tab.GetComponent<TabTitleInEditor>().titleGO.GetComponent<TitleController>().titleText.color = textColor;
                tab.GetComponent<TabListInEditor>().tickerListGO.GetComponent<ListController>().textColor = textColor;
                // edit placeholder (this will be applied to graph in Graph class at runtime)
                PlaceholderController placeholder = tab.GetComponentInChildren<PlaceholderController>();
                placeholder.background.color = graphBackgroundColor;
                placeholder.text.color = graphDetailColor;
            }
            // formatting for tab labels
            labelBarController.offBackgroundColor = tabLabelOffBackColor;
            labelBarController.offTextColor = tabLabelOffTextColor;
            labelBarController.onBackgroundColor = tabLabelOnBackColor;
            labelBarController.onTextColor = tabLabelOnTextColor;
        }
    }
}
