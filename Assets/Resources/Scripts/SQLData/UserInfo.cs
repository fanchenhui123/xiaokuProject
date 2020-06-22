using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SQLTable(Name ="user_info")]
public class UserInfo
{
	[SQLField(Name = "id", Type = "integer", AutoIncrement = true, IsNotNull = true, IsPrimaryKey = true)]
	public int id { get; set; }
    [SQLField(Name = "phone", Type = "text")]
    public string phone { get; set; }
	[SQLField(Name = "psd", Type = "text")]
	public string psd { get; set; }
	[SQLField(Name = "name", Type = "text")]
	public string name { get; set; }
	[SQLField(Name = "nickname", Type = "text")]
	public string nickname { get; set; }
	[SQLField(Name = "relation", Type = "text")]
	public string relation { get; set; }
	[SQLField(Name = "username", Type = "text")]
	public string username { get; set; }
	[SQLField(Name = "ip", Type = "text")]
	public string ip { get; set; }
	[SQLField(Name = "salt", Type = "text")]
	public string salt { get; set; }
    [SQLField(Name = "remember",Type ="integer")]
    public int remember { get; set; }
}
