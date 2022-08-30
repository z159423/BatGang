using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace MondayOFF.EditorUtil {
    using Assembly = UnityEditor.Compilation.Assembly;

    public class AsmdefLinker : AssetPostprocessor {
        const string UNITY_PURCHASING = "UNITY_PURCHASING";
        const string FIREBASE_ENABLED = "FIREBASE_ENABLED";

        [System.Serializable]
        class Asmdef {
            public string name;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public bool noEngineReferences;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            // var inPackages = importedAssets.Any(path => path.StartsWith("com.unity.purchasing"))
            //                 || deletedAssets.Any(path => path.Contains("com.unity.purchasing"))
            //                 || movedAssets.Any(path => path.StartsWith("com.unity.purchasing"))
            //                 || movedFromAssetPaths.Any(path => path.StartsWith("com.unity.purchasing"));

            bool isImportingPurchasing = importedAssets.Any(path => path.Contains("com.unity.purchasing"));
            bool isDeletingPurchasing = deletedAssets.Any(path => path.Contains("com.unity.purchasing"));
            if (isImportingPurchasing) {
                DefinePurchasing(true);
            }
            if (isDeletingPurchasing) {
                DefinePurchasing(false);
            }

            System.Reflection.Assembly firebaseAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "Firebase.App");
            if (firebaseAssembly == null) {
                ModifyScriptingDefineSymbol(FIREBASE_ENABLED, false);
            } else {
                ModifyScriptingDefineSymbol(FIREBASE_ENABLED, true);
            }
        }

        // [DidReloadScripts(900)]
        private static void SetupEnvironment() {
            var purchasingAssemblyPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("UnityEngine.Purchasing");
            DefinePurchasing(!string.IsNullOrEmpty(purchasingAssemblyPath));

            // System.Reflection.Assembly firebaseAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "Firebase.App");
            // if (firebaseAssembly == null) {
            //     ModifyScriptingDefineSymbol(FIREBASE_ENABLED, false);
            // } else {
            //     ModifyScriptingDefineSymbol(FIREBASE_ENABLED, true);
            // }
        }

        private static void DefinePurchasing(bool isDefine) {
            ModifyScriptingDefineSymbol(UNITY_PURCHASING, isDefine);
            if (isDefine) {
                SetAsmdefReference(
                    new Dictionary<string, string[]>{
                        {"MondayOFFIAP", new string[] { "UnityEngine.Purchasing", "UnityEngine.Purchasing.Security", "UnityEngine.Purchasing.SecurityStub", "UnityEngine.Purchasing.SecurityCore", "UnityEngine.Purchasing.Stores", "Tangle" }},
                        {"Tangle", new string[] { "UnityEngine.Purchasing.Security", "UnityEngine.Purchasing.SecurityStub", "UnityEngine.Purchasing.SecurityCore" }},
                    }
                );
            } else {
                SetAsmdefReference(
                    new Dictionary<string, string[]>{
                        {"MondayOFFIAP", new string[] { }},
                        {"Tangle", new string[] { }},
                    }
                );
            }
        }

        private static void ModifyScriptingDefineSymbol(string symbol, bool isAdd) {
            BuildTargetGroup[] groups = new BuildTargetGroup[]{
                BuildTargetGroup.iOS, BuildTargetGroup.Android
            };

            foreach (var currentGroup in groups) {
                var definedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup);
                var defines = new List<string>(definedSymbols.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                if (isAdd) {
                    if (!defines.Contains(symbol)) {
                        defines.Add(symbol);
                    }
                } else {
                    if (defines.Contains(symbol)) {
                        defines.Remove(symbol);
                    }
                }

                // Debug.Log(string.Join(";", defines.ToArray()) + " : " + currentGroup.ToString());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentGroup, string.Join(";", defines.ToArray()));
            }
        }

        private static void SetAsmdefReference(Dictionary<string, string[]> asmdefReferenceDict) {
            Assembly[] playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            Assembly[] editorAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Editor);
            Assembly[] allAssemblies = new Assembly[playerAssemblies.Length + editorAssemblies.Length];
            playerAssemblies.CopyTo(allAssemblies, 0);
            editorAssemblies.CopyTo(allAssemblies, playerAssemblies.Length);

            bool hasChanged = false;
            foreach (var asmdefRefencePair in asmdefReferenceDict) {
                hasChanged |= AddReferenceToAsmdef(allAssemblies, asmdefRefencePair.Key, asmdefRefencePair.Value);
            }

            if (hasChanged) {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        private static bool AddReferenceToAsmdef(Assembly[] allAssemblies, string targetAssemblyName, string[] requiredReferences) {
            var iapAssemblyPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(targetAssemblyName);
            var iapAssemblyContent = File.ReadAllText(iapAssemblyPath);
            var asmdefObject = JsonUtility.FromJson<Asmdef>(iapAssemblyContent);

            if (asmdefObject.references.SequenceEqual(requiredReferences)) {
                return false;
            }

            asmdefObject.references = requiredReferences;

            var jsonOutput = JsonUtility.ToJson(asmdefObject);
            File.WriteAllText(iapAssemblyPath, jsonOutput);
            return true;
        }
    }
}

/*
In case if you need to use GUID for reference, refer to code below

    ...

    HashSet<string> assemblyReferenceGUIDs = new HashSet<string>();

    foreach (var assembly in allAssemblies) {
        if (requiredReferences.Contains(assembly.name)) {
            var assetGUID = AssetDatabase.AssetPathToGUID(CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assembly.name));
            var assemblyGUID = CompilationPipeline.GUIDToAssemblyDefinitionReferenceGUID(assetGUID);
            assemblyReferenceGUIDs.Add(assemblyGUID);
        }
    }

    ...

    var newGUIDs = new string[assemblyReferenceGUIDs.Count];
    assemblyReferenceGUIDs.CopyTo(asmdefObject.references);

    ...


*/