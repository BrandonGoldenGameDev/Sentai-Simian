using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Takes a vector, makes its y component 0, and normalizes it.
    /// </summary>
    /// <param name="vector">Vector to flatten</param>
    /// <returns></returns>
    public static Vector3 Flatten(this Vector3 vector)
    {
        vector.y = 0f;
        vector.Normalize();
        return vector;
    }
}
