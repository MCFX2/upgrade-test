#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Whenever externally referring to custom editors, use this class.
/// 
/// If you instead directly derive from one of the base classes of this class,
/// you will be reliant on ever-shifting class dependency lineage and
/// your code will eventually break.
///
/// This class guarantees to wrap all necessary attributes
/// and custom editors for the project (at least as far as MonoBehavior).
/// </summary>

[CustomEditor(typeof(MonoBehaviour), true)]
public class ExtendedInspector : ButtonDrawer
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}


/// <summary>
/// The starting point of all custom editors.
/// </summary>
public class BasicInspector : Editor
{
    protected virtual void OnEnable() { }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif