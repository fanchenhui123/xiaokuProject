using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property)]
public class SQLFieldAttribute : Attribute
{
    public string Name { set; get; }
    public string Type { set; get; }
    public bool IsNotNull { set; get; }
    public bool AutoIncrement { set; get; }
    public bool IsPrimaryKey { set; get; }
    public string Default { set; get; }
    public bool IsUnique { set; get; }
    public string Title { set; get; }
}
