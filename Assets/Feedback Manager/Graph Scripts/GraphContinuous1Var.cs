using UnityEngine;
using System;
using System.Collections.Generic;

public class ProgressiveGraphFormatting:AxisGraphFormatting
{
    public void SetForProgressiveGraph (string xAxisTitle, string yAxisTitle, float yMinValue, float yMaxValue,  int yTickNum)
    {
        xAxis.SetAllVariables(title: xAxisTitle, minValue:0f, maxValue: 2f, numTicks: 3, titleOffset: new Vector2(0, 70));
        yAxis.SetAllVariables(title: yAxisTitle, minValue: yMinValue, maxValue: yMaxValue);
        SetAllMiscellanious();
    }
}

public class GraphContinuous1Var : AxisGraph {

    public TabContinuous1Var tab;

    public string unitName = "";
    public string valueFormat = "";
    public int numSigFigs = 0;
    public float initialMinValue;
    public float initialMaxValue;

    public bool limitPointNum = false;
    public int maxPointNum;

    private ProgressiveGraphFormatting graphFormatting = new ProgressiveGraphFormatting();

    private int xTickInterval = 1;
    private int xSectNumAtIntervalIncrease;

    private int currentXMinValue = 0;
    private int currentXMaxValue = 2;

    // the current number of points on the graph. Calculated using dataCount and x min value.
    private int CurrentPointNum
    {
        get
        {
            if (currentXMinValue == 0) return dataCount - currentXMinValue;
            // add an extra point if current min x > 0, as minimum will now have a point on it 
            return dataCount - currentXMinValue + 1;
        }
    }

    /*
    PRIVATE METHODS
    */

    // sets X axis title (incorporating y axis unit due to y-axis title formatting issues)
    string GetProgressiveXTitle () {
		return "Progress of " + title;
	}

    // shorthand for getting the y axis title with a metric unit.
    string GetMetricYTitle ()
    {
        return GetMetricAxisTitle(title, unitName, yHasMetricFormat, yAutoFuncts.MetricPrefix);
    }

    // sets number of sections (ticks-1) at which: the label interval increases ==> the tick number decreases
    void SetXSectNumAtIntervalIncrease()
    {
        xSectNumAtIntervalIncrease = Mathf.FloorToInt(graph.GetComponent<RectTransform>().rect.width / XSectDivisor);
    }

    // shorthand for auto setting axis in auto functions
    void UpdateYAxis(float newValue, bool pointsRemoved, int dataCount)
    {
        yAutoFuncts.AutoSetAxis(graphFormatting.yAxis, dataSeries.pointValues, (Vector2 point) => point.y,
            dataCount, pointsRemoved, newValue);
    }

    // returns the next tick interval if current x corresponds to both the section number at interval increase and a factor of ten.
    // else returns current interval
    int GetNewXTickInterval()
    {
        float newXBaseInterval = (float)CurrentPointNum / (float)xSectNumAtIntervalIncrease;
        // can't be a new interval if the new base interval is not a whole number
        if (newXBaseInterval != (int)newXBaseInterval) return xTickInterval;

        // working with base interval (divided by the interval increase rate)
        int tenMultiplier = (int)Mathf.Pow(10, MathScientific.GetTenPow(newXBaseInterval));
        float baseIntervalAsUnit = (float)(newXBaseInterval) / (float)(tenMultiplier);

        if (baseIntervalAsUnit == 1) return 2 * tenMultiplier;
        if (baseIntervalAsUnit == 2) return 5 * tenMultiplier;
        if (baseIntervalAsUnit == 5) return 10 * tenMultiplier;

        return xTickInterval;
    }

    // change x tick interval to newInterval, update min and max to be multiples of the interval.
    void SetNewXTickInterval (int newInterval)
    {
        // set max and min to the next multiple of the new interval above their current values
        currentXMaxValue = Mathf.CeilToInt((float)dataCount / (float)newInterval) * newInterval;
        currentXMinValue = Mathf.CeilToInt((float)currentXMinValue / (float)newInterval) * newInterval;
        // save new interval
        xTickInterval = newInterval;
    }

    // updates x axis formatting for minimum, maximum and number of 'ticks'
    void UpdateXAxisValues()
    {
        graphFormatting.xAxis.MinValue = currentXMinValue;
        graphFormatting.xAxis.MaxValue = currentXMaxValue;
        graphFormatting.xAxis.NumTicks = (currentXMaxValue - currentXMinValue) / xTickInterval + 1;
    }

    // set and apply various formatting values to graph
    private void SetGraphFormatting ()
    {
        graphFormatting.SetForProgressiveGraph(
            xAxisTitle: GetProgressiveXTitle(),
            yAxisTitle: GetMetricYTitle(),
            yMinValue: yAutoFuncts.Min,
            yMaxValue: yAutoFuncts.Max,
            yTickNum: yAutoFuncts.NumTicks);
    }

    // single functions to change a label into a metric, formatted axis label. Is passed to the Graphing Asset Labeller.
    string CustomYAxisLabelLabeler(WMG_Axis axis, int labelIndex)
    {
        string yLabel = GetMetricAxisLabel(axis, labelIndex, valueFormat, numSigFigs, yAutoFuncts);
        return yLabel;
    }
    
