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
using System.Reflection;
using GameLogic.Sqlite;

namespace GameLogic.EditorTools
{
    /// <summary>
    /// Excel工具类
    /// 注意：该插件调用了Log工具，如果单独使用报错可删除Log部分或者连同Log一起使用
    /// </summary>
    public class ExcelTools
    {
        static readonly string toDir = "Assets/ReunionMovement/ExcelTool/Editor/Resources/ExcelTools";  // 源文件路径
        static readonly string scriptOutPutPath = "Assets/ReunionMovement/GenerateScript/AutoScripts/";             // 脚本输出路径
        static readonly string dataOutPutPath = "Assets/Resources/AutoDatabase/";            // 数据表输出路径
        static readonly string scriptableOutPutPath = "Assets/Resources/ScriptableObjects/"; // 脚本对象输出路径
        static readonly string jsonMgrOutPutPath = "Assets/ReunionMovement/App/Module/DatabaseModule/JsonDatabaseModule.cs"; // 脚本对象输出路径

        static int tableRows_Max = 3;                                           // 最大行数
        static int tableRows_1 = 0;                                             // 第一行中文名称
        static int tableRows_2 = 1;                                             // 第二行数据类型
        static int tableRows_3 = 2;                                             // 第三行英文名称

        #region 表格 -> 脚本
        [MenuItem("工具箱/表格处理/表格 -> 脚本（同时重写JsonDatabaseModule）", false, 1)]
        public static void ExcelToScripts()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            List<SheetData> sheets = new List<SheetData>();

            foreach (var path in xlsxFiles)
            {
                var sheetData = ExcelToScripts(path);
                if (sheetData != null)
                {
                    sheets.AddRange(sheetData);
                }
            }

            // 生成 JsonDatabaseModule 脚本
            GenerateJsonDatabaseModuleScript(sheets);
            Log.Debug("表格转为脚本完成！");
        }

