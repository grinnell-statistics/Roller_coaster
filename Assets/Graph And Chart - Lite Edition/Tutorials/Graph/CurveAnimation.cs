using UnityEngine;
using ChartAndGraph;
using System.Collections;

public class CurveAnimation : MonoBehaviour
{
	void Start ()
    {
        GraphChartBase graph = GetComponent<GraphChartBase>();
        if (graph != null)
        {
            
            graph.Scrollable = false;
            graph.HorizontalValueToStringMap[0.0] = "0"; // example of how to set custom axis strings
            graph.DataSource.StartBatch();
            graph.DataSource.ClearAndMakeBezierCurve("Player 2");

            for (int i = 0; i < 30; i++)
            {
                if (i == 0)
                    graph.DataSource.SetCurveInitialPoint("Player 2", 0f, Random.value * 10f + 10f);
                else
                    graph.DataSource.AddLinearCurveToCategory("Player 2",
                                                                    new DoubleVector2(i * 10f / 30f, Random.value * 10f + 10f));
            }
            graph.DataSource.MakeCurveCategorySmooth("Player 2");
            graph.DataSource.EndBatch();
            graph.DataSource.AnimateCurve("Player 2", 1.0f);
        }
        // StartCoroutine(ClearAll());
    }

    IEnumerator ClearAll()
    {
        yield return new WaitForSeconds(5f);
        GraphChartBase graph = GetComponent<GraphChartBase>();

        graph.DataSource.Clear();
    }
}
