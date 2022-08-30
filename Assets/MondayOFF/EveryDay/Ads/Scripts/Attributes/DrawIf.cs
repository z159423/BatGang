using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute {
    public string comparedPropertyName { get; private set; }
    public string displayName { get; private set; }

    public DrawIfAttribute(string comparedPropertyName, string displayName = null) {
        this.comparedPropertyName = comparedPropertyName;
        this.displayName = displayName;
    }
}