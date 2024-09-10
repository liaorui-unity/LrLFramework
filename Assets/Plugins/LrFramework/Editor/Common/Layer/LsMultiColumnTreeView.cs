// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor.IMGUI.Controls;
// using UnityEngine;
// using UnityEditor;
// using System;
// public class LsMultiColumnTreeView : TreeViewWithTreeModel<MyTreeElement>
// {
//     const float kRowHeights = 20f;
//     const float kToggleWidth = 18f;
//     public bool showControls = true;

//     public LsMultiColumnTreeView(TreeViewState state,
//                               MultiColumnHeader multicolumnHeader,
//                               TreeModel<MyTreeElement> model)
//                               : base(state, multicolumnHeader, model)
//     {
//         // 自定义设置
//         rowHeight = 20;
//         columnIndexForTreeFoldouts = 2;
//         showAlternatingRowBackgrounds = true;
//         showBorder = true;
//         customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f;
//         extraSpaceBeforeIconAndLabel = kToggleWidth;
//         multicolumnHeader.sortingChanged += OnSortingChanged;

//         Reload();
//     }

//     public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
//     {
//         if (root == null)
//             throw new NullReferenceException("root");
//         if (result == null)
//             throw new NullReferenceException("result");

//         result.Clear();

//         if (root.children == null)
//             return;

//         Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
//         for (int i = root.children.Count - 1; i >= 0; i--)
//             stack.Push(root.children[i]);

//         while (stack.Count > 0)
//         {
//             TreeViewItem current = stack.Pop();
//             result.Add(current);

//             if (current.hasChildren && current.children[0] != null)
//             {
//                 for (int i = current.children.Count - 1; i >= 0; i--)
//                 {
//                     stack.Push(current.children[i]);
//                 }
//             }
//         }
//     }


//     protected override IList<TreeViewItem> BuildRoot()
//     {
//         var rows = base.BuildRows(root);
//         SortIfNeeded(root, rows);
//         return rows;
//     }
// }
