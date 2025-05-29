using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public Function Func { get; private set; }
    public float XStart { get; private set; }
    public float XEnd { get; private set; }
    public float Length { get; private set; }
    private const float Tolerance = 0.1001f; // Tolerance for numerical comparisons
    
    public Segment (Function func, float xStart, float xEnd)
    {
        Func = func;
        XStart = xStart;
        XEnd = xEnd;
        Length = xEnd - xStart;
    }

    public override string ToString()
    {
        return Func.ToString();
    }

    // RULE: check continuity and smoothness
    public bool IsValidWithNext(Segment nextSegment)
    {
        // Two sedment's need to continue from each other end/start x-point
        if (XEnd != nextSegment.XStart)
        {
            Debug.Log($"XEnd {XEnd} != nextSegment.XStart {nextSegment.XStart}");
            return false;
        }

        float crossPoint = XEnd;

        // Validate continuity and smoothness:
        // 1. f(crossPoint) = g(crossPoint) (continuity)
        // 2. f'(crossPoint) = g'(crossPoint) (smoothness)
        bool isContinuous = Mathf.Abs(Func.Evaluate(crossPoint) - nextSegment.Func.Evaluate(crossPoint)) <= Tolerance;
        bool isSmooth = Mathf.Abs(Func.FirstDerivative(crossPoint) - nextSegment.Func.FirstDerivative(crossPoint)) <= Tolerance;
        return isContinuous && isSmooth;
    }

    // RULE: check y always stay at or above x-axis
    public bool IsNotUnderground()
    {
        float yStart = Func.Evaluate(XStart);
        float yEnd = Func.Evaluate(XEnd);

        // If there's a sign change between start and end, function crosses y=0
        if (yStart < -Tolerance || yEnd < -Tolerance) return false;

        // If both endpoints are above ground, check local minima
        (float globalMinY, float globalMaxY, float localMinY, float localMaxY) = FindMinMaxOverRange(Func, XStart, XEnd);
        if (globalMinY < -Tolerance) 
        {
            Debug.Log($"Function dips below 0, lowest y={globalMinY}");
            return false;
        }

        return true;
    }

    public (float globalMinY, float globalMaxY, float localMinY, float localMaxY) FindMinMaxOverRange(Function func, float xStart, float xEnd)
    {
        float globalMinY = Mathf.Min(func.Evaluate(xStart), func.Evaluate(xEnd)); // Start with endpoints
        float globalMaxY = Mathf.Max(func.Evaluate(xStart), func.Evaluate(xEnd));

        float localMinY = float.MaxValue;
        float localMaxY = float.MinValue;
        
        float a = 3 * func.D;
        float b = 2 * func.C;
        float c = func.B;

        if (a == 0) // Quadratic case
        {
            if (b != 0)
            {
                float criticalX = -c / b;
                if (criticalX >= xStart && criticalX <= xEnd)
                {
                    float criticalY = func.Evaluate(criticalX);
                    globalMinY = Mathf.Min(globalMinY, criticalY);
                    globalMaxY = Mathf.Max(globalMaxY, criticalY);

                    // Determine if it's a local min or max using f''(x)
                    if (func.SecondDerivative(criticalX) > 0) localMinY = criticalY;
                    else localMaxY = criticalY;
                }
            }
        }
        else
        {
            float discriminant = b * b - 4 * a * c;
            if (discriminant >= 0)
            {
                float sqrtD = Mathf.Sqrt(discriminant);
                float x1 = (-b + sqrtD) / (2 * a);
                float x2 = (-b - sqrtD) / (2 * a);

                if (x1 >= xStart && x1 <= xEnd)
                {
                    float y1 = func.Evaluate(x1);
                    globalMinY = Mathf.Min(globalMinY, y1);
                    globalMaxY = Mathf.Max(globalMaxY, y1);

                    if (func.SecondDerivative(x1) > 0) localMinY = Mathf.Min(localMinY, y1);
                    else localMaxY = Mathf.Max(localMaxY, y1);
                }
                if (x2 >= xStart && x2 <= xEnd)
                {
                    float y2 = func.Evaluate(x2);
                    globalMinY = Mathf.Min(globalMinY, y2);
                    globalMaxY = Mathf.Max(globalMaxY, y2);

                    if (func.SecondDerivative(x2) > 0) localMinY = Mathf.Min(localMinY, y2);
                    else localMaxY = Mathf.Max(localMaxY, y2);
                }
            }
        }

        return (globalMinY, globalMaxY, localMinY, localMaxY);
    }

}
