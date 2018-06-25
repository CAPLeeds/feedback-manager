using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// generalised formatting for an axis, accessed via graphFormatting
public class AxisFormatting
{
    public WMG_Axis axis;

    public string Title
    {
        get { return axis.AxisTitleString; }
        set { axis.AxisTitleString = value; }
    }
    public int TitleFontSize
    {
        get { return axis.AxisTitleFontSize; }
        set { axis.AxisTitleFontSize = value; }
    }
    public Vector2 TitleOffset
    {
        get { return axis.AxisTitleOffset; }
        set { axis.AxisTitleOffset = value; }
    }
    public bool MinAutoGrow
    {
        get { return axis.MinAutoGrow; }
        set { axis.MinAutoGrow = value; }
    }
    public bool MinAutoShrink
    {
        get { return axis.MinAutoShrink; }
        set { axis.MinAutoShrink = value; }
    }
    public bool MaxAutoGrow
    {
        get { return axis.MaxAutoGrow; }
        set { axis.MaxAutoGrow = value; }
    }
    public bool MaxAutoShrink
    {
        get { return axis.MaxAutoShrink; }
        set { axis.MaxAutoShrink = value; }
    }
    public float MinValue
    {
        get { return axis.AxisMinValue; }
        set { axis.AxisMinValue = value; }
    }
    public float MaxValue
    {
        get { return axis.AxisMaxValue; }
        set { axis.AxisMaxValue = value; }
    }
    public int NumTicks
    {
        get { return axis.AxisNumTicks; }
        set { axis.AxisNumTicks = value; }
    }
    public bool LabelsUseMaxMin
    {
        get { return axis.SetLabelsUsingMaxMin; }
        set { axis.SetLabelsUsingMaxMin = value; }
    }
    public int NumDecimals
    {
        get { return axis.numDecimalsAxisLabels; }
        set { axis.numDecimalsAxisLabels = value; }
    }
    public int LabelSize
    {
        get { return axis.AxisLabelSize; }
        set { axis.AxisLabelSize = value; }
    }
    public string LabelType
    {
        get
        {
            if (axis.LabelType == WMG_Axis.labelTypes.groups) return "groups";
            if (axis.LabelType == WMG_Axis.labelTypes.manual) return "manual";
            if (axis.LabelType == WMG_Axis.labelTypes.ticks) return "ticks";
            if (axis.LabelType == WMG_Axis.labelTypes.ticks_center) return "ticks center";
            return "";
        }
        set
        {
            if (value == "groups") axis.LabelType = WMG_Axis.labelTypes.groups;
            else if (value == "manual") axis.LabelType = WMG_Axis.labelTypes.manual;
            else if (value == "ticks") axis.LabelType = WMG_Axis.labelTypes.ticks;
            else if (value == "ticks center") axis.LabelType = WMG_Axis.labelTypes.ticks_center;
        }
    }

    public bool HideTicks
    {
        get { return axis.hideTicks; }
        set { axis.hideTicks = value; }
    }

    public bool HideArrows
    {
        get { return axis.HideAxisArrowTopRight && axis.HideAxisArrowBotLeft; }
        set
        {
            axis.HideAxisArrowTopRight = value;
            axis.HideAxisArrowBotLeft = value;
        }
    }

    // constructor method
    public AxisFormatting (WMG_Axis axis) 
    { 
        this.axis = axis;
    }
    // shortcut to set all of the axis formatting variables at once
    public void SetAllVariables(
        string title = "", int titleFontSize = 55, Vector2 titleOffset = new Vector2(),
        bool minAutoGrow = false, bool minAutoShrink = false, bool maxAutoGrow = false,
        bool maxAutoShrink = false, float minValue = 0, float maxValue = 5,
        int numTicks = 6, bool labelsUseMaxMin = true, int numDecimals = 0,
        int labelSize = 50, string labelType = "ticks", bool hideTicks = true, bool hideArrows = true)
    {
        this.Title = title;
        this.TitleFontSize = titleFontSize;
        this.TitleOffset = titleOffset;
        this.MinAutoGrow = minAutoGrow;
        this.MinAutoShrink = minAutoShrink;
        this.MaxAutoGrow = maxAutoGrow;
        this.MaxAutoShrink = maxAutoShrink;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.NumTicks = numTicks;
        this.LabelsUseMaxMin = labelsUseMaxMin;
        this.NumDecimals = numDecimals;
        this.LabelSize = labelSize;
        this.LabelType = labelType;
        this.HideTicks = hideTicks;
        this.HideArrows = hideArrows;

        axis.graph.Refresh();
    }
}

public class AxisGraphFormatting
{
    public WMG_Axis_Graph graph;

    public GraphPositioning positioning;
    public AxisFormatting xAxis;
    public AxisFormatting yAxis;

