using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelConfig", menuName = "Level Configuration")]
public class LevelConfig : ScriptableObject
{
    public int levelNumber;
    public float maxTotalLength;
    public int maxTotalSegments;
    public bool isSupportCubic;

    public void ApplyConfig()
    {
        Debug.Log($"Applying Level {levelNumber} Config: Max Length = {maxTotalLength}, Max Segments = {maxTotalSegments}, support cubic function = {isSupportCubic}");
    }
}
