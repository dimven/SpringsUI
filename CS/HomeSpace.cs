using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Dynamo.ViewModels;
using ProtoCore.AST.AssociativeAST;
using SpringsUIzt;
using Dynamo.Controls;
using Dynamo.Wpf;
using Microsoft.Practices.Prism.Commands;


namespace SpringsUI
{
	[NodeCategory("Core.Info")]
	[NodeDescription("Returns the file path of the active graph.")]
	[NodeName("SpringsUI.Graph.FilePath")]
	[InPortDescriptions(new []{"an optional bool value to refresh the node"})]
	[InPortNames(new []{"refresh"})]
	[InPortTypes(new []{"bool"})]
	[OutPortDescriptions("a file path")]
	[OutPortNames(new []{"path"})]
	[OutPortTypes(new []{"string"})]
	[IsDesignScriptCompatible]
	public class HomeSpacePath : NodeModel
	{
		[IsVisibleInDynamoLibrary(false)]
		public HomeSpacePath()
		{
			RegisterAllPorts();
			InPorts[0].SetPortData(new PortData(InPorts[0].PortName,
			                                    InPorts[0].ToolTipContent, 
			                                    AstFactory.BuildBooleanNode(true) ) );
			
		}

		
		[IsVisibleInDynamoLibrary(false)]
		public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
		{
			return new AssociativeNode[]
			{
				AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
				                           AstFactory.BuildFunctionCall(new Func<bool,string>(HomeSpaceZT.GraphPath), inputAstNodes) )
			};
		}
	}
	
	public class RegisterHomeSpacePath : INodeViewCustomization<HomeSpacePath>
	{
        public void CustomizeView(HomeSpacePath _, NodeView nodeView)
        {
        	dvmLink.Instance(nodeView.ViewModel.DynamoViewModel);
        }
        
        public void Dispose()
        {
        	
        }
	}
	
	
	[NodeCategory("Core.Info")]
	[NodeDescription("Returns the categories of all placed nodes")]
	[NodeName("SpringsUI.Graph.PlacedNodesCategories")]
	[InPortDescriptions(new []{"an optional bool value to refresh the node"})]
	[InPortNames(new []{"refresh"})]
	[InPortTypes(new []{"bool"})]
	[OutPortDescriptions(new []{"a list of categories"})]
	[OutPortNames(new []{"str[]"})]
	[OutPortTypes(new []{"string[]"})]
	[IsDesignScriptCompatible]
	public class HomeSpaceNodes : NodeModel
	{
		[IsVisibleInDynamoLibrary(false)]
		public HomeSpaceNodes()
		{
			RegisterAllPorts();
			InPorts[0].SetPortData(new PortData(InPorts[0].PortName,
			                                    InPorts[0].ToolTipContent,
			                                    AstFactory.BuildBooleanNode(true) ) );
		}

		
		[IsVisibleInDynamoLibrary(false)]
		public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
		{
			return new AssociativeNode[]
			{
				AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
				                           AstFactory.BuildFunctionCall(new Func<bool, List<string> >(HomeSpaceZT.GraphCategories),
				                           inputAstNodes) )
			};
		}
	}
	
	public class RegisterHomeSpaceNodes : INodeViewCustomization<HomeSpaceNodes>
	{
		public void CustomizeView(HomeSpaceNodes _, NodeView nodeView)
		{
			dvmLink.Instance(nodeView.ViewModel.DynamoViewModel);
		}
		
		public void Dispose()
		{
			
		}
	}
}