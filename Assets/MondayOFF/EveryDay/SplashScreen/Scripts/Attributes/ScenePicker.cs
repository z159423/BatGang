using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ScenePickerAttribute : PropertyAttribute {
    public ScenePickerAttribute() { }
}