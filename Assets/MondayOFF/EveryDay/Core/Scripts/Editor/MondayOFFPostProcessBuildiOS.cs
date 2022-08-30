#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace MondayOFF {
    public static class MondayOFFPostProcessBuild {
        [PostProcessBuild(int.MaxValue)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
            // BuiltTarget.iOS is not defined in Unity 4, so we just use strings here
            if (buildTarget != BuildTarget.iOS) return;

            PrepareProject(buildPath);
            PrepareInfoPlist(buildPath);
            PrepareBuildPhases(buildPath);

            // Hope this gets fixed on later versions of XCode
            PrepareSwiftLibrarySearchPath(buildPath);
            CreateEmptySwiftFile(buildPath);
        }

        private static void PrepareProject(string buildPath) {
            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));
            var target = project.GetUnityMainTargetGuid();

            project.AddBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            // For Firebase SDK?
            project.AddBuildProperty(target, "ENABLE_BITCODE", "NO");

            project.AddCapability(target, PBXCapabilityType.InAppPurchase);

            // add framework
            project.AddFrameworkToProject(target, "Security.framework", false);
            project.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
            project.AddFrameworkToProject(target, "iAD.framework", false);
            project.AddFrameworkToProject(target, "Adsupport.framework", false);
            project.AddFrameworkToProject(target, "AdServices.framework", false);
            project.AddFrameworkToProject(target, "WebKit.framework", false);
            project.AddFrameworkToProject(target, "StoreKit.framework", false);
            project.AddFrameworkToProject(target, "libsqlite3.0.tbd", false);
            project.AddFrameworkToProject(target, "libz.tbd", false);

            File.WriteAllText(projPath, project.WriteToString());
        }

        private static void PrepareInfoPlist(string buildPath) {
            // Get plist
            string plistPath = Path.Combine(buildPath, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Max.. should go to Ads but this should not hurt i guess
            var appLovinKey = "AppLovinSdkKey";
            rootDict.SetString(appLovinKey, "-uBAP4IJbzlOMFq-KJUwdvW8bwGdhtGgmRr9V8T65CUSSIQocwhFqCNP7e2pVITFkJPERuLW5q-X7PJlJ_-7CM");
            var NSLocationWhenInUseUsageDescription = "NSLocationWhenInUseUsageDescription";
            rootDict.SetString(NSLocationWhenInUseUsageDescription, "Permission to improve advertisement experience");

            // SKAN redirection
            var NSAdvertisingAttributionReportEndpoint = "NSAdvertisingAttributionReportEndpoint";
            rootDict.SetString(NSAdvertisingAttributionReportEndpoint, "https://singular-bi.net");


            // Remove Unity Generated armv7
            const string capsKey = "UIRequiredDeviceCapabilities";
            PlistElementArray capsArray;
            PlistElement pel;
            if (rootDict.values.TryGetValue(capsKey, out pel)) capsArray = pel.AsArray();
            else capsArray = rootDict.CreateArray(capsKey);
            // Remove any existing "armv7" plist entries
            const string armv7Str = "armv7";
            capsArray.values.RemoveAll(x => armv7Str.Equals(x.AsString()));
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void PrepareBuildPhases(string buildPath) {
            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));
            var target = project.GetUnityFrameworkTargetGuid();
            var buildPhases = project.GetAllBuildPhasesForTarget(target);

            const string PBXSourcesBuildPhase = "PBXSourcesBuildPhase";
            const string PBXHeadersBuildPhase = "PBXHeadersBuildPhase";

            var names = new System.Collections.Generic.List<string>();
            string headerGUID = null, sourcesGUID = null;
            for (int i = 0; i < buildPhases.Length; ++i) {
                var name = project.GetBuildPhaseName(buildPhases[i]);
                if (name == null) { continue; }
                if (name.Equals(PBXSourcesBuildPhase)) {
                    sourcesGUID = buildPhases[i];
                } else if (name.Equals(PBXHeadersBuildPhase)) {
                    headerGUID = buildPhases[i];
                }
            }


            // ! Replaces build phase order brutally. Need to watch for sideeffects!
            var projectContent = project.WriteToString();

            projectContent = projectContent.Replace($"\n\t\t\t\t{headerGUID} /* Headers */,", "");
            projectContent = projectContent.Replace($"\n\t\t\t\t{sourcesGUID} /* Sources */,", $"\n\t\t\t\t{headerGUID} /* Headers */,\n\t\t\t\t{sourcesGUID} /* Sources */,");

            File.WriteAllText(projPath, projectContent);
        }

        private static void PrepareSwiftLibrarySearchPath(string buildPath) {
            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));
            var target = project.GetUnityFrameworkTargetGuid();

            var librarySearchPaths = project.GetBuildPropertyForAnyConfig(target, "LIBRARY_SEARCH_PATHS");

            string toolchain_swift = "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)";
            string toolchain_swift5 = "$(TOOLCHAIN_DIR)/usr/lib/swift-5.0/$(PLATFORM_NAME)";
            string sdkroot_swift = "$(SDKROOT)/usr/lib/swift";

            if (!librarySearchPaths.Contains(toolchain_swift)) {
                project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", toolchain_swift);
            }
            if (!librarySearchPaths.Contains(toolchain_swift5)) {
                project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", toolchain_swift5);
            }
            if (!librarySearchPaths.Contains(sdkroot_swift)) {
                project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", sdkroot_swift);
            }

            File.WriteAllText(projPath, project.WriteToString());
        }

        private static void CreateEmptySwiftFile(string buildPath) {
            string fileName = "File.swift";
            var filePath = Path.Combine(buildPath, fileName);

            if (File.Exists(filePath)) {
                return;
            }

            var fileContent =
@"//
//  File.swift
//  Unity-iPhone
//
//  This is a temporal solution!
//

import Foundation";
            File.WriteAllText(filePath, fileContent);

            var projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projPath));

            var target = project.GetUnityMainTargetGuid();
            var file = project.AddFile(fileName, fileName);
            project.AddFileToBuild(target, file);

            File.WriteAllText(projPath, project.WriteToString());
        }
    }
}
#endif