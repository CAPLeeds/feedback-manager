using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathScientific {

    private static string[] metrics = new string[] { "y", "z", "a", "f", "p", "n", "u", "m", "", "k", "M", "G", "T", "P", "E", "Z", "Y" };
    private static string[] fullMetrics = new string[] { "yocto", "zepto", "atto", "fempto", "pico", "nano", "micro", "milli", "", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta", "yotta" };
    // index of "" in metrics
    private static int noPowerIndex = 8;

    // finds single char prefix in list, e.g. 'k', and returns full version from parallel list, e.g. 'kilo'
    public static string GetFullMetricPrefix(string metricPrefix)
    {
        int index = Array.IndexOf(metrics, metricPrefix);
        if (index < 0) return "";
        return fullMetrics[index];
    }

    // use scientific notation format to find power of ten of number
    public static int GetTenPow(float num)
    {
        string asSci = string.Format("{0:E0}", num);
        string powString = asSci.Substring(asSci.Length - 4, 4);
        int pow = int.Parse(powString);
        return pow;
    }

    // rounds float to given significant figures
    public static float RoundToSignificantDigits(float d, int digits)
    {
        if (d == 0)
            return 0;

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return (float)(scale * Math.Round(d / scale, digits));

    }

    // returns largest power of 1000 that 'num' can be divided by and still be >= 1.
    public static int GetThsdPow(float num)
    {
        int lower = -noPowerIndex, upper = (metrics.Length - 1) - noPowerIndex;
        int? thsdPow = null;
        int i = lower;
        while ((i < upper) && (thsdPow == null))
        {
            if (num < Mathf.Pow(1000, i + 1)) thsdPow = i;
            i++;
        }
        if (thsdPow == null) thsdPow = upper;
        return (int)thsdPow;
    }

    // finds metric unit in list that corresponds to given power of 1000
    public static string GetMetricUnit(int thsdPow)
    {
        return metrics[thsdPow + noPowerIndex];
    }

    // converts value to metric units, rounded to given decimal places with given unit
    public static string ToMetric(float value, string valueFormat = "*", int sigFigs = 3, int? customThsdPow = null)
    {
        // always return 0 for zero value
        if (value == 0f) return "0";

        // use custom thousand power if provided, otherwise find one based on number
        int thsdPow;
        if (customThsdPow == null)
        {
            thsdPow = GetThsdPow(value);
        }
        else
        {
            thsdPow = (int)customThsdPow;
        }

        // find number to display by dividing by the power of 1000
        float num = value / Mathf.Pow(1000, thsdPow);
        // round to significant figures
        num = RoundToSignificantDigits(num, sigFigs);

        // format with metric prefix and provided formatting
        string unformattedValue = num.ToString() + GetMetricUnit(thsdPow);
        return FormatString(unformattedValue, valueFormat);
    }

    // takes a float, rounds it to given figures, applies given format and returns a string.
    public static string ToFormattedValue(float value, string format = "*", int sigFigs = 3)
    {
        if (value == 0f) return "0";

        string unformattedValue = RoundToSignificantDigits(value, sigFigs).ToString();

        return FormatString(unformattedValue, format);
    }

    // gets the closest term in the series { .1,.2,.5,1,2,5,10,20,50,100 } to the given number.
    // as used for axis tick intervals, always rounds up.
    public static float Best1v2v5SeriesTerm(float num)
    {
        float tenMultiplier = (float)Math.Pow(10, GetTenPow(num));
        float unitNum = num / tenMultiplier;
        float unitTerm = 0;
        if (unitNum <= 1f) unitTerm = 1;
        else if (unitNum <= 2f) unitTerm = 2;
        else if (unitNum <= 5f) unitTerm = 5;
        else unitTerm = 10;
        return unitTerm * tenMultiplier;
    }

    // returns list of given length of given value; e.g. value 1f, count 3 -> List<float>{1f,1f,1f}
    public static List<T> RepeatedList<T>(T value, int count)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < count; i++) {
            result.Add(value);
        }
        return result;
    }

    // returns string where wildcards * and {0} in format string are replaced with value.
    public static string FormatString(string value, string format)
    {
        return String.Format(format.Replace("*", "{0}"), value);
    }

    // iterates the given function 'reference' though given IEnumerable to find float values, and finds maximum value from these values.
    public static float Max<T>(IEnumerable<T> data, Func<T, float> reference)
    {
        IEnumerator<T> e = data.GetEnumerator();
        e.MoveNext();
        float max = reference(e.Current);
        while (e.MoveNext())
        {
            if (reference(e.Current) > max)
            {
                max = reference(e.Current);
            }
        }
        return max;
    }

    // iterates the given function 'reference' though given IEnumerable to find float values, and finds minimum value from these values.
    public static float Min<T>(IEnumerable<T> data, Func<T, float> reference)
    {
        IEnumerator<T> e = data.GetEnumerator();
        e.MoveNext();
        float min = reference(e.Current);
        while (e.MoveNext())
        {
            if (reference(e.Current) < min)
            {
                min = reference(e.Current);
            }
        }
        return min;
    }
}


