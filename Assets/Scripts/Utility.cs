using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityLib
{
    public static class Utility
    {
        public static void RestrictBetween(ref this float toRestrict, float min, float max, bool loop = false)
        {
            // Verify syntax
            if (min > max)
                Debug.LogError("Cannot RestrictBetween when minimum value is bigger than maximum value");

            // Restrict
            if (loop) toRestrict += toRestrict < min ? max - min : toRestrict > max ? min - max : 0;
            else toRestrict = toRestrict < min ? min : toRestrict > max ? max : toRestrict;
        }
    }
}