    // single functions to change a label into a metric, formatted axis label. Is passed to the Graphing Asset Labeller.
    string CustomXAxisLabelLabeler(WMG_Axis axis, int labelIndex)
    {
        string xLabel = GetMetricAxisLabel(axis, labelIndex, "*", 3);
        return xLabel;
    }

    /*
    PUBLIC METHODS
    */

	///<summary>Assigns variables to graph and calculates other initial values. Called in Tab Init</summary>
	public void Init (
    TabContinuous1Var parentTab, 
    string grUnitName, string grValueFormat, int grNumSigFigs, string grTitle,
    float grInitMinValue, float grInitMaxValue, bool grAutoYAxis, bool grYHasMetricFormat,
    bool limitGrPointNum, int maxGrPointNum, bool grHasPointConnectLine,
    Color grPointColor, Color grPointConnectLineColor) {

        // set all variables from the tab
        tab = parentTab;
        unitName = grUnitName;
        valueFormat = grValueFormat;
        numSigFigs = grNumSigFigs;
        title = grTitle;
        initialMinValue = grInitMinValue;
        initialMaxValue = grInitMaxValue;
        yAxisIsAuto = grAutoYAxis;
        yHasMetricFormat = grYHasMetricFormat;
        limitPointNum = limitGrPointNum;
        // ensure a minimum of 3 values on x axis
        if (maxGrPointNum < 3) maxGrPointNum = 3;
        maxPointNum = maxGrPointNum;
        hasPointConnectLine = grHasPointConnectLine;

        // Create and initialize the Graphing Asset graph, initialize formatting and appearance
        InitAxisGraphObjects(graphFormatting);

        // instantiate object for managing Y Axis minimum, maximum and labels.
        yAutoFuncts = new AxisAutoFunctions(unitIsMetric: yHasMetricFormat);

        // set appearance variables to appearance object
        SetAppearance(grPointColor, grPointConnectLineColor);

        // get y raw section number (using height) and use to process max, min, num ticks, metric prefix
        yAutoFuncts.SetParams(initialMinValue, initialMaxValue, GetYRawSectNum());

        // graph formatting is set and applied to graph
        SetGraphFormatting();

        // set axis labellers to ones accounting for metric units and significant figures
        graph.yAxis.axisLabelLabeler = CustomYAxisLabelLabeler;
        graph.xAxis.axisLabelLabeler = CustomXAxisLabelLabeler;

        // set y axis title offset to not overlap the labels
        ResetYAxisTitleOffset(graphFormatting);

        // x axis maximum tick value
        SetXSectNumAtIntervalIncrease();
        //Debug.Log("x sect num at increase: "+ xSectNumAtIntervalIncrease);
    }

	///<summary>adds a given point to the graph and updates graph formatting</summary>
	public void AddPoint (float newValue) {
		
		dataCount++;

		// add point to series
		dataSeries.pointValues.Add (new Vector2 (dataCount, newValue));
        bool pointsRemoved = false;

        // updates x axis tick interval, sets x axis ticks according to interval and value num
        // (excludes low values)
        if (dataCount > 2) {

            int newInterval = GetNewXTickInterval();
            bool intervalHasChanged = (newInterval != xTickInterval);

            // if there is a new interval, set min and max to fit
            if (intervalHasChanged)
            {
                SetNewXTickInterval(newInterval);
                pointsRemoved = true;
            }

            bool offEdgeOfGraph = ((dataCount - 1) % xTickInterval == 0);
            bool pointLimitExceeded = (limitPointNum && (CurrentPointNum > maxPointNum));

            //Debug.Log("Current Point Num: " + CurrentPointNum);
            // check if min or max require updating
            if (offEdgeOfGraph || pointLimitExceeded)
            {
                // increase maximum x if the previous x was a multiple of the tick interval
                if (offEdgeOfGraph)
                {
                    currentXMaxValue = dataCount - 1 + xTickInterval;
                }
                // increase minimum x if the point number has exceeded its maximum value
                if (pointLimitExceeded)
                {
                    int pointsToRemove = xTickInterval;
                    int valueToAdd = xTickInterval;
                    // use zero slot to hold a point if it's free
                    if (currentXMinValue == 0)
                    {
                        if (xTickInterval == 1) valueToAdd++;
                        else pointsToRemove--;
                    }
                    dataSeries.pointValues.RemoveRange(0, pointsToRemove);
                    currentXMinValue += valueToAdd;
                    pointsRemoved = true;
                }
            }
            // If any of the above circumstances are true, update the x axis values
            if (intervalHasChanged || offEdgeOfGraph || pointLimitExceeded)
            {
                UpdateXAxisValues();
            }
        }

        if (yAxisIsAuto)
        {
            UpdateYAxis(newValue, pointsRemoved, dataCount);
            graphFormatting.yAxis.Title = GetMetricYTitle();
        }
        ResetYAxisTitleOffset(graphFormatting);
        graph.Refresh();
	}

    ///<summary>Clears all data from the graph and resets counters, axes and formatting</summary>
    public override void Clear () 
    {
        // clear data set and main counter
        base.Clear();
        // reset other counters
        currentXMinValue = 0;
        currentXMaxValue = 2;
        // reset the y auto params to their original values
        yAutoFuncts.SetParams(initialMinValue, initialMaxValue, GetYRawSectNum());
        // graph formatting is reset and re-applied to graph
        SetGraphFormatting();
        ResetYAxisTitleOffset(graphFormatting);
        graph.Refresh();
    }

}

