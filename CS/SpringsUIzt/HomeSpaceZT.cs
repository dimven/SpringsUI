using System;
using System.Reflection;
using System.Collections.Generic;
using Dynamo.ViewModels;
using Autodesk.DesignScript.Runtime;

namespace SpringsUIzt
{
	/// <summary>
	///  
	/// </summary>
	[IsVisibleInDynamoLibrary(false)]
	public static class HomeSpaceZT
	{
		internal static object GetDvmInstanceMethod()
		{
			
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies() )
			{
				if (a.GetName().Name == "SpringsUI")
				{
					foreach (var t in a.GetTypes() )
					{
						if (t.Name == "dvmLink")
						{
							foreach(var p in t.GetRuntimeMethods() )
							{
								if (p.Name == "Instance")
								{
									return p;
								}
							}
						}
					}
				}
			}
			return null;
		}
		
		internal static DynamoViewModel GetDvmInstance()
		{
			var instance_method = (MethodInfo) GetDvmInstanceMethod();
			return instance_method != null ? (DynamoViewModel)instance_method.Invoke(null, new Object[]{null}) : null;
		}
		
		[IsVisibleInDynamoLibrary(false)]
		public static string GraphPath(bool _)
		{
			var link = GetDvmInstance();
			return link != null ? link.HomeSpace.FileName : "";
		}
		
		
		[IsVisibleInDynamoLibrary(false)]
		public static List<string> GraphCategories(bool _)
		{
			var cats = new HashSet<string>();
			var link = GetDvmInstance();
			if (link != null)
			{
				foreach (var node in link.HomeSpace.Nodes)
				{
					var c1 = node.Category;
					if (!string.IsNullOrEmpty(c1) )
					{
						cats.Add(c1);
					}
				}
			}
			var sorted_cats = new List<string>(cats);
			sorted_cats.Sort();
			return sorted_cats;
		}
	}
}