using UnityEngine;
using UnityEngine.UI;

// handles rect transform formatting for graph, access via graphFormatting
public class GraphPositioning
{
    public RectTransform graphrt;

    public Vector2 AnchorMin
    {
        get { return graphrt.anchorMin; }
        set { graphrt.anchorMin = value; }
    }
    public Vector2 AnchorMax
    {
        get { return graphrt.anchorMax; }
        set { graphrt.anchorMax = value; }
    }
    public Vector2 AnchoredPosition
    {
        get { return graphrt.anchoredPosition; }
        set { graphrt.anchoredPosition = value; }
    }
    public Vector2 OffsetMin
    {
        get { return graphrt.offsetMin; }
        set { graphrt.offsetMin = value; }
    }
    public Vector2 OffsetMax
    {
        get { return graphrt.offsetMax; }
        set { graphrt.offsetMax = value; }
    }

    // constructor method.
    public GraphPositioning(WMG_Graph_Manager graph) 
    { 
        graphrt = graph.GetComponent<RectTransform>();
    }

    // sets all relevant graph recttransform variables (i.e. graph position)
    public void SetAllVariables(
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, 
        Vector2 offsetMin, Vector2 offsetMax) 
    {
        this.AnchorMin = anchorMin;
        this.AnchorMax = anchorMax;
        this.AnchoredPosition = anchoredPosition;
        this.OffsetMin = offsetMin;
        this.OffsetMax = offsetMax;
    }
}

public class GraphAppearance
{
    public virtual void SetAllVariables (Color backgroundColor, Color detailColor) {}
}

public class Graph : MonoBehaviour 
{
	public GameObject emptyGraphPrefab;

	public string title = "[title]";

    public void SetTitle(string newTitle) 
    {
		title = newTitle;
	}

	public void SetEmptyGraphPrefab(string prefabPath)
    {
        // instantiate prefab for script
        emptyGraphPrefab = GetPrefab(prefabPath);
	}

    public GameObject GetPrefab(string prefabPath)
    {
        return Resources.Load(prefabPath) as GameObject;
    }

    // sets the placeholder transform to the graph, then destroys the placeholder
	public void SetPlaceAndDestroyHolder (GraphPositioning positioning, GraphAppearance appearance) {
        // find placeholder
        PlaceholderController placeholder = transform.GetComponentInChildren<PlaceholderController>();
        // assign offsets as placeholder offsets
        positioning.SetAllVariables(
            anchorMin: new Vector2(0, 0), anchorMax: new Vector2(1, 1), 
            anchoredPosition: new Vector2(0.5f, 0.5f),
            offsetMin: placeholder.GetOffsetMin(), offsetMax: placeholder.GetOffsetMax());
        // apply placeholder colors to graph appearance
        appearance.SetAllVariables(
            backgroundColor: placeholder.background.color,
            detailColor: placeholder.text.color);
		// remove placeholder
		GameObject.Destroy (placeholder.gameObject);
	}

    public GameObject InstantiateGraph () {
		GameObject graphGO = GameObject.Instantiate (emptyGraphPrefab);
		graphGO.transform.SetParent (transform, false);
		return graphGO;
	}

}
