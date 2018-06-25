using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScatterGraphFormatting:AxisGraphFormatting
{
    public void SetForScatterGraph(
    string xAxisTitle, string yAxisTitle, Vector2 minValues, Vector2 maxValues, int xTickNum, int yTickNum, bool hasLineOfBestFit)
    {
        xAxis.SetAllVariables(title: xAxisTitle, minValue: minValues.x, maxValue: maxValues.x, numTicks:xTickNum, titleOffset:new Vector2(0, 70));
        yAxis.SetAllVariables(title: yAxisTitle, minValue: minValues.y, maxValue: maxValues.y, numTicks:yTickNum);
        SetAllMiscellanious(legendOn:hasLineOfBestFit);
    }

}

public class GraphContinuous2Var : AxisGraph {

    public TabContinuous2Var tab;

    public string xUnitName = "";
	public string xValueFormat = "";
    public string yUnitName = "";
    public string yValueFormat = "";
	public Vector2Int numSigFigs = new Vector2Int(0,0);

	public string xAxisTitle = "";
	public string yAxisTitle = "";

    public Vector2 initialMinValues = new Vector2(0, 0);
    public Vector2 initialMaxValues = new Vector2(10, 10);

    public AxisAutoFunctions xAutoFuncts;
    public bool xAxisIsAuto;
    public bool xHasMetricFormat;
    public bool hasLineOfBestFit;
    public Color lineOfBestFitColor = Color.red;

    private ScatterGraphFormatting graphFormatting = new ScatterGraphFormatting();

    private RegressionLinePlotter regression;
    private WMG_Series regSeries;

    /*
    UNUSED FUNCTIONALITY FOR SETTING X AXIS TICK NUMBER
    */

    // x label space per character, used to set x axis tick number
    //private float xLabelSpacePerChar = 35f;
    /*
    
    Functionality for formatting X Axis Label spacing.
    Not currently functional.

    void SetInitialXAxis(float initMinValue, float initMaxValue)
    {
        xAutoFuncts.rawSectNumSetter = GetXRawSectNum;
        xAutoFuncts.SetParams(initMinValue, initMaxValue, 0);
    }
    */

    // gets x axis tick number based on decimal places and width of graph
    /*private int XAxisTickNum
    { 
        get
        {
            float graphWidth = graph.GetComponent<RectTransform>().rect.width;
            //Debug.Log("graph width " + graphWidth);
            //Debug.Log("x character space " + xLabelSpacePerChar);
            int newTickNumber;
            if (numSigFigs.x == 0)
            {
                newTickNumber = Mathf.FloorToInt(graphWidth / xLabelSpacePerChar);
            }
            else if (numSigFigs.x == 1)
            {
                newTickNumber = Mathf.FloorToInt(graphWidth / xLabelSpacePerChar / 4f);
            }
            else
            {
                newTickNumber = Mathf.FloorToInt(graphWidth / xLabelSpacePerChar / (numSigFigs.x + 2f));
            }
            return newTickNumber;
        }
	}
    */

    /*
    PRIVATE METHODS
    */

    // Current functionality for X Axis label spacing.
    // Gets section number for X Axis based on graph width.
    float GetXRawSectNum(int baseSigFigs)
    {
        float rawSect = graph.GetComponent<RectTransform>().rect.width / (XSectDivisor * 3f);
        //int totalSigFigs = baseSigFigs + (int)(numSigFigs.x);
        //Debug.Log("Total sig figs: " + totalSigFigs);
        if (numSigFigs.x <= 3) return rawSect;
        return rawSect / (numSigFigs.x - 1);
    }

    // single functions to change a label into a metric, formatted axis label. Is passed to the Graphing Asset Labeller.
    string CustomYAxisLabelLabeler(WMG_Axis axis, int labelIndex)
    {
        string yLabel = GetMetricAxisLabel(axis, labelIndex, yValueFormat, (int)(numSigFigs.y), yAutoFuncts);
        return yLabel;
    }
    // single functions to change a label into a metric, formatted axis label. Is passed to the Graphing Asset Labeller.
    string CustomXAxisLabelLabeler(WMG_Axis axis, int labelIndex)
    {
        string xLabel = GetMetricAxisLabel(axis, labelIndex, xValueFormat, (int)(numSigFigs.x), xAutoFuncts);
        return xLabel;
    }

