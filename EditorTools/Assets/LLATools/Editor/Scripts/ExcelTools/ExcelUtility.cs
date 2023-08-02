using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;
using ExcelDataReader;
using LitJson;

namespace GameLogic.Editor
{
    public class ExcelUtility
    {
        /// <summary>
        /// 表格数据集合
        /// </summary>
        private DataSet mResultSet;

        /// <summary>
        /// 用于获取表格数据
        /// </summary>
        public DataSet ResultSet { get { return mResultSet; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="excelFile">Excel file.</param>
        public ExcelUtility(string path)
        {
            //通过文件获取类名
            string className = Path.GetFileNameWithoutExtension(path);

            //检查类名是否有问题
            if (!Tools.CheckClassName(className))
            {
                string msg = string.Format("Excel文件“{0}”无效，因为xlsx文件的名称应为类名！", path);
                Debug.Log(msg);
                return;
            }

            //拷贝一份文件
            int indexOfDot = path.LastIndexOf('.');
            string tempExcel = string.Concat(path.Substring(0, indexOfDot), "_temp_", path.Substring(indexOfDot, path.Length - indexOfDot));
            File.Copy(path, tempExcel);

            //读取拷贝的文件
            Stream stream = null;
            try
            {
                stream = File.OpenRead(tempExcel);
            }
            catch
            {
                File.Delete(tempExcel);
                string msg = string.Format("由于共享冲突，无法打开“{0}”。也许您应该先关闭Excel应用程序！", path);
                Debug.Log(msg);
                return;
            }

            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            mResultSet = reader.AsDataSet();

            reader.Dispose();
            stream.Close();
            File.Delete(tempExcel);
        }

        /// <summary>
        /// 转换为实体类列表
        /// </summary>
        public List<T> ConvertToList<T>()
        {
            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1) return null;
            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[0];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1) return null;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表以保存全部数据
            List<T> list = new List<T>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //创建实例
                Type t = typeof(T);
                ConstructorInfo ct = t.GetConstructor(System.Type.EmptyTypes);
                T target = (T)ct.Invoke(null);
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[1][j].ToString();
                    object value = mSheet.Rows[i][j];
                    //设置属性值
                    SetTargetProperty(target, field, value);
                }

                //添加至列表
                list.Add(target);
            }

