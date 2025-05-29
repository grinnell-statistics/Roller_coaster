using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;

/**
 * Manages track segments in a persistent singleton.
 * Handles storing `Segment` segments, validating/math-checking the whole design.
 * Is attached and initialized by an object in the `MainGame` scene.
 * To use it in other scripts after `MainGame` scene, use `SegmentManager.instance`.
 * 
 * Main usage:
 * use `SegmentsManager.instance.ValidateLevel1` or `SegmentsManager.instance.ValidateLevel2` for math checks
 */
public class SegmentsManager : MonoBehaviour
{
    // static instance
    public static SegmentsManager instance;

    // HOW TO USE THIS
    // This LevelConfig object is assigned in the Unity inspector
    public LevelConfig currentLevelConfig;

    // current level
    private int levelNumber;

    // maximum number of segments allowed of the whole game
    private int totalNumSegs;

    // RULE: Level 1 is 50, Level 2 is 100
    private float targetLength;

    // storing all the segments
    // design decision: this array is not dynamic and we initialize it with totalNumSegs
    // but for some levels or player inputs, some segment will be left `null`
    public Segment[] segments;

    private List<Segment> validSegments = new List<Segment>();

    // MATH-CHECK HINTS
    // store the strings for math-check hints
    private string errorText;

    private const float Tolerance = 0.1001f; // Tolerance for numerical comparisons
    private const float MaxHeight = 500f;
 
    /// <summary>
    /// Retrieves the stored validation error message.
    /// </summary>
    /// <returns>Validation message</returns>
    public string GetHint()
    {
        return errorText;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (currentLevelConfig == null)
        {
            Debug.LogError("LevelConfig not assigned!");
            return;
        } else
        {
            levelNumber = currentLevelConfig.levelNumber;
            totalNumSegs = currentLevelConfig.maxTotalSegments;
            targetLength = currentLevelConfig.maxTotalLength;
            segments = new Segment[totalNumSegs];
        }

        Debug.Log("SegmentsManager is persisting across scenes.");

        foreach (Segment segment in segments)
        {
            Debug.Log(segment != null ? $"Segment: {segment}" : "Segment is null");
        }
    }

    public LevelConfig GetLevelConfig()
    {
        return currentLevelConfig;
    }

    /// <summary>
    /// Assigns a segment to a specific index.
    /// </summary>
    /// <param name="index">Index in the segment array</param>
    /// <param name="segment">Segment to be assigned</param>
    public void SetSegment(int index, Segment segment)
    {
        if (index >= 0 && index < segments.Length)
        {
            segments[index] = segment;
        }
    }

