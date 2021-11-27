//Author: Charles Osberg
//Filename: MultiTagEditor.cs
//
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(MultiTag))]
public class MultiTagEditor : Editor
{
    private MultiTag tagObj = null;

    private ReorderableList list = null;
    private void TagEdit(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= tagObj.Tags.Count) return;
        
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        
        var tagList = GlobalTags.AllTags.ToList();
        var curSelection = tagList.IndexOf(element.stringValue);

        if (curSelection == -1)
        {
            curSelection = tagList.Count;
            tagList.Add(tagObj.Tags[index]);
        }

        var selection = EditorGUI.Popup(
            new Rect(rect.x, rect.y, rect.width - 100, rect.height),
            curSelection,
            tagList.ToArray()
        );

        if (curSelection != selection)
        {
            tagObj.Tags[index] = GlobalTags.AllTags[selection];
            EditorUtility.SetDirty(tagObj);
        }

        if (GlobalTags.AllTags.Contains(tagObj.Tags[index]))
        {
            if (GUI.Button(new Rect(rect.x + rect.width - 100, rect.y, 80, EditorGUIUtility.singleLineHeight), "Unregister"))
            {
                GlobalTags.DestroyTag(tagObj.Tags[index]);
            }
        }
        else
        {
            if (GUI.Button(new Rect(rect.x + rect.width - 100, rect.y, 80, EditorGUIUtility.singleLineHeight), "Register"))
            {
                GlobalTags.AddTags(new List<string>{tagObj.Tags[index]});
            }
        }


        if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight), "X"))
        {
            tagObj.Tags.RemoveAt(index);
            EditorUtility.SetDirty(tagObj);
        }
    }
    
    public void OnEnable()
    {
        tagObj = target as MultiTag;

        Debug.Assert(tagObj != null, nameof(tagObj) + " != null - Something has gone very wrong.");
        
        list = new ReorderableList(serializedObject,
            serializedObject.FindProperty("_tags"),
            true,
            false,
            false,
            false);

        list.drawElementCallback += TagEdit;
    }

    private string currentNewTag = "";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();

        var forceSubmit = false;
        var forceClear = false;
        var curKeyboardInput = Event.current;
        if (curKeyboardInput.isKey)
        {
            if (curKeyboardInput.keyCode == KeyCode.KeypadEnter
                || curKeyboardInput.keyCode == KeyCode.Return)
            {
                forceSubmit = true;
            }
            else if (curKeyboardInput.keyCode == KeyCode.Escape)
            {
                forceClear = true;
            }
        }
        
        currentNewTag = EditorGUI.TextField(new Rect(20, list.GetHeight(), 350, EditorGUIUtility.singleLineHeight),currentNewTag);
        if (GUI.Button(new Rect(370, list.GetHeight(), 50, EditorGUIUtility.singleLineHeight), "Add")
        || forceSubmit)
        {
            currentNewTag = currentNewTag.Trim();
            if (currentNewTag != "")
            {
                tagObj.Tags.Add(currentNewTag);
                currentNewTag = "";
                EditorUtility.SetDirty(tagObj);
            }
 
        }

        if (forceClear)
        {
            currentNewTag = "";
        }
        
        GUILayout.Space(20);
            
        serializedObject.ApplyModifiedProperties();

    }
}

#endif