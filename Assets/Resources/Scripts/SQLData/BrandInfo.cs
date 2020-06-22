using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SQLTable(Name ="brand_info")]
public class BrandInfo
{
	[SQLField(Name = "id", Type = "integer", IsPrimaryKey = true)]
	public int id { get; set; }
    [SQLField(Name = "title", Type = "text")]
    public string title { get; set; }
	[SQLField(Name = "logo", Type = "text")]
	public string logo { get; set; }	
}

[SQLTable(Name = "cartLines_info")]
public class CartLinesInfo:BrandInfo
{
    [SQLField(Name = "brand_id", Type = "integer")]
    public int brand_id { get; set; }
}

[SQLTable(Name = "cartModels_info")]
public class CartModelsInfo : BrandInfo
{
    [SQLField(Name = "cart_line_id", Type = "integer")]
    public int cart_line_id { get; set; }
    [SQLField(Name = "price", Type = "text")]
    public string price { get; set; }
    [SQLField(Name = "appear_color", Type = "text")]
    public string appear_color { get; set; }
}