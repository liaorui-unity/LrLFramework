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
        internal static void SetDrity()
		{
			isDrity = true;
        }



        static LsInfoSave infoSave;
        internal static void Quit()
        {
            infoSave?.Clear();
			SaveInfo();
        }


        static void SaveInfo()
        {
            if (infoSave != null)
            {
                EditorUtility.SetDirty(infoSave);
                AssetDatabase.SaveAssets();
            }
        }


        [NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_ColumnHeaderState;

		LsMultiColumnHeader lsColumnHeader;
		TreeModel<LsInfo> m_treeModel;
		LsMultiColumnTreeView m_TreeView;


		GameObject selectGo;

		internal List<LsInfo> m_Infos;
		internal int selectID = 0;
		internal int changeLayerID = 0;
		internal int sortLayerID = 0;
		internal Vector2 orderRangeID;

		internal enum ControlType
		{
			Layers, SortingLayer, OrderID
		}

		internal ControlType control;

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
		int appendHeight = 85;
		int appendY      = 10;
		int appendSpace  = 10;
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
			get { return new Rect(20, NextY(multiTreeViewRect)+1, position.width - 40, appendControl); }
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


				var find = infoSave.FindData(selectGo);
				if (find == null)
				{
					infoSave.Save(selectGo, m_Infos);
				}

                for (int i = infoSave.changeDatas.Count - 1; i >= 0; i--)
				{
                    if (DateTime.TryParse(infoSave.changeDatas[i].savetime, out var save))
                    {
                        TimeSpan difference = DateTime.Now - save;

                        if (difference.TotalHours > 24)
                        {
                            // 两个时间之间的差异大于24小时去掉
                            infoSave.changeDatas.RemoveAt(i);
                        }
                    }
                }
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
					control = ControlType.Layers;

                    CustomView();
				}
				else if (selectID == 1)
				{
                    control = ControlType.SortingLayer;
                    CustomView();
				}
				else
                {
                    control = ControlType.OrderID;
                    CustomIDView();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}
    
		void CustomView()
		{
			GUILayout.BeginHorizontal("helpbox");
			{
				GUILayout.BeginVertical("helpbox");
				{
					GUILayout.Label(control.ToString(), richTextStyle);
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label($"修改当前 {control}", GUILayout.Width(120));
						GUI.enabled = false;

						if (control == ControlType.Layers)
						{
							EditorGUILayout.MaskField(lsColumnHeader.layerID, lsColumnHeader.filterLayerNames, GUILayout.Width(120));
						}
						else if (control == ControlType.SortingLayer)
						{
							EditorGUILayout.MaskField(lsColumnHeader.sortLayerID, lsColumnHeader.filterSortNames, GUILayout.Width(120));
						}

						GUI.enabled = true;
						GUILayout.FlexibleSpace();
						GUILayout.Label("<----------------->");
						GUILayout.FlexibleSpace();
						GUILayout.Label($"修改为 {control}", GUILayout.Width(120));

						if (ControlType.Layers == control)
						{
							changeLayerID = EditorGUILayout.Popup(changeLayerID, lsColumnHeader.filterLayerNames, GUILayout.Width(120));
						}
						else if (ControlType.SortingLayer == control)
						{
							sortLayerID = EditorGUILayout.Popup(sortLayerID, lsColumnHeader.filterSortNames, GUILayout.Width(120));
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("helpbox");
				{
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("数据应用", GUILayout.Width(100));
                    if (GUILayout.Button("Apply", GUILayout.Width(100), GUILayout.Height(24)))
					{
                        ApplyData(control);
					}
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		void CustomIDView()
		{
			GUILayout.BeginHorizontal("helpbox");
			{
				GUILayout.BeginVertical("helpbox");
				{
					GUILayout.Label("OrderID", richTextStyle);

					EditorGUILayout.BeginHorizontal();
					{
						GUI.enabled = false;

						GUILayout.Label($"区间值 [{lsColumnHeader.rangeID.x},{lsColumnHeader.rangeID.y}]");
						EditorGUILayout.MinMaxSlider(ref lsColumnHeader.rangeID.x, ref lsColumnHeader.rangeID.y, LsMultiColumnHeader.Min, LsMultiColumnHeader.Max);
						GUI.enabled = true;

						GUILayout.FlexibleSpace();
						GUILayout.Label("<----------------->");
						GUILayout.FlexibleSpace();

						GUILayout.Label($"修改为  min");
						orderRangeID.x = EditorGUILayout.IntField ((int)orderRangeID.x, GUILayout.Width(40));
						GUILayout.Label($"max");
						orderRangeID.y = EditorGUILayout.IntField((int)orderRangeID.y, GUILayout.Width(40));
						EditorGUILayout.MinMaxSlider(ref orderRangeID.x, ref orderRangeID.y, LsMultiColumnHeader.Min, LsMultiColumnHeader.Max);

						orderRangeID.x = Mathf.RoundToInt(orderRangeID.x);
						orderRangeID.y = Mathf.RoundToInt(orderRangeID.y);
					}
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical("helpbox");
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label("数据应用", GUILayout.Width(100));
					if (GUILayout.Button("Apply", GUILayout.Width(100),GUILayout.Height(24)))
					{
						ApplyData(ControlType.OrderID);
						lsColumnHeader.rangeID = orderRangeID;
                    }
					GUILayout.FlexibleSpace();
					GUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
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
					GUILayout.FlexibleSpace();
					GUILayout.Label("<----------------->");
					GUILayout.FlexibleSpace();

					GUI.enabled = isDrity;
					if (GUILayout.Button("Save", GUILayout.Width(100)))
					{
						isDrity = false;
						infoSave.Save(selectGo, lsColumnHeader.findInfos);
					}
					GUI.enabled = true;

					GUILayout.Space(10);
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
		}


		void ApplyData(ControlType type)
		{
            switch (type)
            {
				case ControlType.Layers:

					foreach (var item in lsColumnHeader.findInfos)
					{
						if (item.info == null)
							continue;

						item.Layer = changeLayerID;
					}
					break;
                case ControlType.SortingLayer:
                    foreach (var item in lsColumnHeader.findInfos)
                    {
                        if (item.info == null)
                            continue;

                        item.SortLayerName   = lsColumnHeader.filterSortNames[sortLayerID];
						item.SortLayerSortID = sortLayerID;
                    }
                    break;
                case ControlType.OrderID:
                    foreach (var item in lsColumnHeader.findInfos)
                    {
                        if (item.info == null)
                            continue;

						var newRadio = (item.OrderInLayer - lsColumnHeader.rangeID.x) / (orderRangeID.y - orderRangeID.x);
						item.OrderInLayer = Mathf.RoundToInt(Mathf.Lerp(orderRangeID.x, orderRangeID.y, newRadio));
                    }
                    break;
            }

			LsMultiColumnWindow.SetDrity();
            lsColumnHeader.isDrity = true;
        }


	

		void OnDisable()
		{
			SaveInfo();
            EditorUtility.SetDirty(selectGo);
			AssetDatabase.SaveAssetIfDirty(selectGo);
        }
	}
}
