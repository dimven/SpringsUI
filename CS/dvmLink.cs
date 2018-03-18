using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.ViewModels;

namespace SpringsUI
{
	[SupressImportIntoVM]
	internal class dvmLink
	{
		private static dvmLink instance;
		private static bool isSet = false;
		private DynamoViewModel _viewmodel { get; set; }

		private dvmLink(DynamoViewModel dvm)
		{
			_viewmodel = dvm;
		}
		
		internal static DynamoViewModel Instance(DynamoViewModel dvm=null)
		{
			if (isSet)
			{
				return instance._viewmodel;
			}
			
			if (dvm != null)
			{
				instance = new dvmLink(dvm);
				isSet = true;
				return instance._viewmodel;
			}
			return null;
		}
	}
}
