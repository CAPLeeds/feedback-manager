using UnityEngine;
using System.Collections.Generic;

public class TabCategorical : TabGraph
{
	public List<string> categories;

    public List<Color> colors;

	private GraphPieChart graphScript;

    // categorical graph values.
    public List<float> Values {
        get {return graphScript.graph.sliceValues.list;}
    }

	///<summary>Initialize graph, graph-script and list. Called in tab manager Start.</summary>
	public override void Init ()
    {
		// gets graph and ticker-list GameObjects, initializes ticker-list
		base.Init ();
		// add script component to graph and initialize script
		graphScript = graph.AddComponent<GraphPieChart>();
        graphScript.Init (categories, colors);
    }

    ///<summary>add category value to graph and category to list</summary>
    public void AddToCategory(string category, int value = 1)
    {
        // ensure updates only when active, to avoid null references
        if (gameObject.activeSelf) {
            
            graphScript.AddToCategory(category, (float)(value));
            tickerList.GetComponent<ListController>().AddListDataItem(category);
        }
	}

    ///<summary>Clears all data from the tab (including graph and list)</summary>
    public override void Clear ()
    {
        tickerList.GetComponent<ListController>().Clear();
        graphScript.Clear();
    }
}
