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
	[NodeCategory("Core.Execution")]
	[NodeDescription("Force stops the graph execution after a period of time")]
	[NodeName("SpringsUI.CancelRun")]
	[OutPortDescriptions("returns 1 if the timer intervened or 0 if the graph executed sucessfully before that")]
	[OutPortNames(new []{"result"})]
	[OutPortTypes(new []{"int"})]
	[IsDesignScriptCompatible]
	public class CancelRunButton : NodeModel
	{
		private int result;
		private double _seconds;
		
		public string seconds
		{
			get
			{
				return string.Format("{0:0.00}", _seconds);
			}
			set
			{
				bool valid = double.TryParse(value, out _seconds);
				if (valid)
				{
					RaisePropertyChanged("seconds");
					OnNodeModified();
				}
				else
				{
					_seconds = 10d;
				}
			}
		}
		[IsVisibleInDynamoLibrary(false)]
		public DelegateCommand ExecuteCancelRunCommand { get; set; }
		
		[IsVisibleInDynamoLibrary(false)]
		public CancelRunButton()
		{
			result = -1;
			_seconds = 10d;
			ExecuteCancelRunCommand = new DelegateCommand(ExecuteCancelRun);
			RegisterAllPorts();
		}
		
		internal void ExecuteCancelRun()
		{
			var link = dvmLink.Instance();
			if (link != null)
			{
				var rs = link.HomeSpace.RunSettings;
				if (rs.RunType.ToString() == "Manual" && !link.HomeSpace.RunSettings.RunEnabled)
				{
					var new_cancel = new Dynamo.Models.DynamoModel.ForceRunCancelCommand(false, false); //this can't be assigned anywhere
					link.HomeSpaceViewModel.RunSettingsViewModel.CancelRunCommand.Execute(false); //only notifies the UI of a calcel
					link.EngineController.LiveRunnerRuntimeCore.ExecutionState = (int)ProtoCore.ExecutionStateEventArgs.State.ExecutionBreak; //doesn't do anything
					result = 1;
				}
			}
		}

		[IsVisibleInDynamoLibrary(false)]
		public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
		{
			return new AssociativeNode[]
			{
				AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
				                           AstFactory.BuildIntNode(result) )
//					                           AstFactory.BuildFunctionCall(new Func<double,int>(HomeSpaceZT.KillGraph), parameters) )
			};
		}
	}
	
	public class RegisterHomeSpaceTerminate : INodeViewCustomization<CancelRunButton>
	{
		public void CustomizeView(CancelRunButton model, NodeView nodeView)
		{
			dvmLink.Instance(nodeView.ViewModel.DynamoViewModel);
			var control = new CancelRunButtonView();
        	nodeView.inputGrid.Children.Add(control);
        	control.DataContext = model;
		}
		
		public void Dispose()
		{
			
		}
	}
}