    public bool ResizeEnabled
    {
        get { return graph.resizeEnabled; }
        set
        {
            graph.resizeEnabled = value;
            if (value)
            {
                WMG_Axis_Graph.ResizeProperties resizeProperties =
                    WMG_Axis_Graph.ResizeProperties.AxesArrowSize |
                    WMG_Axis_Graph.ResizeProperties.AxesLinePadding |
                    WMG_Axis_Graph.ResizeProperties.AxesWidth |
                    WMG_Axis_Graph.ResizeProperties.TickSize;
                graph.resizeProperties = resizeProperties;
            }
        }
    }
    public bool AutoPaddingEnabled
    {
        get { return graph.autoPaddingEnabled; }
        set { graph.autoPaddingEnabled = value; }
    }
    public float AutoPaddingAmount
    {
        get { return graph.autoPaddingAmount; }
        set { graph.autoPaddingAmount = value; }
    }
    public bool LegendOn
    {
        get { return !graph.legend.hideLegend; }
        set { graph.legend.hideLegend = !value; }
    }
    public int LegendFontSize
    {
        get { return graph.legend.legendEntryFontSize; }
        set { graph.legend.legendEntryFontSize = value; }
    }
    public float LegendHeight
    {
        get { return graph.legend.legendEntryHeight; }
        set { graph.legend.legendEntryHeight = value; }
    }
    public bool AnimEnabled
    {
        get { return graph.tooltipAnimationsEnabled || graph.autoAnimationsEnabled; }
        set
        {
            graph.tooltipAnimationsEnabled = value;
            graph.autoAnimationsEnabled = value;
        }
    }

    // call before using anything else!
    public void Init (WMG_Axis_Graph graph)
    {
        this.graph = graph;
        positioning = new GraphPositioning(graph as WMG_Graph_Manager);
        xAxis = new AxisFormatting(graph.xAxis);
        yAxis = new AxisFormatting(graph.yAxis);
    }

    // sets extra variables not related to axis or positioning
    public void SetAllMiscellanious(
        bool resizeEnabled = true,
        bool autoPaddingEnabled = true, float autoPaddingAmount = 30f,
        bool legendOn = false, int legendFontSize = 40, float legendHeight = 55,
        bool animEnabled = false)
    {
        this.ResizeEnabled = resizeEnabled;
        this.AutoPaddingEnabled = autoPaddingEnabled;
        this.AutoPaddingAmount = autoPaddingAmount;
        this.LegendOn = legendOn;
        this.LegendFontSize = legendFontSize;
        this.LegendHeight = legendHeight;
        // note: DOTween animations sometimes do not work or raise errors.
        this.AnimEnabled = animEnabled;

        graph.Refresh();
    }
}

public class AxisGraphAppearance:GraphAppearance
{
    public WMG_Axis_Graph graph;

    public WMG_Series dataSeries;

    public Color BackgroundColor
    {
        // assume background image is first image found in background
        get { return graph.graphBackground.GetComponentInChildren<Image>().color; }
        set
        {
            graph.setBackgroundColor(value);
            // get rid of sprite image so that color is accurate
            graph.graphBackground.GetComponentInChildren<Image>().sprite = null;
        }
    }

    private Color _detailColor = Color.white;
    public Color DetailColor
    {
        get
        {
            return _detailColor;
        }
        set
        {
            _detailColor = value;
            graph.xAxis.AxisLabelColor = value;
            graph.yAxis.AxisLabelColor = value;
            // unfortunately, have to search for axis title colors and lines by name. POSSIBLE SOURCE OF ERROR
            graph.transform.Find("Background/GraphAreaBounds/AxisTitle-X").GetComponent<Text>().color = value;
            graph.transform.Find("Background/GraphAreaBounds/AxisTitle-Y").GetComponent<Text>().color = value;
            graph.transform.Find("Background/XAxis/Line").GetComponent<Image>().color = value;
            graph.transform.Find("Background/YAxis/Line").GetComponent<Image>().color = value;
        }
    }

    public Color PointConnectLineColor
    {
        get { return dataSeries.lineColor; }
        set { dataSeries.lineColor = value; }
    }

    public Color PointColor
    {
        get { return dataSeries.pointColor; }
        set { dataSeries.pointColor = value; }
    }

    // constructor method
    public AxisGraphAppearance(WMG_Axis_Graph graph, WMG_Series dataSeries)
    {
        this.graph = graph;
        this.dataSeries = dataSeries;
    }

    public override void SetAllVariables(Color backgroundColor, Color detailColor)
    {
        BackgroundColor = backgroundColor;
        DetailColor = detailColor;
    }
}

public class AxisGraph : Graph {

    public WMG_Axis_Graph graph;

    public WMG_Series dataSeries;

    public AxisGraphAppearance appearance;

    // the constants for setting axis label intervals based on graph dimentions
    public float XSectDivisor { get { return 120f; } }
    public float YSectDivisor { get { return 120f; } }

    // total data values taken
    public int dataCount = 0;

    // functions for automatic axis size and label updating
    public AxisAutoFunctions yAutoFuncts;
    // y axis automatically updates?
    public bool yAxisIsAuto;
    // include a metric unit prefix (e.g. m, k) automatically
    public bool yHasMetricFormat;

