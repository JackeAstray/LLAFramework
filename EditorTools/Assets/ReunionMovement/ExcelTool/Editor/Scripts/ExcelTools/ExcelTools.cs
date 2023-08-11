using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace GameLogic.EditorTools
{
    /// <summary>
    /// Excel工具类
    /// 注意：该插件调用了Log工具，如果单独使用报错可删除Log部分或者连同Log一起使用
    /// </summary>
    public class ExcelTools
    {
        static readonly string toDir = "Assets/ReunionMovement/ExcelTool/Editor/Resources/ExcelTools";  // 源文件路径
        static readonly string scriptOutPutPath = "Assets/Scripts/AutoScripts/";             // 脚本输出路径
        static readonly string dataOutPutPath = "Assets/Resources/AutoDatabase/";            // 数据表输出路径

        static int tableRows_Max = 3;                                           // 最大行数
        static int tableRows_1 = 0;                                             // 第一行中文名称
        static int tableRows_2 = 1;                                             // 第二行数据类型
        static int tableRows_3 = 2;                                             // 第三行英文名称

        #region 表格 -> 脚本
        [MenuItem("工具箱/表格处理/表格 -> 脚本", false, 1)]
        public static void ExcelToScripts()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToScripts(path);
            }
            Log.Debug("表格转为脚本完成！");
        }
        /// <summary>
        /// Excel到脚本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool ExcelToScripts(string path)
        {
            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("无法读取“{0}”。似乎这不是一个xlsx文件!", path);
                EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                return false;
            }

            List<SheetData> sheets = new List<SheetData>();
            //处理表数据
            foreach (DataTable table in excel.ResultSet.Tables)
            {
                string tableName = table.TableName.Trim();
                //判断表名称前面是否有#  有则忽略
                if (tableName.StartsWith("#"))
                {
                    continue;
                }

                SheetData sheet = new SheetData();
                sheet.table = table;

                if (table.Rows.Count < tableRows_Max)
                {
                    EditorUtility.ClearProgressBar();
                    string msg = string.Format("无法分析“{0}”。1、检查行数：Excel文件应至少包含三行（第一行：中文名称，第二行：数据类型，第三行：英文名称）!\n2、检查Sheet是否存在多个！", path);
                    EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                    return false;
                }
                //设置类名
                sheet.itemClassName = tableName;

                if (!Tools.CheckClassName(sheet.itemClassName))
                {
                    EditorUtility.ClearProgressBar();
                    string msg = string.Format("工作表名称“{0}”无效，因为该工作表的名称应为类名!", sheet.itemClassName);
                    EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                    return false;
                }
                //字段名称
                object[] fieldNames;
                fieldNames = table.Rows[tableRows_3].ItemArray;
                //字段注释
                object[] fieldNotes;
                fieldNotes = table.Rows[tableRows_1].ItemArray;
                //字段类型
                object[] fieldTypes;
                fieldTypes = table.Rows[tableRows_2].ItemArray;

                for (int i = 0, imax = fieldNames.Length; i < imax; i++)
                {
                    string fieldNameStr = fieldNames[i].ToString().Trim();
                    string fieldNoteStr = fieldNotes[i].ToString().Trim();
                    string fieldTypeStr = fieldTypes[i].ToString().Trim();
                    //检查字段名
                    if (string.IsNullOrEmpty(fieldNameStr))
                    {
                        break;
                    }
                    if (!Tools.CheckFieldName(fieldNameStr))
                    {
                        EditorUtility.ClearProgressBar();
                        string msg = string.Format("无法分析“{0}”，因为字段名“{1}”无效!", path, fieldNameStr);
                        EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                        return false;
                    }

                    //解析类型
                    FieldTypes fieldType = GetFieldType(fieldTypeStr);

                    FieldData field = new FieldData();
                    field.fieldName = fieldNameStr;
                    field.fieldNotes = fieldNoteStr;
                    field.fieldIndex = i;
                    field.fieldType = fieldType;
                    field.fieldTypeName = fieldTypeStr;

                    if (fieldType == FieldTypes.Unknown)
                    {
                        fieldType = FieldTypes.UnknownList;
                        if (fieldTypeStr.StartsWith("[") && fieldTypeStr.EndsWith("]"))
                        {
                            fieldTypeStr = fieldTypeStr.Substring(1, fieldTypeStr.Length - 2).Trim();
                        }
                        else if (fieldTypeStr.EndsWith("[]"))
                        {
                            fieldTypeStr = fieldTypeStr.Substring(0, fieldTypeStr.Length - 2).Trim();
                        }
                        else
                        {
                            fieldType = FieldTypes.Unknown;
                        }

                        field.fieldType = field.fieldType == FieldTypes.UnknownList ? FieldTypes.CustomTypeList : FieldTypes.CustomType;
                    }

                    sheet.fields.Add(field);
                }

                sheets.Add(sheet);
            }

            for (int i = 0; i < sheets.Count; i++)
            {
                GenerateScript(sheets[i]);
            }

            return true;
        }

        /// <summary>
        /// 生成脚本
        /// </summary>
        /// <param name="sheet"></param>
        static async void GenerateScript(SheetData sheet)
        {
            string ScriptTemplate = @"//此脚本为自动生成 {_CREATE_TIME_} <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    
    [Serializable]
    public class {_0_}
    {
        {_1_}

        public override string ToString()
        {
            return string.Format(
                {_2_},
                {_3_}
            );
        }
    }
}
";
            var dataName = sheet.itemClassName;
            var str = GenerateDataScript(ScriptTemplate, dataName, sheet.fields);
            await Tools.SaveFile(scriptOutPutPath + dataName + ".cs", str);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建数据结构脚本
        /// </summary>
        /// <param name="template"></param>
        /// <param name="scriptName"></param>
        /// <param name="fieldDatas"></param>
        /// <returns></returns>
        static string GenerateDataScript(string template, string scriptName, List<FieldData> fieldDatas)
        {
            StringBuilder privateType = new StringBuilder();
            privateType.AppendLine();

            string toString_1 = "";
            string toString_2 = "";

            string additional = "{{ get; set; }}";

            for (int i = 0; i < fieldDatas.Count; i++)
            {
                var typeName = GetFieldTypeString(fieldDatas[i].fieldType, fieldDatas[i].fieldTypeName);

                string attribute = string.Format("        public {0} {1} {2}    //{3}", typeName, fieldDatas[i].fieldName, additional, fieldDatas[i].fieldNotes);
                privateType.AppendFormat(attribute);
                privateType.AppendLine();

                int value = i + 1;
                toString_1 += fieldDatas[i].fieldName + "={" + value + "}";
                if (i < fieldDatas.Count - 1)
                    toString_1 += ",";

                toString_2 += "this." + fieldDatas[i].fieldName;
                if (i < fieldDatas.Count - 1)
                    toString_2 += ",\r\n                ";

            }

            string str = template;
            str = str.Replace("{_0_}", scriptName);
            str = str.Replace("{_1_}", privateType.ToString());
            str = str.Replace("{_2_}", "\"[" + toString_1 + "]\"");
            str = str.Replace("{_3_}", toString_2);
            str = str.Replace("{_CREATE_TIME_}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return str;
        }
        #endregion

        #region 表格 -> Json
        [MenuItem("工具箱/表格处理/表格 -> JSON", false, 2)]
        public static void ExcelToJson()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            if (xlsxFiles.Count <= 0)
            {
                Log.Error("未找到任何表格！");
                return; 
            }

            foreach (var path in xlsxFiles)
            {
                ExcelToJson(path);
            }

            Log.Debug("表格转为Json完成！");
        }

        /// <summary>
        /// Excel 转 Json
        /// </summary>
        /// <param name="path"></param>
        public static void ExcelToJson(string path)
        {
            //等待编译结束
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "等待编译结束。", "OK");
                return;
            }

            //查看路径是否存在
            if (Directory.Exists(dataOutPutPath) == false)
            {
                Directory.CreateDirectory(dataOutPutPath);
            }

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("文件“{0}”不是表格！", path);
                Log.Warning(msg);
                return;
            }

            //获取Excel文件的绝对路径
            List<string> strArray = path.Split('\\').ToList();
            string output = dataOutPutPath + strArray[strArray.Count - 1];
            output = output.Replace(".xlsx", ".json");
            excel.ConvertToJson(output);

            //刷新本地资源
            AssetDatabase.Refresh();
        }
        #endregion

        #region 表格 -> Xml
        [MenuItem("工具箱/表格处理/表格 -> XML", false, 3)]
        public static void ExcelToXml()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToXml(path);
            }

            Log.Error("表格转为XML完成！");
        }
        /// <summary>
        /// Excel 转 Xml
        /// </summary>
        /// <param name="path"></param>
        public static void ExcelToXml(string path)
        {
            //等待编译结束
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "等待编译结束。", "OK");
                return;
            }

            //查看路径是否存在
            if (Directory.Exists(dataOutPutPath) == false)
            {
                Directory.CreateDirectory(dataOutPutPath);
            }

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("文件“{0}”不是表格！", path);
                Log.Warning(msg);
                return;
            }

            //获取Excel文件的绝对路径
            List<string> strArray = path.Split('\\').ToList();
            string output = dataOutPutPath + strArray[strArray.Count - 1];
            output = output.Replace(".xlsx", ".xml");
            excel.ConvertToXml(output);

            //刷新本地资源
            AssetDatabase.Refresh();
        }
        #endregion


        #region 表格 -> LUA
        [MenuItem("工具箱/表格处理/表格 -> LUA", false, 4)]
        public static void ExcelToLua()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToLua(path);
            }

            Log.Debug("表格转为Lua完成！");
        }
        /// <summary>
        /// Excel 转 Xml
        /// </summary>
        /// <param name="path"></param>
        public static void ExcelToLua(string path)
        {
            //等待编译结束
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "等待编译结束。", "OK");
                return;
            }

            //查看路径是否存在
            if (Directory.Exists(dataOutPutPath) == false)
            {
                Directory.CreateDirectory(dataOutPutPath);
            }

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("文件“{0}”不是表格！", path);
                Log.Warning(msg);
                return;
            }

            //获取Excel文件的绝对路径
            List<string> strArray = path.Split('\\').ToList();
            string output = dataOutPutPath + strArray[strArray.Count - 1];
            output = output.Replace(".xlsx", ".lua");
            excel.ConvertToLua(output,Encoding.UTF8);

            //刷新本地资源
            AssetDatabase.Refresh();
        }
        #endregion
        //----------------------工具----------------------

        /// <summary>
        /// 获取所有的xlsx文件路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllConfigFiles(string filetype = "*.xlsx")
        {
            List<string> tableList = new List<string>();
            //等待编译结束
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "等待编译结束。", "OK");
                return null;
            }
            //查看路径是否存在
            if (Directory.Exists(toDir) == false)
            {
                Directory.CreateDirectory(toDir);
                return null;
            }
            //查找文件目录
            foreach (var path in Directory.GetFiles(toDir, "*", SearchOption.AllDirectories))
            {
                var suffix = Path.GetExtension(path);
                if (suffix != ".xlsx" && suffix != ".xls")
                {
                    string msg = string.Format("文件“{0}”不是表格！", path);
                    Log.Warning(msg);
                    continue;
                }
                tableList.Add(path);
            }

            if(tableList.Count <= 0)
            {
                Log.Error("没有找到表格！");
            }

            return tableList;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        static FieldTypes GetFieldType(string typeName)
        {
            FieldTypes type = FieldTypes.Unknown;
            if (!string.IsNullOrEmpty(typeName))
            {
                switch (typeName.Trim().ToLower())
                {
                    case "bool":
                        type = FieldTypes.Bool;
                        break;
                    case "int":
                    case "int32":
                        type = FieldTypes.Int;
                        break;
                    case "ints":
                    case "int[]":
                    case "[int]":
                    case "int32s":
                    case "int32[]":
                    case "[int32]":
                        type = FieldTypes.Ints;
                        break;
                    case "float":
                        type = FieldTypes.Float;
                        break;
                    case "floats":
                    case "float[]":
                    case "[float]":
                        type = FieldTypes.Floats;
                        break;
                    case "long":
                    case "int64":
                        type = FieldTypes.Long;
                        break;
                    case "longs":
                    case "long[]":
                    case "[long]":
                    case "int64s":
                    case "int64[]":
                    case "[int64]":
                        type = FieldTypes.Longs;
                        break;
                    case "vector2":
                        type = FieldTypes.Vector2;
                        break;
                    case "vector3":
                        type = FieldTypes.Vector3;
                        break;
                    case "vector4":
                        type = FieldTypes.Vector4;
                        break;
                    case "rect":
                    case "rectangle":
                        type = FieldTypes.Rect;
                        break;
                    case "color":
                    case "colour":
                        type = FieldTypes.Color;
                        break;
                    case "string":
                        type = FieldTypes.String;
                        break;
                    case "strings":
                    case "string[]":
                    case "[string]":
                        type = FieldTypes.Strings;
                        break;
                }
            }
            return type;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="fieldTypes"></param>
        /// <returns></returns>
        static string GetFieldTypeString(FieldTypes fieldTypes, string fieldTypeName)
        {
            string result = string.Empty;
            switch (fieldTypes)
            {
                case FieldTypes.Bool:
                    result = "bool";
                    break;
                case FieldTypes.Int:
                    result = "int";
                    break;
                case FieldTypes.Ints:
                    result = "List<int>";
                    break;
                case FieldTypes.Float:
                    result = "float";
                    break;
                case FieldTypes.Floats:
                    result = "List<float>";
                    break;
                case FieldTypes.Long:
                    result = "long";
                    break;
                case FieldTypes.Longs:
                    result = "List<long>";
                    break;
                case FieldTypes.Vector2:
                    result = "Vector2";
                    break;
                case FieldTypes.Vector3:
                    result = "Vector3";
                    break;
                case FieldTypes.Vector4:
                    result = "Vector4";
                    break;
                case FieldTypes.Rect:
                    result = "Rect";
                    break;
                case FieldTypes.Color:
                    result = "Color";
                    break;
                case FieldTypes.String:
                    result = "string";
                    break;
                case FieldTypes.Strings:
                    result = "List<string>";
                    break;
                case FieldTypes.CustomType:
                    result = "fieldTypeName";
                    break;
                case FieldTypes.CustomTypeList:
                    result = "List<fieldTypeName>";
                    break;
            }

            return result;
        }
    }

    /// <summary>
    /// 单张表数据
    /// </summary>
    public class SheetData
    {
        public DataTable table;
        public string itemClassName;
        public bool keyToMultiValues;
        public bool internalData;
        public List<FieldData> fields = new List<FieldData>();
    }

    /// <summary>
    /// 字段数据
    /// </summary>
    public class FieldData
    {
        public string fieldName;        //字段名称
        public string fieldNotes;       //字段注释
        public int fieldIndex;          //字段索引
        public FieldTypes fieldType;    //字段类型
        public string fieldTypeName;    //字段类型名称
    }
}