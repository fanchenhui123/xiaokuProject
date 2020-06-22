using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SQLTable(Name ="bussiness_info")]
public class BussinessInfo
{
    /*
     * 商户id、商户昵称、商户名称、商户手机、商户信息
     */

	[SQLField(Name = "id", Type = "integer", AutoIncrement = true, IsNotNull = true, IsPrimaryKey = true)]
	public int id { get; set; }
    [SQLField(Name ="bussinessId",Type ="text")]
    public int bussinessId { get; set; }
    [SQLField(Name ="nickname",Type ="text")]
    public string nickname { get; set; }

    [SQLField(Name = "name", Type = "text")]
    public string name { get; set; }

    [SQLField(Name = "phone", Type = "text")]
    public string phone { get; set; }

    [SQLField(Name = "info", Type = "text")]
    public string info { get; set; }

}
