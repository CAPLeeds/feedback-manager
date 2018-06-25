using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PieGraphFormatting
{
    public WMG_Pie_Graph graph;

    public GraphPositioning positioning;

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
    public float LegendSwatchSize
    {
        get { return graph.legend.pieSwatchSize; }
        set { graph.legend.pieSwatchSize = value; }
    }
    public int LabelFontSize
    {
        get { return graph.sliceLabelFontSize; }
        set { graph.sliceLabelFontSize = value; }
    }
    public bool SetLegendWidthFromLabels
    {
        get { return graph.legend.setWidthFromLabels; }
        set { graph.legend.setWidthFromLabels = value; }
    }
    public float AnimTime
    {
        get { return graph.animationDuration; }
        set { graph.animationDuration = value; }
    }
    public string LegendPlacement
    {
        get
        {
            if (graph.legend.legendType == WMG_Legend.legendTypes.Bottom)
            {
                if (graph.legend.oppositeSideLegend) return "top";
                else return "bottom";
            }
            else if (graph.legend.legendType == WMG_Legend.legendTypes.Right)
            {
                if (graph.legend.oppositeSideLegend) return "right";
                else return "left";
            }
            return "";
        }
        set
        {
            if (value == "right")
            {
                graph.legend.legendType = WMG_Legend.legendTypes.Right;
                graph.legend.oppositeSideLegend = false;
            }
            else if (value == "left")
            {
                graph.legend.legendType = WMG_Legend.legendTypes.Right;
                graph.legend.oppositeSideLegend = true;
            }
            else if (value == "bottom")
            {
                graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
                graph.legend.oppositeSideLegend = false;
            }
            else if (value == "top")
            {
                graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
                graph.legend.oppositeSideLegend = true;
            }
        }
    }

    public void Init(WMG_Pie_Graph graph)
    {
        this.graph = graph;
        positioning = new GraphPositioning(graph as WMG_Graph_Manager);
    }

    // shortcut to set formatting all at once. Also places legend according to graph dimentions
    public void SetAllMiscallaneous(
        int legendFontSize = 50, float legendHeight = 60f,
        float legendSwatchSize = 50, int labelFontSize = 50,
        bool setLegendWidthFromLabels = true, float animTime = 0.5f)
    {
        this.LegendFontSize = legendFontSize;
        this.LegendHeight = legendHeight;
        this.LegendSwatchSize = legendSwatchSize;
        this.LabelFontSize = labelFontSize;
        this.SetLegendWidthFromLabels = setLegendWidthFromLabels;
        this.AnimTime = animTime;
        // set legend on bottom or side according to dimensions
		Rect graphRect = graph.GetComponent<RectTransform>().rect;
		if (graphRect.width <= graphRect.height + 50f ) this.LegendPlacement = "bottom";
		else this.LegendPlacement = "right";
    }

}

public class PieGraphAppearance:GraphAppearance
{
    public WMG_Pie_Graph graph;

    public Color BackgroundColor
    {
        get { return graph.background.GetComponentInChildren<Image>().color; }
        set
        {
            graph.setBackgroundColor(value);
            graph.setBackgroundCircleColor(value);
            // get rid of image so that color is accurate
            graph.background.GetComponentInChildren<Image>().sprite = null;
            //unfortunately, have to find legend background by name. Possible source of error.
            graph.legend.transform.Find("Background").GetComponent<Image>().color = value;
        }
    }

    private Color _detailColor = Color.black;
    public Color DetailColor
    {
        get { return _detailColor; }
        set
        {
            _detailColor = value;
            graph.sliceLabelColor = value;
            graph.legend.labelColor = value;
        }
    }
    public void Init(WMG_Pie_Graph graph)
    {
        this.graph = graph;
    }

    public override void SetAllVariables(Color backgroundColor, Color detailColor)
    {
        BackgroundColor = backgroundColor;
        DetailColor = detailColor;
    }
}

public class GraphPieChart : Graph {

	public WMG_Pie_Graph graph;

