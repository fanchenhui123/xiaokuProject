using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SQLiteTools
{
    public static string GetTableName(Type item)
    {
        Type att_type = typeof(SQLTableAttribute);

        Attribute a = Attribute.GetCustomAttribute(item, att_type);
        if (a == null)
            return null;
        SQLTableAttribute sa = (SQLTableAttribute)a;
        return sa.Name;
    }

    public static string GetFieldName(PropertyInfo item)
    {
        Type att_type = typeof(SQLFieldAttribute);
        Attribute a = Attribute.GetCustomAttribute(item, att_type);
        if (a == null)
            return null;
        SQLFieldAttribute sfa = (SQLFieldAttribute)a;
        return sfa.Name;
    }

    public static string GetFieldType(PropertyInfo item)
    {
        Type att_type = typeof(SQLFieldAttribute);

        Attribute a = Attribute.GetCustomAttribute(item, att_type);
        if (a == null)
            return null;
        SQLFieldAttribute sfa = (SQLFieldAttribute)a;
        return sfa.Type;
    }
    
    public static string GetFieldTitle(PropertyInfo item)
    {
        Type att_type = typeof(SQLFieldAttribute);
        Attribute a = Attribute.GetCustomAttribute(item, att_type);
        if (a == null)
            return null;
        SQLFieldAttribute sfa = (SQLFieldAttribute)a;
        return sfa.Title;
    }

    public static string GetFieldString(PropertyInfo item)
    {
        Type att_type = typeof(SQLFieldAttribute);
        Attribute a = Attribute.GetCustomAttribute(item, att_type);

        if (a == null)
        {
            return null;
        }

        SQLFieldAttribute sfa = (SQLFieldAttribute)a;

        string sql = "";
        sql += sfa.Name + " ";
        sql += sfa.Type + " ";

        if (sfa.IsPrimaryKey)
        {
            sql += "primary key" + " ";
        }
        if (sfa.AutoIncrement)
        {
            sql += "autoincrement" + " ";
        }
        if (sfa.IsNotNull)
        {
            sql += "not null" + " ";
        }
        if (sfa.IsUnique)
        {
            sql += "unique" + " ";
        }
        if (sfa.Default != null)
        {
            sql += "default " + sfa.Default;
        }

        return sql;
    }

    public static string sqliteEscape(object obj)
    {
        if (obj == null)
        {
            return "";
        }
        else
        {
            string keyWord = obj.ToString();
            //keyWord = keyWord.Replace("/", "//");
            keyWord = keyWord.Replace("'", "''");
            keyWord = keyWord.Replace("[", "/[");
            keyWord = keyWord.Replace("]", "/]");
            keyWord = keyWord.Replace("%", "/%");
            keyWord = keyWord.Replace("&", "/&");
            //keyWord = keyWord.Replace("_", "/_");
            keyWord = keyWord.Replace("(", "/(");
            keyWord = keyWord.Replace(")", "/)");
            return keyWord;
        }
    }
}
