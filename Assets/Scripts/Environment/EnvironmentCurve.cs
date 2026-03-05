using UnityEngine;

[System.Serializable]
public class EnvironmentCurve
{
    [System.Serializable]
    public struct ControlPoint
    {
        public float mean;
        public float stdDev;
        public float time; // 0-1 across the year
    }

    public ControlPoint[] points = new ControlPoint[4]
    {
        new ControlPoint { mean = 15f, stdDev = 3f, time = 0.125f }, // Spring
        new ControlPoint { mean = 30f, stdDev = 2f, time = 0.375f }, // Summer
        new ControlPoint { mean = 12f, stdDev = 4f, time = 0.625f }, // Fall
        new ControlPoint { mean = -5f, stdDev = 5f, time = 0.875f }  // Winter
    };

    public float SampleMean(float t) => Sample(t, p => p.mean);
    public float SampleSD(float t)   => Sample(t, p => p.stdDev);

    private float Sample(float t, System.Func<ControlPoint, float> getValue)
    {
        if (points == null || points.Length == 0) return 0f;

        // Find surrounding points, wrapping around year boundary
        ControlPoint p0 = points[points.Length - 1];
        ControlPoint p1 = points[0];

        for (int i = 0; i < points.Length - 1; i++)
        {
            if (t >= points[i].time && t < points[i + 1].time)
            {
                p0 = points[i];
                p1 = points[i + 1];
                break;
            }
        }

        float localT = Mathf.InverseLerp(p0.time, p1.time, t);
        return Mathf.Lerp(getValue(p0), getValue(p1), localT);
    }
    
    public float SampleValue(float t)
    {
        float mean = SampleMean(t);
        float sd = SampleSD(t);
        return mean + Random.Range(-sd, sd); // Simple random sample within 1 SD
    }
}