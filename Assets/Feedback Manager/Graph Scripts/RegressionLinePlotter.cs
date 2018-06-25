using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class RegressionLinePlotter
{
    public WMG_Series dataSeries;
    public WMG_Series lineSeries;

    float minX = 0f;
    float maxX = 5f;
    float minY = 0f;
    float maxY = 5f;

    float pointNum = 100;
    int numDecimalsToRound;

    float decimalsMultiplier;
    Regex operatorAndParenthesesRegex;

    // get x and y values from data separately
    List<float> Xdata
    {
        get
        {
            return new List<float>(dataSeries.pointValues.Select(n => n.x));
        }
    }

    List<float> Ydata
    {
        get
        {
            return new List<float>(dataSeries.pointValues.Select(n => n.y));
        }
    }

    // regression variables to be used
    float Sxy
    {
        get
        {
            return SumOfProdOfAandBDeviationsFromMean(Xdata, Ydata);
        }
    }

    float Sxx
    {
        get
        {
            return SumOfProdOfAandBDeviationsFromMean(Xdata, Xdata);
        }
    }

    float Syy
    {
        get
        {
            return SumOfProdOfAandBDeviationsFromMean(Ydata, Ydata);
        }
    }

    // using y on x, so x should be independent variable
    float? Gradient
    {
        get
        {
            if (Sxx == 0f) return null;
            return Sxy / Sxx;
        }
    }

    float? Const
    {
        get
        {
            if (Gradient == null) return null;
            return Mean(Ydata) - (Gradient * Mean(Xdata));
        }
    }

    // sum of IEnumerable values using Linq
    float Sum(IEnumerable<float> values) { return values.Aggregate((a, b) => a + b); }

    // mean of IEnumerable values using Linq
    float Mean(IEnumerable<float> values) { return values.Average(); }

    float SumOfProdOfAandBDeviationsFromMean(List<float> listA, List<float> listB)
    {
        if (listA.Count != listB.Count) Debug.LogError("parallel lists must be of equal length!");

        int n = listA.Count;
        float sumOfABProducts = Sum(Enumerable.Range(0, n).Select(x => listA[x] * listB[x]));
        float result = sumOfABProducts - (Sum(listA) * Sum(listB)) / n;
        return result;
    }

    // Separate all the mathematical operators by spaces, so can split the string into a list
    string EnsureDelimiterAroundOperatorsAndParentheses(string input)
    {
        string result = operatorAndParenthesesRegex.Replace(input, @" $& ");
        Regex multiWhitespaceRegex = new Regex(@"\s+");
        result = multiWhitespaceRegex.Replace(result, " ");
        return result.Trim();
    }

    ////////////////////////////// Public Methods /////////////////////////////

    // constructor method
    public RegressionLinePlotter(WMG_Series dataSeries, WMG_Series lineSeries, int decimalsToRound = 6) 
    { 
        this.dataSeries = dataSeries;
        this.lineSeries = lineSeries;
        numDecimalsToRound = decimalsToRound;
        Init();
    }

    // initializes regression line. Called in constructor
    public void Init()
    {
        List<string> operatorsAndParentheses = new List<string>(new string[] {@"\(", @"\)", @"\+", "-", @"\*", "/"});
        string operatorAndParenthesesRegexStr = " ?(";
        for (int i = 0; i < operatorsAndParentheses.Count; i++)
        {
            operatorAndParenthesesRegexStr += "(";
            operatorAndParenthesesRegexStr += operatorsAndParentheses[i];
            if (i == operatorsAndParentheses.Count - 1)
            {
                operatorAndParenthesesRegexStr += ")";
            }
            else
            {
                operatorAndParenthesesRegexStr += ")|";
            }
        }
        operatorAndParenthesesRegexStr += ") ?";
        operatorAndParenthesesRegex = new Regex(operatorAndParenthesesRegexStr);

        decimalsMultiplier = Mathf.Pow(10f, numDecimalsToRound);
    }

    public void Plot(float? newMinX = null, float? newMaxX = null, float? newMinY = null, float? newMaxY = null)
    {
        if (newMinX != null) minX = (float)newMinX;
        if (newMaxX != null) maxX = (float)newMaxX * 1.02f;
        if (newMinY != null) minY = (float)newMinY;
        if (newMaxY != null) maxY = (float)newMaxY;

        //Debug.Log("Range: " + minX + " to " + maxX);
        float intervalX = (maxX - minX) / pointNum;
        //Debug.Log("Best Fit Line Interval: " + intervalX);

        string yExpression = ExpressionForY(rounded:false);
        // exception for y = infinity.
        if (yExpression == "")
        {
            dataSeries.seriesName = "\ty = undefined";
            return;
        }
        // get data series legend to show r, to save space
        dataSeries.seriesName = "\tr = " + PearsonsPMCC(rounded: true);
        lineSeries.seriesName = "\ty = " + ExpressionForY(rounded: true);

        lineSeries.pointValues.Clear();

        string formattedEquationStr = EnsureDelimiterAroundOperatorsAndParentheses(yExpression);
        List<string> rpnString = WMG_Util.ShuntingYardAlgorithm(formattedEquationStr);

        for (float i = minX; i <= (maxX + Mathf.Epsilon); i += intervalX)
        {
            i = Mathf.Round(i * decimalsMultiplier) / decimalsMultiplier;
            Vector2 expResult = WMG_Util.ExpressionEvaluator(rpnString, i);
            if (!float.IsNaN(expResult.y) && (minY <= expResult.y) && (expResult.y <= maxY))
            {
                lineSeries.pointValues.Add(expResult);
            }
        }

        //Debug.Log("y = " + ExpressionForY(rounded:true));
    }

    // returns regression equation in terms of x, without "y = "
    public string ExpressionForY(bool rounded = false)
    {
        // return empty when best fit equation is of form x = A
        if (Sxx == 0f) return "";

        // option to round or not roun
        float a;
        float b;
        if (rounded)
        {
            a = Mathf.Round((float)Const * decimalsMultiplier) / decimalsMultiplier;
            b = Mathf.Round((float)Gradient * decimalsMultiplier) / decimalsMultiplier;
        }
        else
        {
            a = (float)Const;
            b = (float)Gradient;
        }

        // return with no scientific notation
        return a.ToString("0.################") + " + " + b.ToString("0.################") + "*x";
    }

    public float PearsonsPMCC(bool rounded = false)
    {
        // exception to avoid zero division
        if ((Sxx == 0) || (Syy == 0)) return 1f;

        float r = Sxy / Mathf.Sqrt(Sxx * Syy);
        if (rounded)
        {
            r = Mathf.Round(r * decimalsMultiplier) / decimalsMultiplier;
        }
        return r;
    }

}
