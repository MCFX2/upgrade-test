using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

/// <summary>
/// Serializes the function given its name and
/// creates a button that runs the function in the inspector.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public string NameOverride { get; private set; }

    /// <summary>
    /// Create a new instance of the attribute.
    /// </summary>
    /// <param name="nameOverride">The text to put on the button. If none is specified, uses the function name.</param>
    public InspectorButtonAttribute(string nameOverride = null)
    {
        NameOverride = nameOverride;
    }
}

#if UNITY_EDITOR
public class ButtonDrawer : BasicInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var monoTarget = target as MonoBehaviour;

        Debug.Assert(monoTarget != null, nameof(monoTarget) + " != null, something has gone very wrong.");
        
        //Reflect to get method or methods
        var methods = monoTarget.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(info => Attribute.IsDefined(info, typeof(InspectorButtonAttribute)));

        //Iterate and create buttons
        foreach (var memberInfo in methods)
        {
            var printName = memberInfo.Name;
            var attributeOverride = memberInfo.GetCustomAttributes()
                .Where(attribute => attribute is InspectorButtonAttribute);

            foreach (var attribute in attributeOverride)
            {
                var trueAttribute = attribute as InspectorButtonAttribute;
                if (trueAttribute.NameOverride != null)
                {
                    printName = trueAttribute.NameOverride;
                }
            }
            
            if (GUILayout.Button(printName))
            {
                var method = memberInfo as MethodInfo;
                if (method != null)
                {
                    method.Invoke(monoTarget, null);
                }
            }
        }
    }
}
#endif
