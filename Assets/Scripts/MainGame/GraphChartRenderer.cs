using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// DOCS: https://bitsplash.io/docs/tutorials/graph-chart

public class GraphChartRenderer : MonoBehaviour
{
    public GraphChart graph;

    // In Unity, Graph object, in the inspector, find the Data section of 'GraphChart'
    private string[] categoryNames;
    private int pointsPerSegment = 10;

    void Start()
    {
        if (graph == null)
        {
            Debug.LogError("GraphChartBase component is missing!");
            graph = GetComponent<GraphChart>();
        } else
        {
            Debug.Log("Found GraphChartBase");
        }

        // Set category names based on the selected mode
        switch (SegmentsManager.instance.currentLevelConfig.levelNumber)
        {
            case 1: categoryNames = new string[] { "Line1", "Line2", "Line3" }; break;
            case 2: categoryNames = new string[] { "Line1", "Line2", "Line3", "Line4", "Line5" }; break;
            default: categoryNames = new string[] { "Line1", "Line2", "Line3", "Line4", "Line5" }; break;
        }

        // Setting x and y axis origin
        graph.DataSource.VerticalViewOrigin = 0;
        graph.DataSource.HorizontalViewOrigin = 0;

        // TODO: There has to be a better way to do this but this is my workaround lol
        // What I'm doing: I made an empty graph that is a line from (0,0) -> (50,10) so that
        // the x-axis is always atleast 0-50 and y-axis is atleast 0-10
        // TODO: If it is desired that the graph will cut off anything beyond x=50, this is not doing that yet
        graph.DataSource.StartBatch();
        graph.DataSource.ClearCategory("Empty");
        graph.DataSource.AddPointToCategory("Empty", 0.0, 0.0);
        graph.DataSource.AddPointToCategory("Empty", 50.0, 10.0);
        graph.DataSource.EndBatch();
    }

    public void DrawTrack()
    {
        if (SegmentsManager.instance == null)
        {
            Debug.Log("Error: No segments manager instance.");
        } else
        {
            Debug.Log("segments manager found in graph chart render draw track");
        }

        if (graph != null)
        {
            graph.DataSource.StartBatch();

            foreach (string categoryName in categoryNames)
            {
                graph.DataSource.ClearAndMakeBezierCurve(categoryName);
            }

            for (int index = 0; index < categoryNames.Length; index++) // Loop through indices 0 to 2
            {
                Segment segment = SegmentsManager.instance.GetSegment(index);
                if (segment == null) continue; // Skip if the segment is null

                // Define the initial point for the curve
                float startX = (float)segment.XStart;
                float startY = segment.Func.Evaluate(startX);
                graph.DataSource.SetCurveInitialPoint(categoryNames[index], startX, startY);

                // Add points to approximate the curve for this segment
                float segmentLength = (float)(segment.XEnd - segment.XStart);
                for (int i = 1; i <= pointsPerSegment; i++)
                {
                    float t = i / (float)pointsPerSegment; // Fractional position along the segment
                    float x = Mathf.Lerp((float)segment.XStart, (float)segment.XEnd, t); // Interpolate X
                    float y = segment.Func.Evaluate(x); // Compute Y using the segment's function
                    graph.DataSource.AddLinearCurveToCategory(categoryNames[index], new DoubleVector2(x, y)); // Add the point
                }
            }

            foreach (string categoryName in categoryNames)
            {
                graph.DataSource.MakeCurveCategorySmooth(categoryName);
            }

            graph.DataSource.EndBatch();

            foreach (string categoryName in categoryNames)
            {
                graph.DataSource.AnimateCurve(categoryName, 1.0f);
            }

        } else
        {
            Debug.Log("Error: NO GRAPH CHART BASE");
        }
    }

    IEnumerator ClearAll()
    {
        yield return new WaitForSeconds(5f);
        GraphChart graph = GetComponent<GraphChart>();

        graph.DataSource.Clear();
    }
}