    // shorthand for getting the y axis title with a metric unit.
    string GetMetricYTitle()
    {
        return GetMetricAxisTitle(yAxisTitle, yUnitName, yHasMetricFormat, yAutoFuncts.MetricPrefix);
    }

    // shorthand for getting the x axis title with a metric unit.
    string GetMetricXTitle()
    {
        return GetMetricAxisTitle(xAxisTitle, xUnitName, xHasMetricFormat, xAutoFuncts.MetricPrefix);
    }

    // shorthand for auto setting axis in auto functions
    void UpdateXAxis(float newValue, bool pointsRemoved, int dataCount)
    {
        xAutoFuncts.AutoSetAxis(graphFormatting.xAxis, dataSeries.pointValues, (Vector2 point) => point.x,
            dataCount, pointsRemoved, newValue);
    }

    // shorthand for auto setting axis in auto functions
    void UpdateYAxis(float newValue, bool pointsRemoved, int dataCount)
    {
        yAutoFuncts.AutoSetAxis(graphFormatting.yAxis, dataSeries.pointValues, (Vector2 point) => point.y,
            dataCount, pointsRemoved, newValue);
    }

    // set and apply various formatting values to graph
    void SetGraphFormatting ()
    {
        graphFormatting.SetForScatterGraph(
            xAxisTitle: GetMetricXTitle(),
            yAxisTitle: GetMetricYTitle(),
            minValues: new Vector2(xAutoFuncts.Min, yAutoFuncts.Min),
            maxValues: new Vector2(xAutoFuncts.Max, yAutoFuncts.Max),
            xTickNum: xAutoFuncts.NumTicks, yTickNum: yAutoFuncts.NumTicks,
            hasLineOfBestFit: hasLineOfBestFit);
    }

    // Adds a series to the graph for regression line. 
    // Also sets series names to regression values, to be displayed in the legend.
    void AddRegressionSeries()
    {
        regSeries = graph.addSeries();
        dataSeries.seriesName = "\tr = ?";
        regSeries.seriesName = "\ty = [x expression]";
        regSeries.lineScale = 3.5f;
        regSeries.lineColor = lineOfBestFitColor;
        regSeries.pointWidthHeight = 20f;
        regSeries.pointColor = lineOfBestFitColor;
    }

    /*
    PUBLIC METHODS
    */

