using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Dynamo.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using Microsoft.Practices.Prism.Commands;

namespace SpringsUI
{
	[NodeCategory("Core.Input")]
	[NodeDescription("Returns a bool value and/or re-runs the model.")]
	[NodeName("SpringsUI.Refresh")]
	[OutPortDescriptions("A true/false bool value")]
	[OutPortNames(new []{"bool"})]
	[OutPortTypes(new []{"bool"})]
	[IsDesignScriptCompatible]
	public class RefreshButton : NodeModel
	{
		internal bool _val;
		internal bool _run;
		internal bool _force;
		
		[IsVisibleInDynamoLibrary(false)]
		public DelegateCommand UpdateRefreshButton { get; set; }
		
		[IsVisibleInDynamoLibrary(false)]
		public bool val
		{
			get
			{
				return _val;
			}
			set
			{
				_val = value;
				RaisePropertyChanged("val");
				OnNodeModified();
			}
		}
		
		[IsVisibleInDynamoLibrary(false)]
		public bool run
		{
			get
			{
				return _run;
			}
			set
			{
				_run = value;
				RaisePropertyChanged("run");
				OnNodeModified();
			}
		}
		
		[IsVisibleInDynamoLibrary(false)]
		public bool force
		{
			get
			{
				return _force;
			}
			set
			{
				_force = value;
				RaisePropertyChanged("force");
				OnNodeModified();
			}
		}
		
		[IsVisibleInDynamoLibrary(false)]
		public RefreshButton()
		{
			run = false;
			force = false;
			val = false;
			UpdateRefreshButton = new DelegateCommand(Flip);
			RegisterAllPorts();
		}

		internal void Flip()
		{
			val = !val;
			var link = dvmLink.Instance();
			if (link != null)
			{
				if (run)
				{
					if(force)
					{
						foreach (var node in link.HomeSpace.Nodes)
						{
							node.MarkNodeAsModified(true);
						}
					}
					link.HomeSpace.Run();
				}
			}
		}
		
		[IsVisibleInDynamoLibrary(false)]
		public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
		{
			return new AssociativeNode[]
			{
				AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildBooleanNode(val) )
			};
		}
	}
	
	public class RefreshButtonViewCustomization : INodeViewCustomization<RefreshButton>
	{
        public void CustomizeView(RefreshButton model, NodeView nodeView)
        {
        	dvmLink.Instance(nodeView.ViewModel.DynamoViewModel);
        	var control = new RefreshButtonView();
        	nodeView.inputGrid.Children.Add(control);
        	control.DataContext = model;
        }
        
        public void Dispose()
        {
        	
        }
	}
}