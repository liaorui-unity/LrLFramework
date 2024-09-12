using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LayerAndSorting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.TreeViewExamples
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<LsInfo>
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		// All columns
		internal enum MyColumns
		{
			Name,
			Layer,
			SortingLayer,
			OrderID,
		}


		public void SetSortLayer(string[] strings)
		{
			sortingNames    = strings.ToList();
			popSortingNames = strings;
		}

		List<string> sortingNames;
		string[] popSortingNames;

		public MultiColumnTreeView (TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<LsInfo> model) : base (state, multicolumnHeader, model)
		{
			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 2;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
          
			Reload();
		}


	

		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows (root);
			return rows;
		}

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

			Debug.Log("id");
        }




		
        


		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<LsInfo>) args.item;

			// 检查选择状态
			if (args.selected)
			{
				Debug.Log("Currently selected: " + item.data.name);
			}

			for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI (Rect cellRect, TreeViewItem<LsInfo> item, MyColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case MyColumns.Name:
					{
						EditorGUI.LabelField(cellRect,item.data.mainGo.name);
					}
					break;
				case MyColumns.Layer:
					 item.data.Layer = EditorGUI.LayerField(cellRect, item.data.Layer);
					 break;
				case MyColumns.SortingLayer:

					item.data.SortLayerSortID = sortingNames.FindIndex(x => x == item.data.SortLayerName);
					item.data.SortLayerSortID = EditorGUI.Popup(cellRect, item.data.SortLayerSortID, popSortingNames);
					item.data.SortLayerName   = sortingNames[item.data.SortLayerSortID];
					break;
				case MyColumns.OrderID:
					{
						item.data.OrderInLayer = EditorGUI.IntField(cellRect, item.data.OrderInLayer);
					}
					break;
			}
		}





        protected override void SelectionChanged(IList<int> selectedIds)
		{
			base.SelectionChanged(selectedIds);

			Debug.Log("Selected Item: " + selectedIds[0]);

			if (selectedIds.Count > 0)
			{
				int selectedId = selectedIds[0];
				TreeViewItem<LsInfo> selectedItem = (TreeViewItem<LsInfo>)FindItem(selectedId, rootItem);
				Debug.Log("Selected Item: " + selectedItem.data.name);
				// Add additional logic here for when an item is selected
			}
		}
	

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
		{
			var columns = new[] 
			{
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Left,
					width = 150, 
					minWidth = 60,
					autoResize = false,
					userData = (int)MyColumns.Name
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Layer", "In sed porta ante. Nunc et nulla mi."),
					headerTextAlignment = TextAlignment.Left,
					width = 110,
					minWidth = 60,
					autoResize = true,
					userData = (int)MyColumns.Layer
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("SortingLayer", "Maecenas congue non tortor eget vulputate."),
					headerTextAlignment = TextAlignment.Left,
					width = 95,
					minWidth = 60,
					autoResize = true,
					userData = (int)MyColumns.SortingLayer

				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("OrderLayer", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus."),
					headerTextAlignment = TextAlignment.Left,
					width = 70,
					minWidth = 60,
					autoResize = true,
					userData = (int)MyColumns.OrderID
				}
			};

			var state =  new MultiColumnHeaderState(columns);
			return state;
		}
	}


    internal class LsMultiColumnHeader : MultiColumnHeader
    {
		private string[] filterSortNames;
		private string[] filterLayerNames;

		string _fifterName;
		private string fifterName { get; set; }


		private int sortLayerID = -1;
		private int layerID = -1;
		private Vector2 rangeID;

		Rect GetShowArea(Rect headerRect)
		{
			return new Rect(headerRect.x + headerRect.width / 2 - 2, headerRect.y / 2 + (headerRect.height - 16) / 2, headerRect.width / 2, 16);
		}

		private MultiColumnTreeView m_view;
		private MultiColumnWindow m_window;
		public LsMultiColumnHeader(MultiColumnHeaderState state) : base(state)
        {
			canSort = false;
			filterSortNames = GetSortingLayerNames();
		}


		public void Init(MultiColumnWindow window, MultiColumnTreeView treeView)
		{
			m_window = window;

			m_view = treeView;
			m_view.SetSortLayer(filterSortNames);

			// 获取所有层的名称
			List<string> layerNames = new List<string>();
			for (int i = 0; i < 32; i++)
			{
				var name= LayerMask.LayerToName(i);
				if (string.IsNullOrEmpty(name) == false)
				{
					layerNames.Add(name);
				}
			}
			filterLayerNames = layerNames.ToArray();
		}
		

		protected override void ColumnHeaderGUI (MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
            var columnType = (MultiColumnTreeView.MyColumns)column.userData;
			switch (columnType)
			{
				
				case MultiColumnTreeView.MyColumns.Name:

					// 显示列头文本
					GUI.Label(headerRect, column.headerContent);

					// 如果需要在列头显示弹出菜单
					Rect popupRect = GetShowArea(headerRect);
					_fifterName =  EditorGUI.TextField(popupRect, _fifterName);
					break;
				case MultiColumnTreeView.MyColumns.Layer:
					Rect layerRect = GetShowArea(headerRect);
					layerID = EditorGUI.MaskField(layerRect, layerID, filterLayerNames);
					break;
				case MultiColumnTreeView.MyColumns.SortingLayer:
					Rect lsortRect = GetShowArea(headerRect);
					sortLayerID = EditorGUI.MaskField(lsortRect,sortLayerID, filterSortNames);
					break;
				case MultiColumnTreeView.MyColumns.OrderID:
					Rect minMax = GetShowArea(headerRect);
					EditorGUI.MinMaxSlider(minMax,ref rangeID.x, ref rangeID.y, -20, 1000);
					break;
			}

			if (GUI.changed)
			{
				Debug.Log("changed");
				OnFilterSelected();
			}
		}


		List<LsInfo> GetFifterLists()
		{
			List<LsInfo> infos = new List<LsInfo>(LayerAndSortingData.lSInfos);
			if (string.IsNullOrEmpty(fifterName) == false)
				infos = infos.Where(_ => _.name.Contains(fifterName)).ToList();

			infos = infos.Where(_ => layerID == -1     || (layerID & (1 << _.Layer)) != 0).ToList();
			infos = infos.Where(_ => sortLayerID == -1 || (sortLayerID & (1 << _.SortLayerSortID)) != 0).ToList();
			infos = infos.Where(_ => rangeID.x == -20  || rangeID.y == 1000 || _.OrderInLayer >= rangeID.x && _.OrderInLayer <= rangeID.y).ToList();

			infos.Insert(0, m_view.treeModel.root);

		    return infos;
		}

		private void OnFilterSelected()
		{
			m_view.treeModel.SetData(GetFifterLists());
			m_view.Reload();
		}

		public string[] GetSortingLayerNames()
		{
			// 使用反射获取SortingLayerUtility类
			System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
			var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
	}
}