    public bool hasPointConnectLine = true;

    [HideInInspector]
    public string graphPrefabPath = "Graph_Maker/Prefabs/Graphs/AxisGraphs/EmptyGraph";
    [HideInInspector]
    public string tooltipPrefabPath = "Graph_Maker/Prefabs/Misc/Tooltip";

    private Transform yLabels;

    /*
    PRIVATE METHODS
    */

    // create worldspace script and assign graph and tooltip prefab to it
    void AddWorldSpaceScript()
    {
        WMG_X_WorldSpace worldSpaceScript = gameObject.AddComponent<WMG_X_WorldSpace>();
        worldSpaceScript.graph = graph;
        worldSpaceScript.tooltipPrefab = GetPrefab(tooltipPrefabPath);
        worldSpaceScript.canvasGO = transform.GetComponentInParent<Canvas>().gameObject;
    }

    // add main data series for graph
    void AddGraphSeries()
    {
        dataSeries = graph.addSeries();
        dataSeries.seriesName = title;
        dataSeries.hideLines = !hasPointConnectLine;
        dataSeries.lineScale = 4;
        dataSeries.pointWidthHeight = 30;
    }

    // create graph gameobject, save Axis Graph and add world space and series
    void InitializeGraph()
    {
        GameObject graphGO = base.InstantiateGraph();
        graph = graphGO.GetComponent<WMG_Axis_Graph>();
        graph.Init();
        AddWorldSpaceScript();
        AddGraphSeries();
        yLabels = GetLabels('y');
    }

    private Transform GetLabels(char axisChar)
    {
        string axis = axisChar.ToString().ToUpper();
        Transform axisLabels = graph.transform.Find("Background/" + axis + "AxisLabels");
        return axisLabels;
    }

    // finds each object in the given label container transform, and returns the one with the rect of maximum width 
    private float GetMaxWidthLabel(Transform labelContainer)
    {
        List<Transform> nodes = new List<Transform>();
        foreach (Transform node in labelContainer) nodes.Add(node);
        return MathScientific.Max(nodes, (Transform t) => t.GetChild(0).GetComponent<RectTransform>().rect.width);
    }

    /*
    PUBLIC VIRTUAL METHODS
    */
    
    // resets axis graph to original
    public virtual void Clear() 
    {
        dataSeries.pointValues.Clear();
        dataCount = 0;
    }

    /*
    PUBLIC METHODS CALLED IN CHILD CLASSES
    */

    ///<summary>Creates and initializes the Graphing Asset graph, initializes formatting and appearance</summary>
    public void InitAxisGraphObjects(AxisGraphFormatting formatting)
    {
        // find prefab and create graph
        SetEmptyGraphPrefab(graphPrefabPath);
        InitializeGraph();
        // initialise formatting (passed from child class object)
        formatting.Init(graph);
        // create and initialize appearance
        appearance = new AxisGraphAppearance(graph, dataSeries);
        // set graph to placeholder position via formatting and destroy placeholder
        SetPlaceAndDestroyHolder(formatting.positioning, appearance);
    }

    ///<summary>Sets color of points and line connecting the points</summary>
    public void SetAppearance(Color pointColor, Color pointConnectLineColor) 
    {
        appearance.PointColor = pointColor;
        appearance.PointConnectLineColor = pointConnectLineColor;
    }

    // gets float value for y axis section number based on height (sections = ticks - 1)
    public float GetYRawSectNum()
    {
        return graph.GetComponent<RectTransform>().rect.height / YSectDivisor;
    }

    // formats an axis title to include the unit
    public string GetMetricAxisTitle(string axisTitle, string unit, bool unitIsMetric = false, string metricPrefix = "")
    {
        // return title only, if no unit
        if (unit == "" || unit == " ") return axisTitle;
        // return title and unit only if no metric
        if (!unitIsMetric) return axisTitle + " (" + unit + ")";
        // if metric, return title AND unit with metric unit prefix written out in full
        return axisTitle + " (" + MathScientific.GetFullMetricPrefix(metricPrefix) + unit + ")";
    }

        // custom update function for labelling any axis with metric units
    public string GetMetricAxisLabel(WMG_Axis axis, int labelIndex, string valueFormat, int sigFigs, AxisAutoFunctions autoAxisParams = null)
    {
        // map label value to min/max
        float num = axis.AxisMinValue + labelIndex * (axis.AxisMaxValue - axis.AxisMinValue) / (axis.axisLabels.Count - 1);
        if (autoAxisParams == null) return MathScientific.ToMetric(num, valueFormat, sigFigs);
        return autoAxisParams.FormatLabel(num, valueFormat, sigFigs);
    }

    public void ResetYAxisTitleOffset(AxisGraphFormatting formatting)
    {
        graph.yAxis.UpdateAxesLabels();
        formatting.yAxis.TitleOffset = new Vector2(GetMaxWidthLabel(yLabels) + 20f, 0);
        //Debug.Log(GetMaxWidthLabel(yLabels));
    }
}
 