using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MondayOFF {
    public class LabelOverrideAttribute : PropertyAttribute {
        public string displayName;
        public LabelOverrideAttribute(string displayName) {
            this.displayName = displayName;
        }
    }
}