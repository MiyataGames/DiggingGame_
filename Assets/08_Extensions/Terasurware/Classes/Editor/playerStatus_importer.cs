using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class playerStatus_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/playerStatus.xlsx";
	private static readonly string exportPath = "Assets/Resources/playerStatus.asset";
	private static readonly string[] sheetNames = { "Sheet1","Sheet2", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			PlayerStatusBase data = (PlayerStatusBase)AssetDatabase.LoadAssetAtPath (exportPath, typeof(PlayerStatusBase));
			if (data == null) {
				data = ScriptableObject.CreateInstance<PlayerStatusBase> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					PlayerStatusBase.Sheet s = new PlayerStatusBase.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						PlayerStatusBase.Param p = new PlayerStatusBase.Param ();
						
					cell = row.GetCell(0); p.level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.sp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.atk = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.def = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.agi = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
