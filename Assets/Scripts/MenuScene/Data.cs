using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Data : MonoBehaviour
{
    public struct datum
    {
        public System.DateTime date;
        public string playerID;
        public string groupID;
        public int level;
        public float eq1a;
        public float eq1b;
        public float eq1c;
        public float x1max;
        public float eq2a;
        public float eq2b;
        public float eq2c;
        public float x2max;
        public float eq3a;
        public float eq3b;
        public float eq3c;
        public float x3max;
        public bool mathCheck;
        public float score;
        public bool success;
    }
}