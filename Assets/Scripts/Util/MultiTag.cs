using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

[DisallowMultipleComponent, ExecuteAlways]
public class MultiTag : MonoBehaviour
{
    [SerializeField] private List<string> _tags = new List<string>();
    public List<string> Tags => _tags;

    private void OnEnable()
    {
        GlobalTags.RegisterGameObjectTags(gameObject, Tags.ToArray());
    }

    private void OnDisable()
    {
        GlobalTags.UnregisterGameObjectTags(gameObject, Tags.ToArray());
    }
}
