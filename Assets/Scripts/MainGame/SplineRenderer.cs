using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwissArmySpline;

public class SplineRenderer : MonoBehaviour
{
    private int numPoints = 30;
    public Vector3[] positions;

    public SAS_CurveTools swissArmySpline;
    public float tangentFactor = 0.4f;
    public float secondOrderOffsetFactor = 0.276142375f;
    public float realignmentFactor = 1;
    public float maxYUnadjusted;
    public float divisor;
    private const float MAX_Y_CONSTANT = 10f; // Example constant to scale the Y-values within range

    // Start is called before the first frame update
    void Start()
    {
        // Find the existing SegmentsManager in the scene
        //segmentsManager = FindObjectOfType<SegmentsManager>();

        if (SegmentsManager.instance != null)
        {
            // Assign it to the SplineRenderer's slot
            //GetComponent<SplineRenderer>().segmentsManager = segmentsManager;
        }
        else
        {
            Debug.LogError("SegmentsManager not found in the scene!");
        }

        maxYUnadjusted = GetMaxYValueFromSegments(); // Find max y value from all segments
        divisor = maxYUnadjusted / MAX_Y_CONSTANT; // Calculate the divisor to adjust the coefficients
        List<Vector3> temp = new(); // List to hold adjusted segment positions

        foreach (Segment segment in SegmentsManager.instance.segments)
        {
            if (segment == null)
            {
                Debug.Log("Segment is null");
                continue;
            }

            // Create adjusted function for the segment
            Function adjustedFunc = AdjustFunctionCoefficients(segment.Func, divisor);

            for (int i = 0; i <= numPoints; i++)
            {
                // Normalize `i` to get a value from 0 to 1, then use that to interpolate `x`
                float t = i / (float)(numPoints - 1);
                float x = Mathf.Lerp((float)segment.XStart, (float)segment.XEnd, (float)t); // Interpolate x
                float y = adjustedFunc.Evaluate(x); // Use adjusted function at x

                temp.Add(new Vector3((float)x, (float)y, 0));
            }
        }

        positions = temp.ToArray();

        // Set up and calculate the spline with adjusted positions
        swissArmySpline.points = SetupSmoothCurve(positions);
        swissArmySpline.CalculateLength();
        swissArmySpline.Execute();
        try
        {
            swissArmySpline.changePillarLength(MAX_Y_CONSTANT);
            Debug.Log($"Change pillar length to {MAX_Y_CONSTANT}");
        } catch 
        {
            Debug.LogError("Cannot change pillar length.");
        }
        SetupAnimation();
    }

    private void SetupAnimation()
    {
        SAS_Animator animator = FindObjectOfType<SAS_Animator>();
        if (animator == null)
        {
            Debug.LogError("SAS_Animator not found in the scene!");
            return;
        }

        // Get the first segment and extract the startY
        Segment firstSegment = SegmentsManager.instance.GetSegment(0);
        if (firstSegment == null)
        {
            Debug.LogError("No first segment found!");
            return;
        }

        float startYUnadjusted = firstSegment.Func.Evaluate(firstSegment.XStart); // Y-value at start of the first segment
        if (startYUnadjusted >= maxYUnadjusted)
        {
            Debug.LogWarning("StartY is already at or above maxY. No initial speed needed.");
            animator.currentSpeed = 0f;
            return;
        }

        float gravity = Mathf.Abs(Physics.gravity.y);
        float requiredInitialSpeedUnadjusted = Mathf.Sqrt(2 * gravity * (maxYUnadjusted - startYUnadjusted)); // Physics formula
        animator.currentSpeed = requiredInitialSpeedUnadjusted / Mathf.Sqrt(divisor);
        animator.useGravity = true;
        animator.flipForward = false; // Ensure left to right motion
        animator.drag = 0f;

        Debug.Log($"Initial Speed Set: {requiredInitialSpeedUnadjusted} m/s to reach maxY: {maxYUnadjusted} from startY: {startYUnadjusted}");
    }

    private float GetMaxYValueFromSegments()
    {
        float maxY = float.MinValue;

        foreach (Segment segment in SegmentsManager.instance.segments)
        {
            if (segment == null) continue;

            // Get min and max values over the range
            (float globalMinY, float globalMaxY, float localMinY, float localMaxY) = 
                segment.FindMinMaxOverRange(segment.Func, segment.XStart, segment.XEnd);

            // Check if we found the first local maximum
            if (globalMaxY > maxY)
            {
                maxY = globalMaxY;
            }
        }

        return maxY;
    }