    public void DestroySegmentsManager()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    /// <summary>
    /// Clears all stored segments.
    /// </summary>
    public void ClearAll()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i] = null;
        }

        Debug.Log("All segments cleared.");
    }

    /// <summary>
    /// Clears a specific segment at the given index.
    /// </summary>
    /// <param name="index">Index of the segment to be cleared</param>
    public void ClearSegment(int index)
    {
        if (index >= 0 && index < segments.Length)
        {
            segments[index] = null;
            Debug.Log($"Segment at index {index} cleared.");
        }
        else
        {
            Debug.LogError($"Index {index} is out of bounds. Cannot clear segment.");
        }
    }

    /// <summary>
    /// Retrieves a segment at a given index.
    /// </summary>
    /// <param name="index">Index of the segment</param>
    /// <returns>The segment at the specified index, or null if out of bounds</returns>
    public Segment GetSegment(int index)
    {
        return (index >= 0 && index < segments.Length) ? segments[index] : null;
    }

    private void CleanSegments()
    {
        validSegments.Clear();
        foreach (Segment segment in segments)
        {
            if (segment != null)
            {
                validSegments.Add(segment);
            }
        }
    }

    // Helper validate function. Any checks that is universal should go inside here.
    private bool BaseValidate(out string baseValidationMessage)
    {
        CleanSegments();

        bool isValid = true;

        StringBuilder logBuilder = new StringBuilder();

        // Check if there is any segment at all
        if (validSegments.Count == 0)
        {
            logBuilder.AppendLine("Have you added any track?");
            isValid = false;
        }

        // Validate sequential segments
        for (int i = 0; i < validSegments.Count - 1; i++)
        {
            Segment curr = validSegments[i];
            Segment next = validSegments[i + 1];

            if (!curr.IsValidWithNext(next))
            {
                logBuilder.AppendLine($"Recheck the continuity and/or differentiability of {curr} and {next} at x = {curr.XEnd}.");
                isValid = false;
            }
        }

        // No underground
        for (int i = 0; i < validSegments.Count; i++)
        {
            Segment curr = validSegments[i];
            if (!curr.IsNotUnderground())
            {
                logBuilder.AppendLine("You can't build your roller coaster underground!");
                isValid = false;
            }
        }

        Segment firstSeg = validSegments[0];
        
        // must start at x = 0, y = 0
        if (firstSeg.XStart <  -Tolerance || firstSeg.XStart > Tolerance || firstSeg.Func.Evaluate(firstSeg.XStart) < -Tolerance | firstSeg.Func.Evaluate(firstSeg.XStart) > Tolerance) {
            logBuilder.AppendLine("Your roller coaster must start at the origin (0,0).");
            isValid = false;
        }

        // Ensure the first segment goes up from the start
        if (firstSeg.Func.FirstDerivative(firstSeg.XStart) <= 0)
        {
            logBuilder.AppendLine("Your roller coaster must start with an upward slope.");
            isValid = false;
        }

        // Max height <= 500
        float maxY = float.MinValue;
        foreach (Segment segment in segments)
        {
            if (segment != null)
            {
                // Calculate the global min and max Y values for this segment
                (float globalMinY, float globalMaxY, float localMinY, float localMaxY) =
                    segment.FindMinMaxOverRange(segment.Func, segment.XStart, segment.XEnd);

                // Update the max Y if this segment's max Y is greater
                if (globalMaxY > maxY)
                {
                    maxY = globalMaxY;
                }
            }
        }
        if (maxY > MaxHeight + Tolerance)
        {
            logBuilder.AppendLine($"The roller coaster exceeds the maximum height limit of {MaxHeight}. Maximum height reached: {maxY}");
            isValid = false;
        }

        baseValidationMessage = logBuilder.ToString();
        return isValid;
    }

    // Only validate for level 1 rules
    private bool ValidateLevel1(out string validationMessageLevel1) {
        bool isValid = true;
        StringBuilder logBuilder = new StringBuilder();
        isValid = BaseValidate(out string baseValidationMessage);
        logBuilder.AppendLine(baseValidationMessage);

        // last track's xEnd needs to be target length
        if (validSegments[validSegments.Count - 1].XEnd != targetLength)
        {
            logBuilder.AppendLine($"Your last track's xEnd needs to be {targetLength}");
            isValid = false;
        }

        // total length must be target length
        float totalLength = 0;
        foreach (Segment segment in validSegments)
        {
            totalLength += segment.Length;
        }

        if (Mathf.Abs(totalLength - targetLength) > Tolerance)
        {
            logBuilder.AppendLine($"Your design total length needs to be {targetLength}");
            isValid = false;
        }

        // If all checks pass
        if (isValid)
        {
            logBuilder.AppendLine("Looking good!");
        }

        validationMessageLevel1 = logBuilder.ToString();
        return isValid;
    }

    // Only validate for level 2 - more complex than level 1
    private bool ValidateLevel2(out string validationMessageLevel2)
    {
        bool isValid = true;
        StringBuilder logBuilder = new StringBuilder();
        isValid = BaseValidate(out string baseValidationMessage);
        logBuilder.AppendLine(baseValidationMessage);

        // last track's xEnd needs to be target length
        if (validSegments[validSegments.Count - 1].XEnd > targetLength)
        {
            logBuilder.AppendLine($"Your last track's xEnd must not exceed {targetLength}.");
            isValid = false;
        }

        // total length must be UPTO target length
        float totalLength = 0;
        foreach (Segment segment in validSegments)
        {
            totalLength += segment.Length;
        }

        if (totalLength > targetLength + Tolerance)
        {
            logBuilder.AppendLine($"Your design total length must not exceed {targetLength}.");
            isValid = false;
        }

        Segment firstSeg = validSegments[0];
        Segment lastSeg = validSegments[validSegments.Count - 1];

        // must ends at 0 <= y <= 1 (x = targetLength not necessary)
        if (!(-Tolerance <= lastSeg.Func.Evaluate(lastSeg.XEnd) && lastSeg.Func.Evaluate(lastSeg.XEnd) <= (1 + Tolerance)))
        {
            logBuilder.AppendLine($"Your roller coaster must end at height between 0 and 1.");
            isValid = false;
        }

        // Find the first local maximum and enforce a decreasing pattern
        float firstLocalMax = float.MaxValue;
        bool foundFirstMax = false;

        for (int i = 0; i < validSegments.Count; i++)
        {
            Segment segment = validSegments[i];

            // Extract min/max values
            (float globalMinY, float globalMaxY, float localMinY, float localMaxY) = 
                segment.FindMinMaxOverRange(segment.Func, segment.XStart, segment.XEnd);

            // Find the first true local maximum
            if (!foundFirstMax && localMaxY != float.MinValue) 
            {
                firstLocalMax = localMaxY;
                foundFirstMax = true;
            }
            else if (foundFirstMax && globalMaxY > firstLocalMax + Tolerance)
            {
                logBuilder.AppendLine($"Maximum at segment {i} (y={globalMaxY}) exceeds first local max (y={firstLocalMax}).");
                isValid = false;
            }
        }

        // speed, velo??

        // If all checks pass
        if (isValid)
        {
            logBuilder.AppendLine("Looking good!");
        }

        validationMessageLevel2 = logBuilder.ToString();
        return isValid;
    }

    // Handle applying the correct level's validation rule
    public bool Validate(out string validationMessage)
    {
        bool isValid = true; // Flag to track overall validity

        switch (levelNumber)
        {
            // Level 1
            case 1:
                {
                    isValid = ValidateLevel1(out string validationMessageLevel1);
                    validationMessage = validationMessageLevel1;
                    return isValid;
                }
            // Level 2
            case 2:
                {
                    isValid = ValidateLevel2(out string validationMessageLevel2);
                    validationMessage = validationMessageLevel2;
                    return isValid;
                }
            // Unexpected level
            default:
                {
                    validationMessage = "No level assigned.";
                    return isValid;
                }
        }
    }

    public void ValidateAndDisplay()
    {
        DataManager.gameData.mathCheck = true;
        Debug.Log("MathCheck change to true");
        if (Validate(out string validationMessage))
        {
            Debug.Log(validationMessage); // Log success
        }
        else
        {
            Debug.LogError(validationMessage); // Log error
        }

        errorText = validationMessage;
    }
}

