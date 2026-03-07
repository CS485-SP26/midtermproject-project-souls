using UnityEngine;

public class SDCurveAttribute : PropertyAttribute
{
    public float minValue;
    public float maxValue;
    public string xLabel;
    public string yLabel;

    public SDCurveAttribute(float minValue, float maxValue, string xLabel = "Time", string yLabel = "Value")
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.xLabel = xLabel;
        this.yLabel = yLabel;
    }
}