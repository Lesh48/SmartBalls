using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTreck : MonoBehaviour
{
    public int num_places;

    public bool use_defaults;

    public class Objects_ar : MonoBehaviour
    {
        public bool use_high;
        public bool use_medium;
        public bool use_small;

        public float[] high;  //chances
        public float[] medium;
        public float[] small;
        
    }

    public Objects_ar objects_ar;
    
}
