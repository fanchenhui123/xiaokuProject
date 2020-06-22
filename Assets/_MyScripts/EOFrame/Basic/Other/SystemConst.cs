using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemConst  {

	//view在resources文件夹下的路径
	public  const string viewResPath="Views/";
	public  const string moduleResPath="Modules/";
    
	public static bool isDebug2File = true;
    internal static bool canCreateVirtualObj=false;
	
}
public delegate void VoidFunc();
public delegate void Param1Func(View view);
