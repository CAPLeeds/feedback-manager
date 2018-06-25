using UnityEngine;
using System.Collections.Generic;

public class TabContinuous1Var : TabAxisGraph
{

    public string unitName = "";
	public string valueFormat = "*";
    public int graphNumSigFigs = 3;
	public int listNumSigFigs = 3;

    public float initialMinValue = 0f;
    public float initialMaxValue = 10f;

    public bool autoYAxis = true;
    public bool yHasMetricFormat = true;

    public bool limitGraphPointNum = false;
    public int maxGraphPointNum = 0;

	public List<float> dataValues;

	private GraphContinuous1Var graphScript;
    
	///<summary>Creates and initializes graph, sets all variables. Called in Tab Manager `Start()`</summary>
	public override void Init ()
    {
		// get graph and list
		base.Init ();

        // modify a variable to ensure correct range
        if (maxGraphPointNum < 3) maxGraphPointNum = 3;

		// add script component to graph and initialize with all variables,
        // including titles, units, formatting and appearance.
		graphScript = graph.AddComponent<GraphContinuous1Var>();

		graphScript.Init (
        this, unitName, valueFormat, graphNumSigFigs,
        GetComponent<TabTitleInEditor>().titleString,
        initialMinValue, initialMaxValue, 
        autoYAxis, yHasMetricFormat, 
        limitGraphPointNum, maxGraphPointNum, 
        hasPointConnectLine,
        graphPointColor, graphLineColor);

		// instantiate dataValue list
		dataValues = new List<float> ();
	}

    ///<summary>Adds a new value to the Tab, including Graph, Ticker List and dataValues.</summary>
    ///<param name = "newValue">The value to be added to the graph (displayed on y-axis)</param>
    public void AddSingleValue(float newValue)
    {
		dataValues.Add (newValue);
		graphScript.AddPoint (newValue);
        // set decimals in list value 
        string newListItem;
        if (yHasMetricFormat)
        {
            newListItem = MathScientific.ToMetric(newValue, valueFormat, listNumSigFigs);
        }
        else
        {
            newListItem = MathScientific.ToFormattedValue(newValue, valueFormat, listNumSigFigs);
        }
        tickerList.GetComponent<ListController> ().AddListDataItem (newListItem);
	}

    ///<summary>Clears all data in the Tab, Graph and TickerList.</summary>
    public override void Clear ()
    {
        dataValues.Clear();
        tickerList.GetComponent<ListController>().Clear();
        graphScript.Clear();
    }

}
