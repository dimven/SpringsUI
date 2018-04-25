using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.ViewModels;

namespace SpringsUI
{
	[SupressImportIntoVM]
	internal class dvmLink
	{
		private static dvmLink instance = null;
		private DynamoViewModel _viewmodel { get; set; }

		private dvmLink(DynamoViewModel dvm)
		{
			_viewmodel = dvm;
		}
		
		internal static DynamoViewModel Instance(DynamoViewModel dvm=null)
		{
			if (dvm != null)
			{
				instance = new dvmLink(dvm);
				return instance._viewmodel;
			}
			return instance._viewmodel;
		}
	}
}
