using System;
using System.Collections.Generic;
using UnityEngine;


namespace B2B.GameKit.Maths
{
    public static class Maths
    {
        public static float SmoothStep(float min, float max, float x)
        {
            // Scale, bias and saturate x to 0..1 range
            float v = Mathf.Clamp((x - min) / (max - min), 0.0f, 1.0f);
            // Evaluate polynomial
            return v * v * (3 - 2 * v);
        }
        public static float BlendingSmoothStep(float min1, float max1, float min2, float max2, float x)
        {
            return SmoothStep(min1, max1, x) * (1 - SmoothStep(min2, max2, x));
        }

        public static float Smootherstep(float min, float max, float x)
        {
            // Scale, bias and saturate x to 0..1 range
            float v = Mathf.Clamp((x - min) / (max - min), 0.0f, 1.0f);
            // Evaluate polynomial
            return v * v * v * (v * (v * 6 - 15) + 10);
        }
        public static float BlendingSmootherstep(float min1, float max1, float min2, float max2, float x)
        {
            return Smootherstep(min1, max1, x) * (1 - Smootherstep(min2, max2, x));
        }


    }
}