        [MenuItem("工具箱/表格处理/表格 -> 脚本（包含ScriptableObject对象生成）", false, 2)]
        public static void ExcelToScripts_ScriptableObject()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToScripts(path, true);
            }
            Log.Debug("表格转为脚本完成！");
        }

        [MenuItem("工具箱/表格处理/表格 -> 脚本（包含SQLite管理脚本生成）", false, 2)]
        public static void ExcelToScripts_SQLite()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            List<SheetData> sheets = new List<SheetData>();

            foreach (var path in xlsxFiles)
            {
                var sheetData = ExcelToScripts(path);
                if (sheetData != null)
                {
                    sheets.AddRange(sheetData);
                }
            }

            // 动态生成 SqliteMgr 脚本
            GenerateSqliteMgrScript(sheets);

            Log.Debug("表格转为脚本完成！");
        }
        /// <summary>
        /// Excel到脚本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<SheetData> ExcelToScripts(string path, bool createScriptableObjects = false)
        {
            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("无法读取“{0}”。似乎这不是一个xlsx文件!", path);
                EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                return null;
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
                    return null;
                }
                //设置类名
                sheet.itemClassName = tableName;

                if (!Tools.CheckClassName(sheet.itemClassName))
                {
                    EditorUtility.ClearProgressBar();
                    string msg = string.Format("工作表名称“{0}”无效，因为该工作表的名称应为类名!", sheet.itemClassName);
                    EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                    return null;
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
                        return null;
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

                if (createScriptableObjects)
                {
                    GenerateScriptDTO(sheets[i]);
                    GenerateScript_ScriptableObjectList(sheets[i], i);
                }
            }

            return sheets;
        }

        /// <summary>
        /// 生成Json、数据库通用脚本
        /// </summary>
        /// <param name="sheet"></param>
        static async void GenerateScript(SheetData sheet)
        {
            string ScriptTemplate = @"//此脚本为工具生成，请勿手动创建 {_CREATE_TIME_} <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace GameLogic
{
    [Serializable]
    [UnityEngine.Scripting.Preserve]
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
        /// 生成Json、数据库通用脚本
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

            // 附加
            string additional = "{{ get; set; }}";

            for (int i = 0; i < fieldDatas.Count; i++)
            {
                var typeName = GetFieldTypeString(fieldDatas[i].fieldType, fieldDatas[i].fieldTypeName);

                // 属性
                string attribute = "";

                if (fieldDatas[i].fieldName == "Id")
                {
                    attribute = string.Format("        [PrimaryKey][AutoIncrement] public {0} {1}{2}    //{3}", typeName, fieldDatas[i].fieldName, additional, fieldDatas[i].fieldNotes);
                }
                else
                {
                    attribute = string.Format("        public {0} {1}{2}    //{3}", typeName, fieldDatas[i].fieldName, additional, fieldDatas[i].fieldNotes);
                }

                privateType.AppendFormat(attribute);
                privateType.AppendLine();

                int value = i + 1;
                toString_1 += fieldDatas[i].fieldName + "={" + value + "}";
                if (i < fieldDatas.Count - 1)
                {
                    toString_1 += ",";
                }

                toString_2 += "this." + fieldDatas[i].fieldName;
                if (i < fieldDatas.Count - 1)
                {
                    toString_2 += ",\r\n                ";
                }
            }

            string str = template;
            str = str.Replace("{_0_}", scriptName);
            str = str.Replace("{_1_}", privateType.ToString());
            str = str.Replace("{_2_}", "\"[" + toString_1 + "]\"");
            str = str.Replace("{_3_}", toString_2);
            str = str.Replace("{_CREATE_TIME_}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return str;
        }

        /// <summary>
        /// 生成ScriptableObject用脚本
        /// </summary>
        /// <param name="sheet"></param>
        static async void GenerateScriptDTO(SheetData sheet)
        {
            string ScriptTemplate = @"//此脚本为工具生成，请勿手动创建 {_CREATE_TIME_} <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace GameLogic
{
    [Serializable]
    public class {_0_}DTO
    {
        {_1_}
        public override string ToString()
        {
            return string.Format(
                {_2_},
                {_3_}
            );
        }

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public {_0_} ToEntity()
        {
            return new {_0_}
            {
                {_4_}
            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static {_0_}DTO FromEntity({_0_} entity)
        {
            return new {_0_}DTO
            {
                {_5_}
            };
        }
    }
}
";
            var dataName = sheet.itemClassName;
            var str = GenerateDataScriptDTO(ScriptTemplate, dataName, sheet.fields);
            await Tools.SaveFile(scriptOutPutPath + dataName + "DTO.cs", str);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成ScriptableObject用脚本
        /// </summary>
        /// <param name="template"></param>
        /// <param name="scriptName"></param>
        /// <param name="fieldDatas"></param>
        /// <returns></returns>
        static string GenerateDataScriptDTO(string template, string scriptName, List<FieldData> fieldDatas)
        {
            StringBuilder privateType = new StringBuilder();
            privateType.AppendLine();

            StringBuilder toEntityAssignments = new StringBuilder();
            StringBuilder fromEntityAssignments = new StringBuilder();

            string toString_1 = "";
            string toString_2 = "";

            // 附加
            string additional = ";";

            // 计算字段名的最大长度，用于对齐
            int maxFieldNameLength = fieldDatas.Max(f => f.fieldName.Length);

            for (int i = 0; i < fieldDatas.Count; i++)
            {
                var typeName = GetFieldTypeString(fieldDatas[i].fieldType, fieldDatas[i].fieldTypeName);

                // 属性
                string attribute = string.Format("        public {0} {1}{2}    //{3}", typeName, fieldDatas[i].fieldName, additional, fieldDatas[i].fieldNotes);
                privateType.AppendFormat(attribute);
                privateType.AppendLine();

                string space = "                ";
                if (i == 0)
                {
                    space = "";
                }

                // DTO -> Entity
                toEntityAssignments.AppendLine($"{space}{fieldDatas[i].fieldName.PadRight(maxFieldNameLength)} = this.{fieldDatas[i].fieldName},");

                // Entity -> DTO
                fromEntityAssignments.AppendLine($"{space}{fieldDatas[i].fieldName.PadRight(maxFieldNameLength)} = entity.{fieldDatas[i].fieldName},");

                int value = i + 1;
                toString_1 += fieldDatas[i].fieldName + "={" + value + "}";
                if (i < fieldDatas.Count - 1)
                {
                    toString_1 += ",";
                }

                toString_2 += "this." + fieldDatas[i].fieldName;
                if (i < fieldDatas.Count - 1)
                {
                    toString_2 += ",\r\n                ";
                }
            }

            string str = template;
            str = str.Replace("{_0_}", scriptName);
            str = str.Replace("{_1_}", privateType.ToString());
            str = str.Replace("{_2_}", "\"[" + toString_1 + "]\"");
            str = str.Replace("{_3_}", toString_2);
            str = str.Replace("{_4_}", toEntityAssignments.ToString().TrimEnd(','));
            str = str.Replace("{_5_}", fromEntityAssignments.ToString().TrimEnd(','));
            str = str.Replace("{_CREATE_TIME_}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return str;
        }


        /// <summary>
        /// 生成ScriptableObjectList脚本
        /// </summary>
        /// <param name="sheet"></param>
        static async void GenerateScript_ScriptableObjectList(SheetData sheet, int order)
        {
            string ScriptTemplate = @"//此脚本为工具生成，请勿手动创建 {_CREATE_TIME_} <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [CreateAssetMenu(fileName = ""{_0_}Container"", menuName = ""ScriptableObjects/{_1_}Container"", order = {_2_})]
    public class {_3_}Container : ScriptableObject
    {
        {_4_}
    }
}
";
            var dataName = sheet.itemClassName;
            var str = GenerateDataScript_ScriptableObjectList(ScriptTemplate, dataName, sheet.fields, order);
            await Tools.SaveFile(scriptOutPutPath + dataName + "Container.cs", str);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建ScriptableObjectList脚本
        /// </summary>
        /// <param name="template"></param>
        /// <param name="scriptName"></param>
        /// <param name="fieldDatas"></param>
        /// <returns></returns>
        static string GenerateDataScript_ScriptableObjectList(string template, string scriptName, List<FieldData> fieldDatas, int order)
        {
            StringBuilder privateType = new StringBuilder();
            privateType.AppendLine();

            string additional = ";";

            var typeName = scriptName;

            string attribute = string.Format("        public List<{0}> {1}{2}", scriptName + "DTO", "configs", additional);
            privateType.AppendFormat(attribute);

            string str = template;
            str = str.Replace("{_0_}", scriptName);
            str = str.Replace("{_1_}", scriptName);
            str = str.Replace("{_2_}", order.ToString());
            str = str.Replace("{_3_}", scriptName);
            str = str.Replace("{_4_}", privateType.ToString());
            str = str.Replace("{_CREATE_TIME_}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return str;
        }

        /// <summary>
        /// 生成 SqliteMgr 脚本
        /// </summary>
        /// <param name="sheets"></param>
        static async void GenerateSqliteMgrScript(List<SheetData> sheets)
        {
            string ScriptTemplate = @"//此脚本为工具生成，请勿手动创建 {_CREATE_TIME_} <ExcelTo>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Base;

namespace GameLogic.Sqlite
{
    public class SqliteMgr : SingletonMgr<SqliteMgr>
    {
        private DataService dataService;

        // 初始化数据库
        public void Initialize(string dbName, string password = null)
        {
            dataService = new DataService(dbName, password);
        }

        {_METHODS_}
        //  销毁
        public void OnDestroy()
        {
            Close();
        }

        // 关闭数据库连接
        public void Close()
        {
            dataService.Close();
        }
    }
}
";

            StringBuilder methodsBuilder = new StringBuilder();

            foreach (var sheet in sheets)
            {
                var dataName = sheet.itemClassName;

                methodsBuilder.AppendLine($@"
        // 查询所有{dataName}
        public IEnumerable<{dataName}> GetAll{dataName}()
        {{
            return dataService.Query<{dataName}>();
        }}

        // 根据条件查询{dataName}
        public IEnumerable<{dataName}> Get{dataName}ByCondition(string condition, params object[] args)
        {{
            return dataService.Query<{dataName}>(condition, args);
        }}

        // 插入{dataName}
        public void Insert{dataName}({dataName} obj)
        {{
            dataService.Insert(obj);
        }}

        // 更新{dataName}
        public void Update{dataName}({dataName} obj)
        {{
            dataService.Update(obj);
        }}

        // 删除{dataName}
        public void Delete{dataName}({dataName} obj)
        {{
            dataService.Delete(obj);
        }}
        ");
            }

            var str = ScriptTemplate.Replace("{_METHODS_}", methodsBuilder.ToString());
            str = str.Replace("{_CREATE_TIME_}", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            await Tools.SaveFile(scriptOutPutPath + "SqliteMgr.cs", str);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成 JsonDatabaseModule 脚本
        /// </summary>
        /// <param name="sheets"></param>
        static void GenerateJsonDatabaseModuleScript(List<SheetData> sheets)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("using System.Collections;");
            scriptBuilder.AppendLine("using System.Collections.Generic;");
            scriptBuilder.AppendLine("using UnityEngine;");
            scriptBuilder.AppendLine("using System.Linq;");
            scriptBuilder.AppendLine("using LitJson;");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("namespace GameLogic");
            scriptBuilder.AppendLine("{");
            scriptBuilder.AppendLine("    public class JsonDatabaseModule : CustommModuleInitialize");
            scriptBuilder.AppendLine("    {");
            scriptBuilder.AppendLine("        public static JsonDatabaseModule Instance = new JsonDatabaseModule();");
            scriptBuilder.AppendLine("        public bool IsInited { get; private set; }");
            scriptBuilder.AppendLine("        private double initProgress = 0;");
            scriptBuilder.AppendLine("        public double InitProgress { get { return initProgress; } }");
            scriptBuilder.AppendLine();

            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        // {sheet.itemClassName} 配置表");
                scriptBuilder.AppendLine($"        Dictionary<int, {sheet.itemClassName}> {sheet.itemClassName.ToLower()}s = new Dictionary<int, {sheet.itemClassName}>();");
            }

            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        string filePath = AppConfig.DatabasePath;");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        string {sheet.itemClassName.ToLower()}_FileName = \"{sheet.itemClassName}.json\";");
            }
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        public IEnumerator Init()");
            scriptBuilder.AppendLine("        {");
            scriptBuilder.AppendLine("            initProgress = 0;");
            scriptBuilder.AppendLine("            InitConfig();");
            scriptBuilder.AppendLine("            yield return null;");
            scriptBuilder.AppendLine("            initProgress = 100;");
            scriptBuilder.AppendLine("            IsInited = true;");
            scriptBuilder.AppendLine("            Log.Debug(\"JsonDatabaseModule 初始化完成\");");
            scriptBuilder.AppendLine("        }");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        public void ClearData()");
            scriptBuilder.AppendLine("        {");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"            {sheet.itemClassName.ToLower()}s.Clear();");
            }
            scriptBuilder.AppendLine("            Log.Debug(\"JsonDatabaseModule 清除数据\");");
            scriptBuilder.AppendLine("        }");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        void InitConfig()");
            scriptBuilder.AppendLine("        {");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"            Load{sheet.itemClassName}();");
            }
            scriptBuilder.AppendLine("        }");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        public void UpdateTime(float elapseSeconds, float realElapseSeconds)");
            scriptBuilder.AppendLine("        {");
            scriptBuilder.AppendLine("        }");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        #region 加载数据");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        public void Load{sheet.itemClassName}()");
                scriptBuilder.AppendLine("        {");
                scriptBuilder.AppendLine($"            List<{sheet.itemClassName}> configs = new List<{sheet.itemClassName}>();");
                scriptBuilder.AppendLine("            string fullPath;");
                scriptBuilder.AppendLine($"            bool exists = PathUtils.GetFullPath(filePath + {sheet.itemClassName.ToLower()}_FileName, out fullPath);");
                scriptBuilder.AppendLine("            if (exists)");
                scriptBuilder.AppendLine("            {");
                scriptBuilder.AppendLine($"                string content = PathUtils.ReadFile(filePath, {sheet.itemClassName.ToLower()}_FileName);");
                scriptBuilder.AppendLine($"                configs = JsonMapper.ToObject<List<{sheet.itemClassName}>>(content);");
                scriptBuilder.AppendLine("            }");
                scriptBuilder.AppendLine("            else");
                scriptBuilder.AppendLine("            {");
                scriptBuilder.AppendLine($"                TextAsset json = ResourcesModule.Instance.Load<TextAsset>(\"AutoDatabase/{sheet.itemClassName}\");");
                scriptBuilder.AppendLine($"                PathUtils.WriteFile(json.text, filePath, {sheet.itemClassName.ToLower()}_FileName);");
                scriptBuilder.AppendLine($"                configs = JsonMapper.ToObject<List<{sheet.itemClassName}>>(json.text);");
                scriptBuilder.AppendLine("            }");
                scriptBuilder.AppendLine("            foreach (var tempData in configs)");
                scriptBuilder.AppendLine("            {");
                scriptBuilder.AppendLine($"                {sheet.itemClassName.ToLower()}s.Add(tempData.Id, tempData);");
                scriptBuilder.AppendLine("            }");
                scriptBuilder.AppendLine("        }");
                scriptBuilder.AppendLine();
            }
            scriptBuilder.AppendLine("        #endregion");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        #region 获取数据");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        public Dictionary<int, {sheet.itemClassName}> Get{sheet.itemClassName}()");
                scriptBuilder.AppendLine("        {");
                scriptBuilder.AppendLine($"            return {sheet.itemClassName.ToLower()}s;");
                scriptBuilder.AppendLine("        }");
                scriptBuilder.AppendLine();
            }
            scriptBuilder.AppendLine("        #endregion");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        #region 保存");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        public void Save{sheet.itemClassName}()");
                scriptBuilder.AppendLine("        {");
                scriptBuilder.AppendLine($"            List<{sheet.itemClassName}> tempList = {sheet.itemClassName.ToLower()}s.Values.ToList();");
                scriptBuilder.AppendLine($"            string jsonStr = JsonMapper.ToJson(tempList, true);");
                scriptBuilder.AppendLine($"            PathUtils.WriteFile(jsonStr, filePath, {sheet.itemClassName.ToLower()}_FileName);");
                scriptBuilder.AppendLine("        }");
                scriptBuilder.AppendLine();
            }
            scriptBuilder.AppendLine("        #endregion");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        #region 通过索引查询数据");
            foreach (var sheet in sheets)
            {
                scriptBuilder.AppendLine($"        public {sheet.itemClassName} Get{sheet.itemClassName}ByNumber(int number)");
                scriptBuilder.AppendLine("        {");
                scriptBuilder.AppendLine($"            if ({sheet.itemClassName.ToLower()}s.TryGetValue(number, out var value))");
                scriptBuilder.AppendLine("            {");
                scriptBuilder.AppendLine("                return value;");
                scriptBuilder.AppendLine("            }");
                scriptBuilder.AppendLine("            return null;");
                scriptBuilder.AppendLine("        }");
                scriptBuilder.AppendLine();
            }
            scriptBuilder.AppendLine("        #endregion");
            scriptBuilder.AppendLine("    }");
            scriptBuilder.AppendLine("}");

            File.WriteAllText(jsonMgrOutPutPath, scriptBuilder.ToString());
            AssetDatabase.Refresh();
        }
        #endregion

        #region 表格 -> Json
        [MenuItem("工具箱/表格处理/表格 -> JSON", false, 3)]
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
        [MenuItem("工具箱/表格处理/表格 -> XML", false, 4)]
        public static void ExcelToXml()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToXml(path);
            }

            Log.Debug("表格转为XML完成！");
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
        [MenuItem("工具箱/表格处理/表格 -> LUA", false, 5)]
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
            excel.ConvertToLua(output, Encoding.UTF8);

            //刷新本地资源
            AssetDatabase.Refresh();
        }
        #endregion

        #region 表格 -> ScriptableObject
        [MenuItem("工具箱/表格处理/表格 -> ScriptableObject", false, 6)]
        public static void ExcelToScriptableObject()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            foreach (var path in xlsxFiles)
            {
                ExcelToScriptableObject(path);
            }

            Log.Debug("表格转为ScriptableObject完成！");
        }

        /// <summary>
        /// Excel 转 ScriptableObject
        /// </summary>
        /// <param name="path"></param>
        public static void ExcelToScriptableObject(string path)
        {
            // 等待编译结束
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "等待编译结束。", "OK");
                return;
            }

            // 查看路径是否存在
            if (Directory.Exists(scriptableOutPutPath) == false)
            {
                Directory.CreateDirectory(scriptableOutPutPath);
            }

            // 构造 Excel 工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("文件“{0}”不是表格！", path);
                Log.Warning(msg);
                return;
            }

            foreach (DataTable table in excel.ResultSet.Tables)
            {
                string tableName = table.TableName.Trim();
                if (tableName.StartsWith("#"))
                {
                    continue; // 忽略以 # 开头的表
                }

                if (table.Rows.Count < tableRows_Max)
                {
                    EditorUtility.ClearProgressBar();
                    string msg = string.Format("无法分析“{0}”。1、检查行数：Excel文件应至少包含三行（第一行：中文名称，第二行：数据类型，第三行：英文名称）!\n2、检查Sheet是否存在多个！", path);
                    EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                    return;
                }

                // 动态生成 ScriptableObject 文件路径
                string assetPath = scriptableOutPutPath + tableName + "Container.asset";

                // 动态获取容器类类型
                Type containerType = Type.GetType($"GameLogic.{tableName}Container, Assembly-CSharp");
                if (containerType == null)
                {
                    Log.Error($"无法获取类型：GameLogic.{tableName}Container, Assembly-CSharp");
                    continue;
                }

                // 动态创建容器实例
                ScriptableObject asset = ScriptableObject.CreateInstance(containerType);

                Type configType = Type.GetType($"GameLogic.{tableName}, Assembly-CSharp");
                if (configType == null)
                {
                    Log.Error($"无法获取类型：GameLogic.{tableName}, Assembly-CSharp");
                    continue;
                }
                // 动态获取 DTO 类类型
                Type configTypeDTO = Type.GetType($"GameLogic.{tableName}DTO, Assembly-CSharp");
                if (configTypeDTO == null)
                {
                    Log.Error($"无法获取类型：GameLogic.{tableName}DTO, Assembly-CSharp");
                    continue;
                }

                // 创建 DTO 列表
                IList configs = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(configTypeDTO));

                // 遍历表数据并填充 DTO 列表
                for (int i = tableRows_Max; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    var config = Activator.CreateInstance(configType);
                    if (config == null)
                    {
                        Log.Error($"无法创建实例：{tableName}");
                        continue;
                    }

                    foreach (DataColumn column in table.Columns)
                    {
                        string fieldName = table.Rows[tableRows_3][column].ToString().Trim();
                        string fieldValue = row[column].ToString().Trim();

                        // 获取属性信息
                        PropertyInfo property = config.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                object value = Convert.ChangeType(fieldValue, property.PropertyType);
                                property.SetValue(config, value);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"属性赋值失败：{fieldName}, 值：{fieldValue}, 错误：{ex.Message}");
                            }
                        }
                        else
                        {
                            Log.Warning($"属性未找到或不可写：{fieldName}");
                        }
                    }

                    // 创建 DTO 实例
                    var configDTO = Activator.CreateInstance(configTypeDTO);

                    // 遍历 GameConfig 的属性并赋值给 GameConfigDTO
                    foreach (PropertyInfo property in config.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var dtoProperty = configDTO.GetType().GetField(property.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (dtoProperty != null)
                        {
                            try
                            {
                                var value = property.GetValue(config);
                                dtoProperty.SetValue(configDTO, value);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"属性赋值失败：{property.Name}, 错误：{ex.Message}");
                            }
                        }
                    }

                    // 将 DTO 添加到列表
                    configs.Add(configDTO);
                }

                // 将 DTO 列表赋值给容器
                FieldInfo configsField = containerType.GetField("configs");
                if (configsField != null)
                {
                    configsField.SetValue(asset, configs);
                }
                else
                {
                    Log.Error($"字段 'configs' 未找到：{tableName}Container");
                }

                // 保存 ScriptableObject
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion

        #region 表格 -> 数据库
        // 添加菜单项
        [MenuItem("工具箱/表格处理/表格 -> 数据库", false, 7)]
        public static void ExcelToDatabase()
        {
            List<string> xlsxFiles = GetAllConfigFiles();

            // 删除现有数据库文件
            string databasePath = Path.Combine(Application.persistentDataPath, SqliteConfig.GameDatabaseName);
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
                Log.Debug($"已删除现有数据库文件：{databasePath}");
            }

            databasePath = Path.Combine(Application.streamingAssetsPath, SqliteConfig.GameDatabaseName);
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
                Log.Debug($"已删除现有数据库文件：{databasePath}");
            }

            foreach (var path in xlsxFiles)
            {
                ExcelToDatabase(path);
            }

            Log.Debug("表格导出到数据库完成！");
        }

        /// <summary>
        /// 将 Excel 数据导出到数据库
        /// </summary>
        /// <param name="path"></param>
        public static void ExcelToDatabase(string path)
        {
            // 构造 Excel 工具类
            ExcelUtility excel = new ExcelUtility(path);

            if (excel.ResultSet == null)
            {
                string msg = string.Format("无法读取“{0}”。似乎这不是一个有效的 Excel 文件!", path);
                EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                return;
            }

            // 数据库密码
            //string password = SqliteConfig.GameDatabasePassword;
            string password = null;

            // 初始化数据库服务
            DataService dataService = new DataService(SqliteConfig.GameDatabaseName, password);

            foreach (DataTable table in excel.ResultSet.Tables)
            {
                string tableName = table.TableName.Trim();

                // 忽略以 # 开头的表
                if (tableName.StartsWith("#"))
                {
                    continue;
                }

                if (table.Rows.Count < tableRows_Max)
                {
                    string msg = string.Format("无法分析“{0}”。表格至少需要包含三行（第一行：中文名称，第二行：数据类型，第三行：英文名称）!", path);
                    EditorUtility.DisplayDialog("ExcelTools", msg, "OK");
                    return;
                }

                Type tableType = Type.GetType($"GameLogic.{tableName}, Assembly-CSharp");

                if (tableType == null)
                {
                    throw new Exception($"类型 GameLogic.{tableName} 不存在，请检查类定义！");
                }

                // 使用反射调用 CreateTable<T>() 方法
                MethodInfo createTableMethod = typeof(DataService).GetMethod("CreateTable").MakeGenericMethod(tableType);
                createTableMethod.Invoke(dataService, null);

                var records = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tableType));
                for (int i = tableRows_Max; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    object record = Activator.CreateInstance(tableType);

                    foreach (DataColumn column in table.Columns)
                    {
                        string fieldName = table.Rows[tableRows_3][column].ToString().Trim();
                        string fieldValue = row[column].ToString().Trim();

                        // 获取属性信息
                        PropertyInfo property = tableType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                object value = Convert.ChangeType(fieldValue, property.PropertyType);
                                property.SetValue(record, value);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"属性赋值失败：{fieldName}, 值：{fieldValue}, 错误：{ex.Message}");
                            }
                        }
                        else
                        {
                            Log.Warning($"属性未找到或不可写：{fieldName}");
                        }
                    }
                    records.Add(record);
                }
                // 使用反射调用 InsertAll<T>() 方法
                MethodInfo insertAllMethod = typeof(DataService).GetMethod("InsertAll").MakeGenericMethod(tableType);
                insertAllMethod.Invoke(dataService, new object[] { records });
            }
            dataService.Close();
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

            if (tableList.Count <= 0)
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
                    case "double":
                        type = FieldTypes.Double;
                        break;
                    case "doubles":
                    case "double[]":
                    case "[double]":
                        type = FieldTypes.Doubles;
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
                case FieldTypes.Double:
                    result = "double";
                    break;
                case FieldTypes.Doubles:
                    result = "List<double>";
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