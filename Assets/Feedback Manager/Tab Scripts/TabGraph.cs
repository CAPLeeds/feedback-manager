using UnityEngine;

public class TabGraph : MonoBehaviour {

    [HideInInspector]
	public GameObject graph;
    [HideInInspector]
	public GameObject tickerList;

    ///<summary>gets graph and ticker-list GameObjects, initializes ticker-list. Called in tab manager Start.</summary>
    public virtual void Init () {
		// get relelevant gameobject children
		TabListInEditor t = GetComponent<TabListInEditor>();
		graph = t.graphRt.gameObject;
		tickerList = t.tickerListRt.gameObject;
        // initialize ticker list
        tickerList.GetComponent<ListController>().Init();
    }

	public virtual void Clear () {}
}
