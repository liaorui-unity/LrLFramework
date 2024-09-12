using System;
using System.Collections.Generic;
using LayerAndSorting;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace UnityEditor.TreeViewExamples
{

	internal class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;

		LsMultiColumnHeader lsMultiColumnHeader;
		TreeModel<LsInfo> treeModel;
		MultiColumnTreeView m_TreeView;
		internal List<LsInfo> m_Infos;
  

		[MenuItem("TreeView Examples/Multi Columns")]
		public static MultiColumnWindow GetWindow ()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Multi Columns");
			window.Focus();
			window.Repaint();
			return window;
		}


		public void SetTreeAsset ()
		{
			m_Initialized = false;
		}

		Rect multiColumnTreeViewRect
		{
			get { return new Rect(20, 46, position.width-40, position.height-40); }
		}


		void InitIfNeeded ()
		{
			if (!m_Initialized)
			{

				if (m_TreeViewState == null)
					m_TreeViewState = new TreeViewState();

				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
		
				m_MultiColumnHeaderState = headerState;
				lsMultiColumnHeader = new LsMultiColumnHeader(headerState);
	
				treeModel  = new TreeModel<LsInfo>(GetData());
				m_TreeView = new MultiColumnTreeView(m_TreeViewState, lsMultiColumnHeader, treeModel);

				lsMultiColumnHeader.Init(this,m_TreeView);

				m_Initialized = true;
			}
		}
		
		IList<LsInfo> GetData ()
		{
			if ( m_Infos != null && m_Infos.Count > 0)
				return m_Infos;

			var root = new LsInfo(SortType.Render, null, -1)
			{
				name = "Root",
				depth = -1,
				id = -1
			};

			// generate some test data
			m_Infos = new List<LsInfo>()
			{
				root
			};

			LayerAndSortingData.FindAllTransfrom(Selection.activeTransform);

			m_Infos.AddRange(LayerAndSortingData.lSInfos);
			return m_Infos;
		}




		void OnGUI ()
		{
			InitIfNeeded();
			DoTreeView (multiColumnTreeViewRect);	
		}

		void DoTreeView (Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}

	}

}