            return list;
        }

        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="JsonPath">Json文件路径</param>
        /// <param name="Header">表头行数</param>
        public async void ConvertToJson(string JsonPath/*, Encoding encoding*/)
        {
            var json = GetJson();
            //写入文件
            await Tools.SaveFile(JsonPath, json);
        }

        /// <summary>
        /// 后补的接口，适配旧的调用
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="IdX"></param>
        /// <param name="IdY"></param>
        /// <returns></returns>
        public string GetJson()
        {
            int x = -1;
            int y = -1;
            //var list = new List<object>();
            var json = GetJson(ref x, ref y/*, ref list*/);
            return json;
        }

        /// <summary>
        /// 获取json
        /// </summary>
        /// <returns></returns>
        public string GetJson(ref int IdX, ref int IdY/*, ref List<object> keepFieldList*/)
        {
            IdX = -1;
            IdY = -1;

            //判断Excel文件中是否存在数据表
            if (mResultSet == null || mResultSet.Tables == null)
            {
                return "";
            }

            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1)
            {
                return "";
            }

            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[0];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                return "";
            }

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();
            /************Keep * Mode 保留带*的行列 ********************/
            /*
             *    Id   |   xxx |
             *    1    |   xxx |
             *    2    |   xxx |
             */

            ////每行的数据
            //List<object> rowDatas = new List<object>();
            //字段名称
            List<object> fieldNameRowDatas = new List<object>();
            //字段类型
            List<object> fieldTypeRowDatas = new List<object>();
            //第一行为备注，
            //寻找到id字段行数，以下全为数据
            int skipRowCount = -1;
            int skipColCount = -1;

            //这里skip 防止有人在 备注行直接输入id
            int skipLine = 1;

            for (int i = skipLine; i < 10 && skipColCount == -1; i++)
            {
                var rows = this.GetRowDatas(i);
                //遍历rows
                for (int j = 0; j < rows.Count; j++)
                {
                    if (rows[j].Equals("Id"))
                    {
                        skipRowCount = i;
                        skipColCount = j;
                        fieldNameRowDatas = rows;
                        //获取字段类型
                        var rowtype = this.GetRowDatas(i - 1);
                        fieldTypeRowDatas = rowtype;
                        //
                        break;
                    }
                }
            }


            if (skipRowCount == -1)
            {
                Debug.LogError("表格数据可能有错,没发现Id字段,请检查!");
                return "{}";
            }

            int count = mSheet.Rows.Count;

            IdX = skipColCount;
            IdY = skipRowCount;

            //读取数据
            for (int i = skipRowCount + 1; i < mSheet.Rows.Count; i++)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                //
                for (int j = skipColCount; j < mSheet.Columns.Count; j++)
                {
                    string field = fieldNameRowDatas[j].ToString();
                    //跳过空字段
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    //Key-Value对应
                    var rowdata = mSheet.Rows[i][j];
                    //根据null判断
                    if (rowdata == null)
                    {
                        Debug.LogErrorFormat("表格数据为空：[{0},{1}]", i, j);
                        continue;
                    }

                    var fieldType = fieldTypeRowDatas[j].ToString().ToLower();
                    if (rowdata is DBNull) //空类型判断，赋默认值
                    {
                        if (fieldType == "int" || fieldType == "float" || fieldType == "double")
                        {
                            row[field] = 0;
                        }
                        else if (fieldType == "string")
                        {
                            row[field] = "";
                        }
                        else if (fieldType == "bool")
                        {
                            row[field] = false;
                        }
                        else if (fieldType.Contains("[]")) //空数组
                        {
                            row[field] = "[]";
                        }
                    }
                    else
                    {
                        //string数组，对单个元素加上""
                        if (fieldType == "string[]")
                        {
                            var value = rowdata.ToString();
                            if (value != "[]" && !value.Contains("\"")) //不是空数组,且没有""
                            {
                                if (value.StartsWith("\"["))
                                {
                                    value = value.Replace("\"[", "[\"");
                                    value = value.Replace("]\"", "\"]");
                                }
                                else
                                {
                                    value = value.Replace("[", "[\"");
                                    value = value.Replace("]", "\"]");
                                }

                                value = value.Replace(",", "\",\"");
                                row[field] = value;
                            }
                            else
                            {
                                row[field] = rowdata;
                            }
                        }
                        //其他数组 会被处理成string
                        else if (fieldType.Contains("["))
                        {
                            var value = rowdata.ToString();
                            value = value.Replace("\"[", "[");
                            value = value.Replace("]\"", "]");
                            row[field] = value;
                        }

                        else if (fieldType == "int" || fieldType == "float" || fieldType == "double")
                        {
                            var oldValue = rowdata.ToString();
                            if (fieldType == "int")
                            {
                                int value = 0;
                                if (int.TryParse(oldValue, out value))
                                {
                                    row[field] = value;
                                }
                                else
                                {
                                    row[field] = 0;
                                    Debug.LogErrorFormat("表格数据出错:{0}-{1}", i, j);
                                }
                            }
                            else if (fieldType == "float")
                            {
                                float value = 0;
                                if (float.TryParse(oldValue, out value))
                                {
                                    row[field] = value;
                                }
                                else
                                {
                                    row[field] = 0;
                                    Debug.LogErrorFormat("表格数据出错:{0}-{1}", i, j);
                                }
                            }
                            else if (fieldType == "double")
                            {
                                double value = 0;
                                if (double.TryParse(oldValue, out value))
                                {
                                    row[field] = value;
                                }
                                else
                                {
                                    row[field] = 0;
                                    Debug.LogErrorFormat("表格数据出错:{0}-{1}", i, j);
                                }
                            }
                        }
                        else if (field.Equals("string"))
                        {
                            row[field] = rowdata.ToString();
                        }
                        else
                        {
                            row[field] = rowdata;
                        }
                    }
                }

                //添加到表数据中
                if (row.Count > 0)
                {
                    table.Add(row);
                }
            }
            //生成Json字符串
            string json = JsonMapper.ToJson(table,true);
            //把当字符串的数组 重新处理成数组
            json = json.Replace("\"[", "[").Replace("]\"", "]");
            json = json.Replace("\\\"", "\"");
            json = json.Replace("\"\"\"\"", "\"\"");
            return json;
        }

        /// <summary>
        /// 获取一行数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<object> GetRowDatas(int index)
        {
            List<object> list = new List<object>();

            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1)
            {
                return list;
            }

            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[0];
            //判断数据表内是否存在数据
            if (mSheet.Rows.Count <= index)
            {
                return list;
            }

            //读取数据
            int colCount = mSheet.Columns.Count;
            for (int j = 0; j < colCount; j++)
            {
                object item = mSheet.Rows[index][j];
                list.Add(item);
            }


            return list;
        }

        /// <summary>
        /// 转换为CSV
        /// </summary>
        public void ConvertToCSV(string CSVPath, Encoding encoding)
        {
            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1) return;

            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[0];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1) return;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();

            //读取数据
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    //使用","分割每一个数值
                    stringBuilder.Append(mSheet.Rows[i][j] + ",");
                }

                //使用换行符分割每一行
                stringBuilder.Append("\r\n");
            }

            //写入文件
            using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }


        /// <summary>
        /// 转换为lua
        /// </summary>
        /// <param name="luaPath">lua文件路径</param>
        public void ConvertToLua(string luaPath, Encoding encoding)
        {
            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1)
                return;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("local datas = {");
            stringBuilder.Append("\r\n");

            //读取数据表
            foreach (DataTable mSheet in mResultSet.Tables)
            {
                //判断数据表内是否存在数据
                if (mSheet.Rows.Count < 1)
                    continue;

                //读取数据表行数和列数
                int rowCount = mSheet.Rows.Count;
                int colCount = mSheet.Columns.Count;

                //准备一个列表存储整个表的数据
                List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

                //读取数据
                for (int i = 1; i < rowCount; i++)
                {
                    //准备一个字典存储每一行的数据
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int j = 0; j < colCount; j++)
                    {
                        //读取第1行数据作为表头字段
                        string field = mSheet.Rows[0][j].ToString();
                        //Key-Value对应
                        row[field] = mSheet.Rows[i][j];
                    }
                    //添加到表数据中
                    table.Add(row);
                }
                stringBuilder.Append(string.Format("\t\"{0}\" = ", mSheet.TableName));
                stringBuilder.Append("{\r\n");
                foreach (Dictionary<string, object> dic in table)
                {
                    stringBuilder.Append("\t\t{\r\n");
                    foreach (string key in dic.Keys)
                    {
                        if (dic[key].GetType().Name == "String")
                            stringBuilder.Append(string.Format("\t\t\t\"{0}\" = \"{1}\",\r\n", key, dic[key]));
                        else
                            stringBuilder.Append(string.Format("\t\t\t\"{0}\" = {1},\r\n", key, dic[key]));
                    }
                    stringBuilder.Append("\t\t},\r\n");
                }
                stringBuilder.Append("\t}\r\n");
            }

            stringBuilder.Append("}\r\n");
            stringBuilder.Append("return datas");

            //写入文件
            using (FileStream fileStream = new FileStream(luaPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }


        /// <summary>
        /// 导出为Xml
        /// </summary>
        public void ConvertToXml(string XmlFile)
        {
            //判断Excel文件中是否存在数据表
            if (mResultSet.Tables.Count < 1) return;

            //默认读取第一个数据表
            DataTable mSheet = mResultSet.Tables[0];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1) return;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();
            //创建Xml文件头
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.Append("\r\n");
            //创建根节点
            stringBuilder.Append("<Table>");
            stringBuilder.Append("\r\n");
            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //创建子节点
                stringBuilder.Append("  <Row>");
                stringBuilder.Append("\r\n");
                for (int j = 0; j < colCount; j++)
                {
                    stringBuilder.Append("   <" + mSheet.Rows[0][j].ToString() + ">");
                    stringBuilder.Append(mSheet.Rows[i][j].ToString());
                    stringBuilder.Append("</" + mSheet.Rows[0][j].ToString() + ">");
                    stringBuilder.Append("\r\n");
                }

                //使用换行符分割每一行
                stringBuilder.Append("  </Row>");
                stringBuilder.Append("\r\n");
            }

            //闭合标签
            stringBuilder.Append("</Table>");
            //写入文件
            using (FileStream fileStream = new FileStream(XmlFile, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// 设置目标实例的属性
        /// </summary>
        private void SetTargetProperty(object target, string propertyName, object propertyValue)
        {
            //获取类型
            Type mType = target.GetType();
            //获取属性集合
            PropertyInfo[] mPropertys = mType.GetProperties();
            foreach (PropertyInfo property in mPropertys)
            {
                if (property.Name == propertyName)
                {
                    property.SetValue(target, Convert.ChangeType(propertyValue, property.PropertyType), null);
                }
            }
        }
    }
}