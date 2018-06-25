using UnityEngine;
using System.Collections.Generic;

public class TabContinuous2Var : TabAxisGraph
{

	public string xAxisTitle = "";
	public string yAxisTitle = "";

    public string xUnitName = "";
    public string xValueFormat = "*";
    public string yUnitName = "";
    public string yValueFormat = "*";

	public Vector2Int graphNumSigFigs = new Vector2Int(3,3);
    public Vector2Int listNumSigFigs = new Vector2Int(3, 3);

    public Vector2 initialMinValues = new Vector2(0, 0);
    public Vector2 initialMaxValues = new Vector2(10, 10);

    public bool autoXAxis = true;
    public bool autoYAxis = true;

    public bool xHasMetricFormat = true;
    public bool yHasMetricFormat = true;
    public bool hasLineOfBestFit = true;
    public Color lineOfBestFitColor = Color.red;

    public List<Vector2> dataValues = new List<Vector2>();

    private GraphContinuous2Var graphScript;

    string GetListItem (float newXValue, float newYValue)
    {
        // set significant figures, optional metric unit prefix and formatting in list x and y values
        string xListItem;
        if (xHasMetricFormat) { xListItem = MathScientific.ToMetric(newXValue, xValueFormat, listNumSigFigs.x); }
        else { xListItem = MathScientific.ToFormattedValue(newXValue, xValueFormat, listNumSigFigs.x); }

        string yListItem;
        if (yHasMetricFormat) { yListItem = MathScientific.ToMetric(newYValue, yValueFormat, listNumSigFigs.y); }
        else { yListItem = MathScientific.ToFormattedValue(newYValue, yValueFormat, listNumSigFigs.y); }

        return "X: " + xListItem + ",\n\t Y: " + yListItem;
    }

    /* 
    PUBLIC METHODS 
    */

	// called in tab manager Start
	public override void Init ()
    {
		// gets graph and list
		base.Init ();

		// add script component to graph and modify variables
		graphScript = graph.AddComponent<GraphContinuous2Var>();

		graphScript.Init (
        this, GetComponent<TabTitleInEditor>().titleString,
        xAxisTitle, yAxisTitle, xUnitName, yUnitName, xValueFormat, yValueFormat,
        graphNumSigFigs, initialMinValues, initialMaxValues, autoXAxis, autoYAxis,
        xHasMetricFormat, yHasMetricFormat, hasPointConnectLine, hasLineOfBestFit, 
        lineOfBestFitColor, graphPointColor, graphLineColor);
	}

	///<summary>add new value to dataValues, graph, and tickerList</summary>
	public void AddPoint(float newXValue, float newYValue)
    {
        Vector2 newPoint = new Vector2(newXValue, newYValue);
        graphScript.AddPoint (newPoint);
        dataValues.Add(newPoint);

        string newListItem = GetListItem(newXValue, newYValue);
        tickerList.GetComponent<ListController> ().AddListDataItem (newListItem);
	}

    ///<summary>Clears all data in the tab, graph and list</summary>
    public override void Clear ()
    {
        dataValues.Clear();
        tickerList.GetComponent<ListController>().Clear();
        graphScript.Clear();
    }
}
