using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;

public class DBManager
{
    private static DBManager _dbInstance = null;

    public static DBManager _DBInstance()
    {
        if (_dbInstance == null)
        {
            _dbInstance = new DBManager();
            _dbInstance.OpenConnect();
        }
        return _dbInstance;
    }

    private string dbName = "data";

    private SqliteConnection connection;
    private SqliteCommand command;
    private SqliteDataReader reader;

    private void Awake()
    {
        OpenConnect();
    }

    private void OnDestroy()
    {
        CloseDB();
    }

    void Init()
    {

    }

    public SqliteDataReader DeleteTable(string tableName)
    {
        string sql = "drop table" + tableName;
        return ExecuteQuery(sql);
    }

    public SqliteDataReader SelectFullTableData(string tableName)
    {
        string queryString = "select * from" + tableName;
        return ExecuteQuery(queryString);
    }

    public List<string> GetDataBySqlQuery(string tableName, string[] fields)
    {
        List<string> list = new List<string>();
        string queryString = "select * from " + tableName;
        reader = ExecuteQuery(queryString);
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object obj = reader.GetValue(i);
                list.Add(obj.ToString());
            }
        }
        return list;
    }

    public SqliteDataReader SelectData(string tableName, string[] values, string[] fields)
    {
        string sql = "select " + values[0];
        for (int i = 0; i < values.Length; i++)
        {
            sql += " ," + values[i];
        }
        sql += " from " + tableName + " where( ";
        for (int i = 0; i < fields.Length-1; i+=2)
        {
            sql += fields[i] + " ='" + fields[i + 1] + " 'and ";
        }

        sql = sql.Substring(0, sql.Length - 4) + ");";
        return ExecuteQuery(sql);
    }

    public SqliteDataReader ExecuteQuery(string queryString)
    {
        command = connection.CreateCommand();
        command.CommandText = queryString;
        reader = command.ExecuteReader();
        return reader;
    }

    public void CreateTable<T>()
    {
        var type = typeof(T);
        string sql = "create table if not exists " + type.Name + "( ";
        var fields = type.GetFields();
        Debug.Log("fields length:" + fields.Length);
        for (int i = 0; i < fields.Length; i++)
        {
            sql += " [" + fields[i].Name + "]" + CS2DB(fields[i].FieldType) + ",";
        }
        sql = sql.TrimEnd(',') + ")";
        Debug.Log("sql:" + sql);
        ExecuteQuery(sql);
    }

    #region 反射原理实现表操作
    //更新表操作
    public void CreateTable(Type type)
    {
        //获取一个类型的所有属性
        PropertyInfo[] p_list = type.GetProperties();

        //获取Table的名字
        string table_name = SQLiteTools.GetTableName(type);

        //获取Table的列名字符串
        string field_list = "(";

        foreach (PropertyInfo item in p_list)
        {
            //对应的属性区域
            field_list += SQLiteTools.GetFieldString(item) + ",";
        }

        //删除最后一个,
        field_list = field_list.Substring(0, field_list.Length - 1);

        field_list += ")";

        //开始构造sql命令
        string sql = "create table if not exists ";
        sql += table_name + field_list + ";";

        Debug.Log(sql);

        ExecuteQuery(sql);

    }

    //查询表操作
    public List<T> QueryTable<T>(string condition = "") where T : new()
    {
        //生成一个类列表
        List<T> listT = new List<T>();
        //获取需要查询的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        //生成查询语句
        string sqlPrefix = "select * from " + tableName;
        string sqlMiddle = " ";
        string sqlSuffix = condition + ";";
        //打印查询内容
        Debug.Log(sqlPrefix + sqlMiddle + sqlSuffix);
        //获取查询结果
        var result = ExecuteQuery(sqlPrefix + sqlMiddle + sqlSuffix);
        //遍历查询内容
        while (result.Read())
        {
            var tmpClass = new T();
            for(int i = 0; i < result.FieldCount; i++)
            {
                var _name = result.GetName(i).ToString();   //类中的属性名
                var _property = typeof(T).GetProperty(_name);   //类中对应属性的数据类型
                var _value = result.GetValue(i).ToString(); //类中属性参数
                if (_value.Equals("") && _property.PropertyType.Name.Contains("Int"))
                {
                    _value = "0";
                }
                //Debug.LogFormat("Name:{0},Value:{1}", _name, _value);
                object v = Convert.ChangeType(_value, _property.PropertyType,null);  //生成对应属性参数
                _property.SetValue(tmpClass, v);    //参数存入类中
            }
            listT.Add(tmpClass);
        }
        return listT;

    }

    //插入数据操作
    public void Insert<T>(object obj)
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        string sqlPrefix = "insert into " + tableName + " values (";
        string sqlMiddle = "";
        string sqlSuffix = ");";
        //从obj中判断是单条还是多条
        //生成sql语句
        //var p_list = obj.GetType().GetProperties();
        //for (int i = 0; i < p_list.Length; i++)
        //{
        //    sqlMiddle += "'" + p_list[i].GetValue(obj).ToString() + "',";
        //}

        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
        {
            string name = descriptor.Name;
            object value = descriptor.GetValue(obj);
            sqlMiddle += "'" + SQLiteTools.sqliteEscape(value) + "',";
        }

        Debug.Log((sqlPrefix + sqlMiddle.Substring(0, sqlMiddle.Length - 1) + sqlSuffix));
        //执行插入操作
        ExecuteQuery(sqlPrefix + sqlMiddle.Substring(0, sqlMiddle.Length - 1) + sqlSuffix);
        //完毕
    }
    /// <summary>
    /// 多条插入方法解析后单条方式提交给单条插入进行处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listT"></param>
    public void Insert<T>(IList listT)
    {
        for (int i = 0; i < listT.Count; i++)
            Insert<T>(listT[i]);
    }

    //删除数据操作
    public void Update<T>(object obj)
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        string sqlPrefix = "update " + tableName + " set ";
        string sqlMiddle = "";
        string sqlSuffix = ");";
        //从obj中判断是单条还是多条
        //生成sql set 部分
        var p_list = obj.GetType().GetProperties();
        for (int i = 0; i < p_list.Length; i++)
        {
            sqlMiddle += SQLiteTools.GetFieldName(p_list[i]) + "='" + p_list[i].GetValue(obj).ToString() + "',";
        }
        sqlMiddle = sqlMiddle.Substring(0, sqlMiddle.Length - 1) + " where (";
        //生成sql where 部分
        for(int i = 0; i < p_list.Length; i++)
        {
            string _param = SQLiteTools.GetFieldName(p_list[i]);
            if (_param.Equals("id"))
            {
                sqlMiddle += _param + "='" + p_list[i].GetValue(obj).ToString() + "'";
                break;
            }
        }
        Debug.Log((sqlPrefix + sqlMiddle + sqlSuffix));
        //执行插入操作
        ExecuteQuery(sqlPrefix + sqlMiddle + sqlSuffix);
    }
    public void Update<T>(IList listT)
    {
        for (int i = 0; i < listT.Count; i++)
            Update<T>(listT[i]);
    }

    //删除数据操作
    public void Delete<T>(object obj)
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        string sqlPrefix = "delete from " + tableName + " where (";
        string sqlMiddle = "";
        string sqlSuffix = ");";
        //从obj中判断是单条还是多条
        //生成sql语句
        var p_list = obj.GetType().GetProperties();
        for (int i = 0; i < p_list.Length; i++)
        {
            sqlMiddle += SQLiteTools.GetFieldName(p_list[i]) + "=" + p_list[i].GetValue(obj) + " and ";
        }
        //执行插入操作
        ExecuteQuery(sqlPrefix + sqlMiddle.Substring(0, sqlMiddle.Length - 4) + sqlSuffix);
    }
    public void Delete<T>(IList listT)
    {
        for (int i = 0; i < listT.Count; i++)
            Delete<T>(listT[i]);
    }

    //删除数据库全部数据操作
    public void DeleteAll<T>()
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        string sql = "delete from " + tableName;
        Debug.Log(sql);
        //执行插入操作
        ExecuteQuery(sql);
    }

    //判断某键值是否存在
    public void CheckReplace<T>(string key, string value, T pItem)
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        //生成查询语句
        string sql = "select * from " + tableName
            + " where " + key + "='" + value + "';";
        Debug.Log(sql);
        var result = ExecuteQuery(sql);
        //	定义结果数据列表
        Dictionary<string, string> rowDic = new Dictionary<string, string>();
        try
        {
            while (result.Read())
            {
                //	定义行数据字典
                
                //	使用循环获取一行中所有的键和数据
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    //	获取键
                    string k = reader.GetName(i);
                    //	获取对应的数据
                    string v = reader.GetValue(i).ToString();
                    //	将键值对放入行数据字典
                    rowDic.Add(k, v);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        bool hasId = false;
        if (rowDic.Count > 0)
        {
            System.Reflection.PropertyInfo propertyInfo = pItem.GetType().GetProperty("id"); //获取指定名称的属性
            propertyInfo.SetValue(pItem, int.Parse(rowDic["id"]), null); //给对应属性赋值

            //pItem.id = int.Parse(rowDic["id"]);
            hasId = true;
        }
        Replace<T>(pItem, hasId);
    }

    public void Replace<T>(object obj ,bool hasId = true)
    { 
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        string sqlPrefix = "replace into " + tableName + " (";
        string sqlMiddle1 = "";
        string sqlMiddle2 = " )values (";
        string sqlSuffix = ");";

        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
        {
            string name = descriptor.Name;
            if (!hasId && name == "id") continue;//id为自增列
            object value = descriptor.GetValue(obj);
            sqlMiddle1 += "" + SQLiteTools.sqliteEscape(name) + ",";
            sqlMiddle2 += "'" + SQLiteTools.sqliteEscape(value) + "',";
        }

        string sql = (sqlPrefix + sqlMiddle1.Substring(0, sqlMiddle1.Length - 1) +
            sqlMiddle2.Substring(0, sqlMiddle2.Length - 1) + sqlSuffix);

        Debug.Log(sql);
        //执行插入操作
        ExecuteQuery(sql);
        //完毕
    }

    //分类
    public List<Dictionary<string, string>> GroupBy<T> (string groupName)
    {
        //获取插入的表名
        string tableName = SQLiteTools.GetTableName(typeof(T));
        //生成查询语句
        string sql = "select * from " + tableName
            + " group by " + groupName + " order by id;";

        Debug.Log(sql);

        var result = ExecuteQuery(sql);

        //	定义结果数据列表
        List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();
        try
        {
            while (result.Read())
            {
                //	定义行数据字典
                Dictionary<string, string> rowDic = new Dictionary<string, string>();
                //	使用循环获取一行中所有的键和数据
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    //	获取键
                    string name = reader.GetName(i);
                    //	获取对应的数据
                    string value = reader.GetValue(i).ToString();
                    //	将键值对放入行数据字典
                    rowDic.Add(name, value);
                }
                //	将行数据字典放入结果数据列表
                dataList.Add(rowDic);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        //	返回结果数据列表
        return dataList;
    }

    #endregion

    string CS2DB(Type type)
    {
        string result = "Text";
        if (type == typeof(Int32))
        {
            result = "Int";
        }
        else if (type == typeof(String))
        {
            result = "Text";
        }
        else if (type == typeof(Single))
        {
            result = "FLOAT";
        }
        else if (type == typeof(Boolean))
        {
            result = "Bool";
        }
        return result;
    }



    /// <summary>
    /// 创建数据库
    /// </summary>
    /// <param name="tableName">数据库名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    /// <returns></returns>
    public SqliteDataReader CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string queryString = "create table if not exists" + tableName + "(" + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += " )";

        Debug.Log("添加成功");
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public SqliteDataReader InsertValues(string tableName, string[] values)
    {
        string sql = "INSERT INTO " + tableName + " values (";
        foreach (var item in values)
        {
            sql += "'" + item + "',";
        }
        sql = sql.TrimEnd(',') + ")";

        Debug.Log("插入成功");
        return ExecuteQuery(sql);
    }

    /// <summary>
    /// 插入数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    //public SqliteDataReader Insert<T>(T t)
    //{
    //    var type = typeof(T);
    //    var fields = type.GetFields();
    //    string sql = "INSERT INTO " + type.Name + " values (";

    //    foreach (var field in fields)
    //    {
    //        //通过反射得到对象的值
    //        sql += "'" + type.GetField(field.Name).GetValue(t) + "',";
    //    }
    //    sql = sql.TrimEnd(',') + ");";

    //    Debug.Log("插入成功");
    //    return ExecuteQuery(sql);
    //}


    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="values">需要修改的数据</param>
    /// <param name="conditions">修改的条件</param>
    /// <returns></returns>
    public SqliteDataReader UpdataData(string tableName, string[] values, string[] conditions)
    {
        string sql = "update " + tableName + " set ";
        for (int i = 0; i < values.Length - 1; i += 2)
        {
            sql += values[i] + "='" + values[i + 1] + "',";
        }
        sql = sql.TrimEnd(',') + " where (";
        for (int i = 0; i < conditions.Length - 1; i += 2)
        {
            sql += conditions[i] + "='" + conditions[i + 1] + "' and ";
        }
        sql = sql.Substring(0, sql.Length - 4) + ");";
        Debug.Log("更新成功");
        return ExecuteQuery(sql);
    }


    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="conditions">查询条件</param>
    /// <returns></returns>
    public SqliteDataReader DeleteValues(string tableName, string[] conditions)
    {
        string sql = "delete from " + tableName + " where (";
        for (int i = 0; i < conditions.Length - 1; i += 2)
        {
            sql += conditions[i] + "='" + conditions[i + 1] + "' and ";
        }
        sql = sql.Substring(0, sql.Length - 4) + ");";
        return ExecuteQuery(sql);
    }

    //打开数据库
    public void OpenConnect()
    {
        try
        {
            //数据库存放在 Asset/StreamingAssets
            string path = Application.streamingAssetsPath + "/" + dbName + ".sqlite";
           
            //新建数据库连接
            connection = new SqliteConnection(@"Data Source = " + path);
            //打开数据库
            connection.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    //关闭数据库
    public void CloseDB()
    {
        if (command != null)
        {
            command.Cancel();
        }
        command = null;

        if (reader != null)
        {
            reader.Close();
        }
        reader = null;

        if (connection != null)
        {
            //connection.Close();
        }
        connection = null;

        Debug.Log("关闭数据库");
    }

}
