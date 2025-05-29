using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalculateFinalScore : MonoBehaviour
{
    public AudioSource booSound;
    public AudioSource winSound;

    private int score;
    // UI Text for displaying score
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        // Find the existing SegmentsManager in the scene
        if (SegmentsManager.instance != null)
        {
            Debug.Log("Segments manager found");
        }
        else
        {
            Debug.LogError("ERROR: SegmentsManager not found in the scene!");
        }

        // Calculate and log the score
        score = (int) Calculate();
        scoreText.text = score.ToString();

        // Play sound according to score
        if (score < 1)
        {
            booSound.Play();
        } else if (score > 1)
        {
            winSound.Play();
        }

        Debug.Log($"Final Score: {score}");

        // Send data
        DataManager.gameData.score = score;
        DataManager.gameData.success = true;
        Debug.Log($"MathCheck is {DataManager.gameData.mathCheck} and Success is {DataManager.gameData.success}");
        StartCoroutine(DataManager.SendData());
        try
        {
            SegmentsManager.instance.ClearAll();
            Debug.Log("Reset segments manager");
        } catch 
        {
            Debug.LogError("Can't reset game");
        }

    }

    private float Calculate()
    {
        string validationMessage;
        if (!SegmentsManager.instance.Validate(out validationMessage))
        {
            Debug.LogError(validationMessage);
            return 0.0f; // Return 0 if validation fails
        }

        List<float> localExtrema = new List<float>(); // Stores alternating max and min heights
        bool searchingForMax = true;  // Start by searching for a maximum
        float lastXEnd = 0f;  // To store the last XEnd value

        // Iterate over each segment to extract extrema
        foreach (Segment segment in SegmentsManager.instance.segments)
        {
            if (segment == null) continue;

            // Get the global and local minima and maxima for this segment
            (float globalMinY, float globalMaxY, float localMinY, float localMaxY) = 
                segment.FindMinMaxOverRange(segment.Func, segment.XStart, segment.XEnd);

            // Add the extrema to the list
            if (searchingForMax)
            {
                if (localMaxY != float.MinValue)
                {
                    localExtrema.Add(localMaxY);  // Add local maximum
                    searchingForMax = false;  // Switch to searching for min
                }
            }
            else
            {
                if (localMinY != float.MaxValue)
                {
                    localExtrema.Add(localMinY);  // Add local minimum
                    searchingForMax = true;  // Switch back to searching for max
                }
            }

            lastXEnd = Mathf.Max(lastXEnd, segment.XEnd); // Update the last XEnd
        }

        // Add the height at the final point, which is the last XEnd
        float finalHeight = GetHeightAtX(lastXEnd);
        localExtrema.Add(finalHeight);  // Add the final point's height

        // Calculate score based on alternating local maxima and minima
        float totalScore = 0f;
        for (int i = 0; i < localExtrema.Count - 1; i++)
        {
            totalScore += Mathf.Abs(localExtrema[i] - localExtrema[i + 1]);
        }

        return totalScore;
    }

    private float GetHeightAtX(float x)
    {
        foreach (Segment segment in SegmentsManager.instance.segments)
        {
            if (segment != null && x >= segment.XStart && x <= segment.XEnd)
            {
                return segment.Func.Evaluate(x); // Evaluate the height using the segment's function
            }
        }

        return 0.0f; // Default to 0 if no valid segment is found
    }
}