    private Function AdjustFunctionCoefficients(Function originalFunc, float divisor)
    {
        // Return a new function with the adjusted coefficients
        return new Function(
            originalFunc.A / divisor,
            originalFunc.B / divisor,
            originalFunc.C / divisor,
            originalFunc.D / divisor
        );
    }

    List<SAS_CurveTools_Helper.Point> SetupSmoothCurve(Vector3[] positions)
    {
        List<SAS_CurveTools_Helper.Point> points = new();
        int count = positions.Length;
        if (count <= 0)
        {
            Debug.LogWarning("No points");
        }

        // Set an initial up vector and previous tangent
        Vector3 initialUp = Vector3.up; // Starting up vector
        Vector3 previousTangent = GetFirstOrderNeighbor(0, 1).normalized;

        // Main loop to set up each point
        for (int i = 0; i < count; i++)
        {
            SAS_CurveTools_Helper.Point point = new()
            {
                position = positions[i]
            };

            // Get or simulate first-order neighbors
            Vector3 toPrev = GetFirstOrderNeighbor(i, -1);
            Vector3 toNext = GetFirstOrderNeighbor(i, 1);

            // Get or simulate second-order neighbors based on first-order neighbors
            Vector3 toSecondPrev = GetSecondOrderNeighbor(i, -1, toPrev) * secondOrderOffsetFactor;
            Vector3 toSecondNext = GetSecondOrderNeighbor(i, 1, toNext) * secondOrderOffsetFactor;

            toSecondPrev *= 1 - Mathf.Abs(Vector3.Dot(toPrev.normalized, toSecondPrev.normalized));
            toSecondNext *= 1 - Mathf.Abs(Vector3.Dot(toNext.normalized, toSecondNext.normalized));

            // Calculate tangent directions with second-order offset
            Vector3 tangentInDir = (toSecondPrev - toPrev);
            Vector3 tangentOutDir = (toNext - toSecondNext);

            // Calculate the current tangent as the normalized average direction
            Vector3 tangent = (tangentInDir + tangentOutDir).normalized;

            if (i == 0)
            {
                // Initialize the up vector and rotation at the first point
                point.rotation = Quaternion.LookRotation(tangent, initialUp);
            }
            else
            {
                // Calculate rotation adjustment to align previous tangent with the current tangent
                Quaternion rotationAdjustment = Quaternion.FromToRotation(previousTangent, tangent);
                initialUp = rotationAdjustment * initialUp; // Rotate the up vector accordingly

                // Apply a gradual blend towards global up
                initialUp = Vector3.Slerp(initialUp, Vector3.up, realignmentFactor * Mathf.Clamp01(Vector3.Dot(initialUp, Vector3.up))).normalized;

                point.rotation = Quaternion.LookRotation(tangent, initialUp);
            }

            // Store the tangent for the next iteration
            previousTangent = tangent;

            // Set tangent lengths based on distances to neighbors
            point.tangentIn = toPrev.magnitude * tangentFactor;
            point.tangentOut = toNext.magnitude * tangentFactor;

            points.Add(point);
        }
        return points;


        // Helper to fetch or simulate first-order neighbors
        Vector3 GetFirstOrderNeighbor(int currentIndex, int offset)
        {
            int neighborIndex = currentIndex + offset;
            if (neighborIndex >= 0 && neighborIndex < count)
            {
                return positions[neighborIndex] - positions[currentIndex];
            }
            else
            {
                // Simulate by flipping the direction of the opposite neighbor
                return -GetFirstOrderNeighbor(currentIndex, -offset);
            }
        }

        // Helper to fetch or simulate second-order neighbors
        Vector3 GetSecondOrderNeighbor(int currentIndex, int offset, Vector3 firstOrderDir)
        {
            int neighborIndex = currentIndex + 2 * offset;
            if (neighborIndex >= 0 && neighborIndex < count)
            {
                return positions[neighborIndex] - positions[currentIndex + offset];
            }
            else
            {
                // Simulate by using the same direction as the first-order neighbor
                return firstOrderDir;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (positions == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 0.25f);
        }
    }
}