    ///<summary>Assigns variables to graph and calculates other initial values. Called in Tab Init</summary>
    public void Init (
    TabContinuous2Var parentTab, 
    string grTitle, string grXAxisTitle, string grYAxisTitle, 
    string grXUnitName, string grYUnitName, string grXValueFormat, string grYValueFormat,
    Vector2Int grNumSigFigs, Vector2 grInitMinValues, Vector2 grInitMaxValues,
    bool grAutoXAxis, bool grAutoYAxis, bool grXHasMetricFormat, bool grYHasMetricFormat,
    bool grHasPointConnectLine, bool grHasLineOfBestFit,
    Color grLineOfBestFitColor, Color grPointColor, Color grPointConnectLineColor) 
    {
        // set all variables from the tab
        tab = parentTab;
        title = grTitle;
        xAxisTitle = grXAxisTitle;
        yAxisTitle = grYAxisTitle;
        xUnitName = grXUnitName;
        yUnitName = grYUnitName;
        xValueFormat = grXValueFormat;
        yValueFormat = grYValueFormat;
        numSigFigs = grNumSigFigs;
        initialMinValues = grInitMinValues;
        initialMaxValues = grInitMaxValues;
        xAxisIsAuto = grAutoXAxis;
        yAxisIsAuto = grAutoYAxis;
        xHasMetricFormat = grXHasMetricFormat;
        yHasMetricFormat = grYHasMetricFormat;
        hasPointConnectLine = grHasPointConnectLine;
        hasLineOfBestFit = grHasLineOfBestFit;
        lineOfBestFitColor = grLineOfBestFitColor;

        // Create and initialize the Graphing Asset graph, initialize formatting and appearance
        InitAxisGraphObjects(graphFormatting);

        // instantiate objects for managing X and Y Axis minimum, maximum and labels.
        xAutoFuncts = new AxisAutoFunctions(unitIsMetric: xHasMetricFormat);
        yAutoFuncts = new AxisAutoFunctions(unitIsMetric: yHasMetricFormat);

        // set appearance variables to appearance object
        SetAppearance(grPointColor, grPointConnectLineColor);

        // get raw section numbers (using dimentions) and use to process max, min, num ticks, metric prefix
        // (x 'raw' section number setter function not currently working)
        xAutoFuncts.rawSectNumSetter = GetXRawSectNum;
        xAutoFuncts.SetParams(initialMinValues.x, initialMaxValues.x, 0);
        yAutoFuncts.SetParams(initialMinValues.y, initialMaxValues.y, GetYRawSectNum());

        // graph formatting is set and applied to graph
        SetGraphFormatting();

        // set axis labellers to ones accounting for metric units and significant figures
        graph.yAxis.axisLabelLabeler = CustomYAxisLabelLabeler;
        graph.xAxis.axisLabelLabeler = CustomXAxisLabelLabeler;

        // set y axis title offset to not overlap the labels
        ResetYAxisTitleOffset(graphFormatting);

        // set up regression if enabled.
        if (hasLineOfBestFit)
        {
            AddRegressionSeries();
            regression = new RegressionLinePlotter(dataSeries, regSeries);
        }
    }

    ///<summary>add 2 variable data to the graph and refresh formatting</summary>
    public void AddPoint(Vector2 newPoint) {
		dataCount++;
		dataSeries.pointValues.Add (newPoint);
        // Sort values so that line connectors are in right place.
        // notes: requires Linq, raises error in DOTween if animations enabled.
        if (hasPointConnectLine)
        {
            dataSeries.pointValues.SetList(dataSeries.pointValues.OrderBy(v => v.x));
        }
        if (xAxisIsAuto)
        {
            UpdateXAxis(newValue:newPoint.x, pointsRemoved: false, dataCount: dataCount);
            graphFormatting.xAxis.Title = GetMetricXTitle();
        }
        if (yAxisIsAuto)
        {
            UpdateYAxis(newValue: newPoint.y, pointsRemoved: false, dataCount: dataCount);
            graphFormatting.yAxis.Title = GetMetricYTitle();
        }
        ResetYAxisTitleOffset(graphFormatting);
        // plot regression line when there are at least three points.
        //Debug.Log("Graph Range: " + xAutoFuncts.Min + " to " + xAutoFuncts.Max);
        if (hasLineOfBestFit && dataCount >= 3)
        {
            regression.Plot(
                newMinX: xAutoFuncts.Min, newMaxX: xAutoFuncts.Max, 
                newMinY: yAutoFuncts.Min, newMaxY: yAutoFuncts.Max);
        }
		graph.Refresh ();
	}

    /// <summary> clears all data on the graph, including all counters
    public override void Clear() 
    {
        base.Clear();
        // reset the x and y auto params to their original values
        xAutoFuncts.SetParams(initialMinValues.x, initialMaxValues.x, 0);
        yAutoFuncts.SetParams(initialMinValues.y, initialMaxValues.y, GetYRawSectNum());
        // graph formatting is reset and re-applied to graph
        SetGraphFormatting();
        // regression line is reset
        regSeries.pointValues.Clear();
        dataSeries.seriesName = "\tr = ?";
        regSeries.seriesName = "\ty = [x expression]";
        ResetYAxisTitleOffset(graphFormatting);
        graph.Refresh();
    }

}