    public PieGraphAppearance appearance = new PieGraphAppearance();

	private PieGraphFormatting graphFormatting = new PieGraphFormatting();

	private string graphPrefabPath = "Graph_Maker/Examples/X_Simple_Pie/EmptyPie";

    private bool emptyDefaultValuesOn = true;

    ///<summary>a hack to cause the customised legend formatting to be applied AFTER the default legend formatting.</summary>
    private void FixedUpdate()
    {
        if ((0f < Time.fixedTime) && (Time.fixedTime <= Time.fixedDeltaTime + 0.01))
        {
            graphFormatting.SetAllMiscallaneous();
        }
    }

    ///<summary>Creates graph, sets formatting, appearance and categories. Called in Tab Init.</summary>
    public void Init (List<string> categories, List<Color> colors = null) 
    {
        // find prefab and create graph
        SetEmptyGraphPrefab(graphPrefabPath);
        InitializeGraph();
        // initialise formatting and appearance
        graphFormatting.Init(graph);
        appearance.Init(graph);
        // set graph to placeholder position via formatting and destroy placeholder
        SetPlaceAndDestroyHolder(graphFormatting.positioning, appearance);
        // set appearance variables
        graphFormatting.SetAllMiscallaneous();
        // set categories and colors to graph
        SetCategories(categories, colors);
    }

    ///<summary>Instantiates emptyGraphPrefab (pie graph) and initializes it.</summary>
    void InitializeGraph () 
    {
		GameObject graphGO = base.InstantiateGraph ();
		graph = graphGO.GetComponent<WMG_Pie_Graph> ();
		graph.Init ();
	}

    ///<summary>set given categories and category colors to pie chart and reset formatting (called in Tab Init)</summary>
    public void SetCategories(List<string> categories, List<Color> colors = null)
    {
        graph.sliceLabels.SetList(categories);
        // only add colors if list meets requirements
        if ((colors != null) && (colors.Count == categories.Count))
        {
            graph.sliceColors.SetList(colors);
        }
        // set empty default values and formatting
        SetEmptyDefaultValues();
        graph.Refresh();
    }

    ///<summary>sets default values. For when all category values = 0</summary>
    void SetEmptyDefaultValues()
    {
        List<float> emptyValues = MathScientific.RepeatedList(0.1f, graph.sliceLabels.Count);
        graph.sliceValues.SetList(emptyValues);
        emptyDefaultValuesOn = true;
    }

    ///<summary>sets all values to zero and sets given value. Called when first value added.</summary>
    void SetInitialValues(int index, float firstValue)
    {
        List<float> initialValues = MathScientific.RepeatedList(0f, graph.sliceLabels.Count);
        initialValues[index] = firstValue;
        graph.sliceValues.SetList(initialValues);
        emptyDefaultValuesOn = false;
    }

    void AddSliceValue (int index, float value) {
        // assigns a list, with given value, if values are empty default.
        if (emptyDefaultValuesOn) {
            SetInitialValues(index, value);
		// otherwise, adds value to value at given index. Ensure no negative-value pie slices.
		} else if (graph.sliceValues[index] + value >= 0 ) {
			graph.sliceValues [index] += value;
		}
	}

    /// <summary> adds given value to given category on the pie chart (default value of 1) </summary>
    public void AddToCategory(string category, float value = 1f)
    {
        //Debug.Log(graph.legend.legendEntryHeight);
        int categoryIndex = graph.sliceLabels.list.IndexOf(category);
        if (categoryIndex >= 0)
        {
            AddSliceValue(categoryIndex, value);
        }
        else
        {
            Debug.LogErrorFormat("Category not in sliceLabels: " + category);
        }

        // update graph
        graph.Refresh();
    }

    ///<summary> clears all data and resets pie chart to initialization</summary>
    public void Clear () 
    {
        // set categories to current categories. This will reset the categories.
        SetCategories(graph.sliceLabels.list, graph.sliceColors.list);
        graph.Refresh();
    }
    
}
