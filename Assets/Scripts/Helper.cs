using UnityEngine;
using System.Collections;

public static class Helper {
    public static Vector2 ToVector2(Vector3 vec){
        return new Vector2(vec.x, vec.z); // Discard y
    }
}
