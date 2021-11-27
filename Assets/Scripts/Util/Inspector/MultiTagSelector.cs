//Author: Charles Osberg
//Filename: MultiTagSelector.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


[Serializable]
public class MultiTagSelector
{
    [SerializeField] private List<string> storedTags = new List<string>();

    public static implicit operator List<string>(MultiTagSelector selector) => selector.storedTags.AsReadOnly().ToList();
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MultiTagSelector))]
public class MultiTagSelectorDrawer : PropertyDrawer
{
    private bool listInit = false;
    private ReorderableList tagList;

    private void TagEdit(Rect rect, int index, bool isActive, bool isFocused)
    {
        
    }

    private string currentNewTag = "";

    private MultiTagSelector target;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!listInit)
        {
            Debug.Log(property.name);
            Debug.Log(property.propertyType);
            
            tagList = new ReorderableList(property.serializedObject,
                property,
                true,
                false,
                false,
                false);
            tagList.drawElementCallback += TagEdit;
            listInit = true;
        }
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        {
            tagList.DoLayoutList();
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
        
            currentNewTag = EditorGUI.TextField(new Rect(20, tagList.GetHeight(), 350, EditorGUIUtility.singleLineHeight),currentNewTag);
            if (GUI.Button(new Rect(370, tagList.GetHeight(), 50, EditorGUIUtility.singleLineHeight), "Add")
                || forceSubmit)
            {
                currentNewTag = currentNewTag.Trim();
                if (currentNewTag != "")
                {
                    //tagObj.Tags.Add(currentNewTag);
                    currentNewTag = "";
                    //EditorUtility.SetDirty(tagObj);
                }
 
            }

            if (forceClear)
            {
                currentNewTag = "";
            }
            
        }
        EditorGUI.EndProperty();
    }
}
#endif