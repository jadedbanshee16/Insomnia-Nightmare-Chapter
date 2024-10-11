// Create an AssetBundle for Windows.
using UnityEngine;
using UnityEditor;

public class BuildAssetBundlesExample : MonoBehaviour
{
    [UnityEditor.MenuItem("Example/Build Asset Bundles")]
    static void BuildABs()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
