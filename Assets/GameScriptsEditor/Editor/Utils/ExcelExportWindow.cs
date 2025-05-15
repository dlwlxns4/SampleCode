/// 2025-05-15

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using Newtonsoft.Json;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.CSV;

public class ExcelClassExporter : EditorWindow
{
    private const string _excelFolderPath = "Assets/Table"; 
    private string _outputFolderPath = "Assets/GameScripts/Table"; 
    private string _outputDataPath = "Assets/GameDatas/Table"; 

    private List<string> _excelFiles = new();
    private Dictionary<string, bool> _selectionMap = new();
    private Vector2 _scroll;

    [MenuItem("Tools/Excel to C# Class Exporter")]
    public static void ShowWindow()
    {
        var window = GetWindow<ExcelClassExporter>();
        window.titleContent = new GUIContent("Excel Class Exporter");
        window.ScanExcelFiles();
    }

    private void ScanExcelFiles()
    {
        _excelFiles.Clear();
        _selectionMap.Clear();

        if (!Directory.Exists(_excelFolderPath))
        {
            Debug.LogWarning($"No Excel Folders : {_excelFolderPath}");
            return;
        }

        var files = Directory.GetFiles(_excelFolderPath, "*.xlsx", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            _excelFiles.Add(name);
            _selectionMap[name] = false;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select Excel Files:", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh"))
            ScanExcelFiles();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        foreach (var file in _excelFiles)
            _selectionMap[file] = EditorGUILayout.ToggleLeft(file, _selectionMap[file]);
        EditorGUILayout.EndScrollView();

        _outputFolderPath = EditorGUILayout.TextField("OutputPath", _outputFolderPath);

        GUILayout.Label("메세지 테이블은 CSV생성 후 Localization Table에 업로드 됩니다.");
        
        if (GUILayout.Button("Export Json and Class"))
        {
            foreach (var kvp in _selectionMap)
            {
                if (kvp.Value)
                    ExportClassAndJson(kvp.Key);
            }
            AssetDatabase.Refresh();
        }
    }

    private void ExportClassAndJson(string tableName)
    {
        string path = Path.Combine(_excelFolderPath, tableName + ".xlsx");
        if (!File.Exists(path))
        {
            Debug.LogError($"File Path is wrong : {path}");
            return;
        }

        using (var package = new ExcelPackage(new FileInfo(path)))
        {
            foreach (var sheet in package.Workbook.Worksheets)
            {
                bool shouldExportCsv = sheet.Name.Contains("Message");

                var fields = new List<(string name, string type)>();
                int colCount = sheet.Dimension.End.Column;
                int rowCount = sheet.Dimension.End.Row;

                for (int col = 1; col <= colCount; col++)
                {
                    string name = sheet.Cells[1, col].Text;
                    string type = sheet.Cells[2, col].Text;
                    fields.Add((name, type));
                }

                var className = sheet.Name + "Data";
                var sb = new StringBuilder();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using Newtonsoft.Json;");
                sb.AppendLine();
                sb.AppendLine($"public class {className}");
                sb.AppendLine("{");

                foreach (var (name, type) in fields)
                {
                    string fieldName = char.ToUpper(name[0]) + name.Substring(1);

                    if (shouldExportCsv && fieldName.Equals("Keys") == false)
                        break;
                        
                        
                    sb.AppendLine($"    public {type} {fieldName} {{ get; set; }}");
                    sb.AppendLine();
                }

                var containerType = sheet.Name.Contains("(Dic)") ? "Dic" : "List";

                sb.AppendLine("}");
                sb.AppendLine();
                sb.AppendLine($"public class Table{sheet.Name}Container : Table{containerType}Container<{className}>");
                sb.AppendLine("{");
                sb.AppendLine("    public override void Deserialize(string json)");
                sb.AppendLine("    {");
                sb.AppendLine($"        var result = JsonConvert.DeserializeObject<{containerType}<{className}>>(json);");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                if (!Directory.Exists(_outputFolderPath))
                    Directory.CreateDirectory(_outputFolderPath);
                string filePath = Path.Combine(_outputFolderPath, $"Table{sheet.Name}Container.cs");
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Debug.Log($"{sheet.Name}Data.cs Complete : {filePath}");

                var clientRows = new List<Dictionary<string, object>>();

                var csvSb = new StringBuilder();

                if (shouldExportCsv)
                {
                    var csvHeader = new List<string>();
                    foreach (var (name, type) in fields)
                    {
                        csvHeader.Add(name);
                    }
                    csvSb.AppendLine(string.Join(",", csvHeader));
                }

                for (int row = 3; row <= rowCount; row++)
                {
                    var firstCell = sheet.Cells[row, 1].Text;
                    if (string.IsNullOrWhiteSpace(firstCell)) 
                        continue;

                    var clientRow = new Dictionary<string, object>();
                    var csvRow = new List<string>();

                    for (int col = 1; col <= colCount; col++)
                    {
                        var (name, type)= fields[col - 1];
                        var cellValue = sheet.Cells[row, col].Text;

                        object parsedValue = type switch
                        {
                            "int" => int.TryParse(cellValue, out var iVal) ? iVal : 0,
                            "float" => float.TryParse(cellValue, out var fVal) ? fVal : 0f,
                            "string" => cellValue,
                            _ => cellValue
                        };

                        csvRow.Add(cellValue.Replace(",", "\\,"));

                        clientRow[name] = parsedValue;
                    }

                    if (clientRow.Count > 0)
                        clientRows.Add(clientRow);

                    if (shouldExportCsv && csvRow.Count > 0)
                        csvSb.AppendLine(string.Join(",", csvRow));
                }

                var jsonClient = JsonConvert.SerializeObject(clientRows, Formatting.Indented);
                Directory.CreateDirectory(_outputDataPath);

                if (shouldExportCsv)
                {
                    var csvPath = Path.Combine(_outputDataPath, $"Table_{sheet.Name}Data.csv");
                    File.WriteAllText(csvPath, csvSb.ToString(), new UTF8Encoding(true));
                    Debug.Log($"[CSV Export] {csvPath}");
                    ImportCsvToLocalization(csvPath);
                }
                else
                {
                    File.WriteAllText(Path.Combine(_outputDataPath, $"Table_{sheet.Name}Data.json"), jsonClient);
                }

                Debug.Log($"{sheet.Name}_Client.json, {sheet.Name}_Server.json Create Complete");
            }
        }
    }
    
    public void ImportCsvToLocalization(string csvPath)
    {
        if (string.IsNullOrEmpty(csvPath)) 
            return;

        var collection = LocalizationEditorSettings.GetStringTableCollection("MessageTable");
        using (var stream = new StreamReader(csvPath))
        {
            Csv.ImportInto(stream, collection);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("CSV Import to Localization Table is Completed!");
    }
}
