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
	internal class LsMultiColumnTreeView : TreeViewWithTreeModel<LsInfo>
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

		public LsMultiColumnTreeView (TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<LsInfo> model) : base (state, multicolumnHeader, model)
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




	


		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<LsInfo>) args.item;

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
					var lastLayer = item.data.Layer;
					item.data.Layer = EditorGUI.LayerField(cellRect, item.data.Layer);
					if (lastLayer != item.data.Layer)
					{ 
						LsMultiColumnWindow.SetDrity();
					}

					 break;
				case MyColumns.SortingLayer:
					item.data.SortLayerSortID = sortingNames.FindIndex(x => x == item.data.SortLayerName);

					var lastSort = item.data.SortLayerSortID;
					item.data.SortLayerSortID = EditorGUI.Popup(cellRect, item.data.SortLayerSortID, popSortingNames);
				
					if (lastSort != item.data.SortLayerSortID)
					{
						LsMultiColumnWindow.SetDrity();
					}

					item.data.SortLayerName = sortingNames[item.data.SortLayerSortID];

					break;
				case MyColumns.OrderID:
					{
						var lastOrder = item.data.OrderInLayer;
						item.data.OrderInLayer = EditorGUI.IntField(cellRect, lastOrder);
						if (lastOrder != item.data.OrderInLayer)
						{
							LsMultiColumnWindow.SetDrity();
						}
					}
					break;
			}
		}





        protected override void SelectionChanged(IList<int> selectedIds)
		{
			base.SelectionChanged(selectedIds);
			if (selectedIds.Count > 0)
			{
				int selectedId = selectedIds[0];
				TreeViewItem<LsInfo> selectedItem = (TreeViewItem<LsInfo>)FindItem(selectedId, rootItem);
				Selection.activeObject = selectedItem.data.mainGo;
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
					minWidth = 100,
					autoResize = false,
					userData = (int)MyColumns.Name
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Layer", "In sed porta ante. Nunc et nulla mi."),
					headerTextAlignment = TextAlignment.Left,
			      	width = 200,
					minWidth = 150,
					autoResize = true,
					userData = (int)MyColumns.Layer
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("SortingLayer", "Maecenas congue non tortor eget vulputate."),
					headerTextAlignment = TextAlignment.Left,
					width = 200,
					minWidth = 150,
					autoResize = true,
					userData = (int)MyColumns.SortingLayer

				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("OrderLayer", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus."),
					headerTextAlignment = TextAlignment.Left,
					width = 250,
					minWidth = 250,
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
		public const int Max = 1000;
		public const int Min = -20;

		internal string[] filterSortNames;
		internal string[] filterLayerNames;
		internal string   fifterName { get; set; }


		internal int sortLayerID = -1;
		internal int layerID = -1;
		internal Vector2 rangeID;

		internal List<LsInfo> findInfos;

		Rect GetShowArea(Rect headerRect)
		{
			return new Rect(headerRect.x + headerRect.width / 2 - 5, headerRect.y / 2 + (headerRect.height - 16) / 2, headerRect.width / 2, 16);
		}

		private LsMultiColumnTreeView m_view;
		private LsMultiColumnWindow m_window;
		public LsMultiColumnHeader(MultiColumnHeaderState state) : base(state)
        {
			canSort = false;
			filterSortNames = GetSortingLayerNames();
			height = DefaultGUI.defaultHeight + 16;
		}


		public void Init(LsMultiColumnWindow window, LsMultiColumnTreeView treeView)
		{
			m_window = window;
			m_view   = treeView;
			m_view   . SetSortLayer(filterSortNames);

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
			findInfos = LayerAndSortingData.lSInfos;
		}
		

		protected override void ColumnHeaderGUI (MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
            var columnType = (LsMultiColumnTreeView.MyColumns)column.userData;

            switch (columnType)
			{
				
				case LsMultiColumnTreeView.MyColumns.Name:
					// 如果需要在列头显示弹出菜单
					// 显示列头文本
					GUI.Label(headerRect, column.headerContent);
					Rect popupRect = GetShowArea(headerRect);
                    fifterName =  EditorGUI.TextField(popupRect, fifterName);
					break;
				case LsMultiColumnTreeView.MyColumns.Layer:
					GUI.Label(headerRect, column.headerContent);
					Rect layerRect = GetShowArea(headerRect);
					layerID = EditorGUI.MaskField(layerRect, layerID, filterLayerNames);
					break;
				case LsMultiColumnTreeView.MyColumns.SortingLayer:
					GUI.Label(headerRect, column.headerContent);
					Rect lsortRect = GetShowArea(headerRect);
					sortLayerID = EditorGUI.MaskField(lsortRect,sortLayerID, filterSortNames);
					break;
				case LsMultiColumnTreeView.MyColumns.OrderID:
                    GUILayout.BeginArea(headerRect);
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.BeginVertical();
							{
								GUILayout.FlexibleSpace();
								GUILayout.Label(column.headerContent);
								GUILayout.FlexibleSpace();
							}
							GUILayout.EndVertical();

							GUILayout.BeginVertical();
							{
								EditorGUILayout.MinMaxSlider(ref rangeID.x, ref rangeID.y, Min, Max);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Label("Min", GUILayout.Width(30));
									rangeID.x = EditorGUILayout.FloatField(rangeID.x, GUILayout.Width(50));

									GUILayout.Label("Max", GUILayout.Width(30));
									rangeID.y = EditorGUILayout.FloatField(rangeID.y, GUILayout.Width(50));
									//向上取整
									rangeID.x = Mathf.RoundToInt(rangeID.x);
									rangeID.y = Mathf.RoundToInt(rangeID.y);
								}
								GUILayout.EndHorizontal();
							}
							GUILayout.EndVertical();
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
					break;
			}

			if (GUI.changed)
			{
				OnFilterSelected();
			}
		}


		List<LsInfo> GetFifterLists()
		{
			List<LsInfo> infos = new List<LsInfo>(LayerAndSortingData.lSInfos);
			if (string.IsNullOrEmpty(fifterName) == false)
			{
				infos = infos.Where(_ => _.name.Contains(fifterName)).ToList();
				Debug.Log(infos.Count);
			}

			infos = infos.Where(_ => layerID == -1     || (layerID & (1 << _.Layer)) != 0).ToList();
			infos = infos.Where(_ => sortLayerID == -1 || (sortLayerID & (1 << _.SortLayerSortID)) != 0).ToList();
			infos = infos.Where(_ => rangeID.x == -20  || rangeID.y == 1000 || _.OrderInLayer >= rangeID.x && _.OrderInLayer <= rangeID.y).ToList();

			infos.Insert(0, m_view.treeModel.root);

		    return infos;
		}

		private void OnFilterSelected()
		{
			findInfos = GetFifterLists();
			m_view.treeModel.SetData(findInfos);
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
