using System;
using System.Collections.Generic;
using System.Linq;
using LayerAndSorting;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace UnityEditor.TreeViewExamples
{

	internal class LsMultiColumnWindow : EditorWindow
	{
		static bool isDrity = false;
		public static void SetDrity()
		{
			isDrity = true;
		}

		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_ColumnHeaderState;

		LsMultiColumnHeader lsColumnHeader;
		TreeModel<LsInfo> m_treeModel;
		LsMultiColumnTreeView m_TreeView;

        LsInfoSave infoSave;
		GameObject selectGo;

		internal List<LsInfo> m_Infos;
		internal int selectID = 0;
		internal int changeLayerID = 0;
		internal int sortLayerID = 0;
		internal Vector2 orderRangeID;
	
	 

	

		internal string[] texts
		{ 
			get
			{
				return new string[] { "Layers", "SortingLayer", "OrderID" };
			}
		}

		[MenuItem("TreeView Examples/Multi Columns")]
		public static LsMultiColumnWindow GetWindow()
		{
			var window = GetWindow<LsMultiColumnWindow>();
			window.titleContent = new GUIContent("Multi Columns");
			window.Focus();
			window.Repaint();
			return window;
		}

		GUIStyle richTextStyle;
		int appendHeight = 80;
		int appendY = 20;
		int appendSpace = 10;
		int appendControl = 40;

		Rect multiColumnTreeViewRect
		{
			get { return new Rect(20, appendY, position.width - 40, position.height - appendHeight - appendY- appendControl); }
		}

		Rect multiTreeViewRect
		{
			get { return new Rect(20, NextY(multiColumnTreeViewRect) + appendSpace,  position.width - 40, appendHeight); }
		}

		Rect multiControlViewRect
		{
			get { return new Rect(20, NextY(multiTreeViewRect), position.width - 40, appendControl); }
		}

		public int NextY(Rect rect)
		{
			var value =(int)rect.y + (int)rect.height;
			return value;
		}


		void InitIfNeeded()
		{
			if (!m_Initialized)
			{

				if (m_TreeViewState == null)
					m_TreeViewState = new TreeViewState();

				m_ColumnHeaderState = LsMultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
				lsColumnHeader      = new LsMultiColumnHeader(m_ColumnHeaderState);
				m_treeModel         = new TreeModel<LsInfo>(GetData());
				m_TreeView          = new LsMultiColumnTreeView(m_TreeViewState, lsColumnHeader, m_treeModel);

				lsColumnHeader.Init(this, m_TreeView);
				m_Initialized = true;

				selectGo = Selection . activeGameObject;
				infoSave = Resources . Load<LsInfoSave>("LsSave");
				richTextStyle = new GUIStyle(GUI.skin.label) { fontSize =20,fontStyle = FontStyle.Bold};
			}
		}

		IList<LsInfo> GetData()
		{
			if (m_Infos != null && m_Infos.Count > 0)
				return m_Infos;

			// generate some test data
			m_Infos = new List<LsInfo>()
			{
				LayerAndSortingData.root
			};

			LayerAndSortingData.FindAllTransfrom(Selection.activeTransform);
			m_Infos.AddRange(LayerAndSortingData.lSInfos);

			return m_Infos;
		}

		void OnGUI()
		{
			InitIfNeeded();
			DoTreeView(multiColumnTreeViewRect);
			DoControlView(multiTreeViewRect);
			ControllerView(multiControlViewRect);
		}

		void DoTreeView(Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}


		void DoControlView(Rect rect)
		{
			GUILayout.BeginArea(rect);
			{
				GUILayout.BeginVertical("helpbox");

				selectID = GUILayout.Toolbar(selectID, texts);
				if (selectID == 0)
				{
					CustomView("Layers");
				}
				else if (selectID == 1)
				{
					CustomView("SortingLayer");
				}
				else
				{
					CustomIDView();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}
    
		void CustomView(string key)
		{
			GUILayout.BeginVertical("helpbox");
			{
				GUILayout.Label(key, richTextStyle);
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label($"修改当前 {key}", GUILayout.Width(120));
					GUI.enabled = false;

					if ("Layers" == key)
					{
						EditorGUILayout.MaskField(lsColumnHeader.layerID, lsColumnHeader.filterLayerNames, GUILayout.Width(120));
					}
					else if ("SortingLayer" == key)
					{
						EditorGUILayout.MaskField(lsColumnHeader.sortLayerID, lsColumnHeader.filterSortNames, GUILayout.Width(120));
					}

					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label("<----------------->");
					GUILayout.FlexibleSpace();
					GUILayout.Label($"修改为 {key}", GUILayout.Width(120));

					if ("Layers" == key)
					{
						changeLayerID = EditorGUILayout.Popup(changeLayerID, lsColumnHeader.filterLayerNames, GUILayout.Width(120));
					}
					else if ("SortingLayer" == key)
					{
						sortLayerID = EditorGUILayout.Popup(sortLayerID, lsColumnHeader.filterSortNames, GUILayout.Width(120));
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}

		void CustomIDView()
		{
			GUILayout.BeginVertical("helpbox");
			{
				GUILayout.Label("OrderID", richTextStyle);
			
				EditorGUILayout.BeginHorizontal();
				{
					GUI.enabled = false;

					GUILayout.Label($"修改当前的区间值 min[{lsColumnHeader.rangeID.x}] max[{lsColumnHeader.rangeID.y}]", GUILayout.Width(200));
					EditorGUILayout.MinMaxSlider(ref lsColumnHeader.rangeID.x, ref lsColumnHeader.rangeID.y, -20, 1000);
					GUI.enabled = true;

					GUILayout.FlexibleSpace();
					GUILayout.Label("<----------------->");
					GUILayout.FlexibleSpace();

					GUILayout.Label($"修改为  min[{orderRangeID.x}] max[{orderRangeID.y}]", GUILayout.Width(200));
					EditorGUILayout.MinMaxSlider(ref orderRangeID.x, ref orderRangeID.y, LsMultiColumnHeader.Min, LsMultiColumnHeader.Max);

					orderRangeID.x = Mathf.RoundToInt(orderRangeID.x);
					orderRangeID.y = Mathf.RoundToInt(orderRangeID.y);
				}
			

				if (GUILayout.Button("Apply", GUILayout.Width(100)))
				{
					Debug.Log("数据应用");
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}

		void ControllerView(Rect rect)
		{
			var current = infoSave.FindData(selectGo);
			GUILayout.BeginArea(rect);
			{
				EditorGUILayout.BeginHorizontal("helpbox");
				{
					GUILayout.Label("记录步骤");
					if (GUILayout.Button("<-", GUILayout.Width(60)))
					{
						infoSave.Back(selectGo);
					}
					GUILayout.Label($"{current?.step}");
					if (GUILayout.Button("->", GUILayout.Width(60)))
					{
						var mainInfo = infoSave.changeDatas.Find(_ => _?.mainGO == selectGo);
						if (mainInfo != null)
						{
							infoSave.Next(selectGo);
						}
					}
					GUILayout.Space(20);
					GUILayout.Label("<----------------->");
					GUILayout.Space(20);

					GUI.enabled = isDrity;
					if (GUILayout.Button("Save", GUILayout.Width(100)))
					{
						isDrity = false;
						infoSave.Save(selectGo, lsColumnHeader.findInfos);
					}
					GUI.enabled = true;

					if (GUILayout.Button("Apply", GUILayout.Width(100)))
					{
						Debug.Log("数据应用");
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
		}

		void OnDisable()
		{ 
			if (infoSave != null)
			{
				EditorUtility.SetDirty(infoSave);
				AssetDatabase.SaveAssets();
			}
		}
	}
}
