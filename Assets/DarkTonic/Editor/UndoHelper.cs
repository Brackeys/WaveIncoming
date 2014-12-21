#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
   // Undo API 
#else
    using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

public static class UndoHelper {
    public static void CreateObjectForUndo(GameObject go, string actionName) {
        #if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
            // No Undo API 
        #else 
            // New Undo API
            Undo.RegisterCreatedObjectUndo(go, actionName);
        #endif
    }

	public static void SetTransformParentForUndo(Transform child, Transform newParent, string name) {
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// No Undo API 
		#else 
			// New Undo API
			Undo.SetTransformParent(child, newParent, name);
		#endif
	}

	public static void RecordObjectPropertyForUndo(Object objectProperty, string actionName) {
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// No Undo API 
		#else 
			// New Undo API
			Undo.RecordObject(objectProperty, actionName);
		#endif
	}

	public static void RecordObjectsForUndo(Object[] objects, string actionName) {
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		// No Undo API 
		#else 
			// New Undo API
			Undo.RecordObjects(objects, actionName);
		#endif
	}

	public static void DestroyForUndo(GameObject go) {
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			GameObject.DestroyImmediate(go);
		#else 
			// New Undo API
			Undo.DestroyObjectImmediate(go);
		#endif
	}
}
