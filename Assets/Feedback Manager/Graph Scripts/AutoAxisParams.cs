using UnityEngine;
using System;
using System.Collections.Generic;

// used to automatically get and set range, tick number and label values on an axis using raw(unprocessed) values
public class AxisAutoFunctions
{
    public bool unitIsMetric = true;

    private float _rawMin;
    private float _rawMax;
    private float _rawNumSects;

    private float _min;
    private float _max;
    private int _numTicks;

    private int _thsdPow;
    private string _metricPrefix;

    private int _baseSigFigs;

    public float RawMin { get { return _rawMin; } }
    public float RawMax { get { return _rawMax; } }
    public float RawNumSects { get { return _rawNumSects; } }

    public float Max { get { return _max; } }
    public float Min { get { return _min; } }
    public int NumTicks { get { return _numTicks; } }

    public int ThsdPow { get { return _thsdPow; } }
    public string MetricPrefix { get { return _metricPrefix; } }

    public int BaseSigFigs { get { return _baseSigFigs; } }

    public delegate float RawSectNumSetter(int baseSigFigs);

    public RawSectNumSetter rawSectNumSetter;

    // default function to set raw number of sections. Can be overridden.
    private float defaultRawSectNumSetter(int baseSigFigs)
    {
        return RawNumSects;
    }

    // sets closest min, max and tick number to raw values based on legal intervals
    private void _SnapRawValuesToInterval()
    {
        float rawInterval = (RawMax - RawMin) / RawNumSects;
        float interval = MathScientific.Best1v2v5SeriesTerm(rawInterval);
        _min = Mathf.Floor(RawMin / interval) * interval;
        _max = Mathf.Ceil(RawMax / interval) * interval;
        _numTicks = Mathf.RoundToInt((Max - Min) / interval + 1);
        _SetMetricPrefix(interval);
    }

    // sets the metric unit prefix of axis labels based on power of 1000 of interval.
    private void _SetMetricPrefix(float interval)
    {
        _thsdPow = MathScientific.GetThsdPow(interval);
        _metricPrefix = MathScientific.GetMetricUnit(_thsdPow);
    }

    // sets a base number of significant figures to ensure the interval is always visible.
    private void _SetBaseSigFigs()
    {
        _baseSigFigs = MathScientific.GetTenPow((RawMax + RawMin) / (RawMax - RawMin));
    }

    // updates using new value and saved range if new value provided and no values have been removed.
    // otherwise, has to find the range by iterating through all points, which uses more processing power.
    // returns true if range was updated, false if not.
    private bool _UpdateRangeQuery(IEnumerable<Vector2> points, Func<Vector2, float> getXOrY,
                                   bool pointsRemoved = true, float? newValue = null)
    {
        if ((!pointsRemoved) && (newValue != null))
        {
            if (newValue < RawMin) _rawMin = (float)newValue;
            else if (newValue > RawMax) _rawMax = (float)newValue;
            else return false;
        }
        else // need to completely update range if a value was removed or new value is not provided
        {
            _rawMin = MathScientific.Min(points, getXOrY);
            _rawMax = MathScientific.Max(points, getXOrY);
        }
        return true;
    }

    // sets the minimum and maximum to the first value when it is added.
    // this gets rid of the initial range and allows the range to span only the first and second values when the second value is added.  
    private void _FirstValueRangeException(int numValues, float firstValue)
    {
        if (numValues == 1)
        {
            _rawMin = firstValue;
            _rawMax = firstValue;
        }
    }

    // ###################### PUBLIC METHODS ##########################

    // constructor method
    public AxisAutoFunctions(bool unitIsMetric) 
    {
        rawSectNumSetter = defaultRawSectNumSetter;
        this.unitIsMetric = unitIsMetric;
    }
    
    // sets all used variables from raw variables
    public void Process()
    {
        // extra significant figures, so step is always visible in labels
        _SetBaseSigFigs();
        // custom section num setter to solve x label overlap issue (not currently used)
        _rawNumSects = rawSectNumSetter(0);
        //Debug.Log(" raw num sects: " + RawNumSects);
        // set closest minimum, maximum, tick number to raw based on legal intervals
        _SnapRawValuesToInterval();
    }

    // assigns processed values to given axis
    public void AssignToAxis(AxisFormatting axisFormat)
    {
        axisFormat.MaxValue = Max;
        axisFormat.MinValue = Min;
        axisFormat.NumTicks = NumTicks;
    }

    // sets raw values, uses these to set processed values
    public void SetParams(float newRawMin, float newRawMax, float newRawNumSects)
    {
        _rawMin = newRawMin;
        _rawMax = newRawMax;
        _rawNumSects = newRawNumSects;
        Process();
    }

    // update raw maximum and minimum according to new value and re-process if they have changed
    public void AutoSetAxis( AxisFormatting axisFormat,
                             IEnumerable<Vector2> points, Func<Vector2, float> getXOrY,
                             int dataCount, bool pointsRemoved = true, float? newValue = null)
    {
        bool rangeHasChanged = _UpdateRangeQuery(points, getXOrY, pointsRemoved, newValue);

        if (rangeHasChanged && RawMin != RawMax)
        {
            Process();
            AssignToAxis(axisFormat);
        }

        if (newValue != null) _FirstValueRangeException(dataCount, (float)newValue);
    }

    // takes a label value, and related varaibles, and returns it formatted to fit the axis formatting
    public string FormatLabel(float value, string valueFormat, int sigFigs)
    {
        if(unitIsMetric)
        {
            return MathScientific.ToMetric(value, valueFormat, BaseSigFigs + sigFigs, customThsdPow:ThsdPow);
        }
        return MathScientific.ToFormattedValue(value, valueFormat, BaseSigFigs + sigFigs);
    }

}

