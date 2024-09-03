using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using PlasticGui.WorkspaceWindow.Sync;
using UnityEditor.Graphs;
using System.Linq;
using System.Text.RegularExpressions;

namespace AddressableExtend
{

    public class AssetFileProcessor : AssetModificationProcessor
    {
        private static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            AssetFolderMenu.RenameAsset(oldPath, newPath);

            return AssetMoveResult.DidNotMove;
        }

        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            AssetFolderMenu.DeleteAsset(path);
            return AssetDeleteResult.DidNotDelete;
        }
    }

}
