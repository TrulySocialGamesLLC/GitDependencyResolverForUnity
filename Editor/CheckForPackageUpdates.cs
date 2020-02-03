using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public class CheckForPackageUpdates
{
    static string kProjectOpened = "ProjectOpened";

    static CheckForPackageUpdates()
    {
        if (SessionState.GetBool(kProjectOpened, false) == true) return;

        CheckForUpdates();
        Debug.Log("Checked for updates");
    }

    [MenuItem("Package Manager Utils/Check for Package Updates")]
    public static void CheckForUpdates()
    {
        var packageManifestPath = $"{Application.dataPath}/../Packages/manifest.json";

        JObject manifest;

        using (StreamReader reader = File.OpenText(packageManifestPath))
        {
            manifest = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

            manifest.Remove("lock");
        }

        using (StreamWriter writer = new StreamWriter(packageManifestPath))
        {
            writer.Write(manifest.ToString());
        }

        Coffee.PackageManager.DependencyResolver.GitDependencyResolver.StartResolve();

        SessionState.SetBool(kProjectOpened, true);
    }
}
