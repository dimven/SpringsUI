using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using System.Xml;
using ProtoCore.AST.AssociativeAST;
using SpringsUIzt;
using Dynamo.Controls;
using Dynamo.Wpf;
using System.Windows.Controls;
using Dynamo.Graph;

using Dynamo.ViewModels;
using System.Windows;
using System.Windows.Data;
using Dynamo.UI.Prompts;
using Binding = System.Windows.Data.Binding;

namespace SpringsUI
{
    [NodeCategory("Core.Input")]
    [NodeDescription(@"You'll need to set up your decode secret prior to using
this input, otherwise a default decode value of 'Dynamo'
will be used instead. To do that, right-click on the
node and select 'Decoding Secret...'. The decode secret
is only saved for the active session. The encoded
password value however will automatically be
serialized for future use.")]
    [NodeName("SpringsUI.Input.Password")]
    [OutPortDescriptions("a password input instance")]
    [OutPortNames(new []{"password"})]
    [OutPortTypes(new []{"Object"})]
    [IsDesignScriptCompatible]
    public class PassInput : NodeModel
    {
        private string _pp;
        private string _secret;
        
        public PassInput()
        {
            _pp = "";
            _secret = "Dynamo";
            ShouldDisplayPreviewCore = false;
            RegisterAllPorts();
        }
        
        [IsVisibleInDynamoLibrary(false)]
        public string Secret
        {
            get
            {
                return "";
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _secret = value;
                    RaisePropertyChanged("Secret");
                    OnNodeModified();
                }
            }
        }
        
        
        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            inputAstNodes.Add(AstFactory.BuildStringNode(_pp));
            
            return new AssociativeNode[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),
                                           AstFactory.BuildFunctionCall(new Func<string, passwordPrimitive>(passwordPrimitive.getPrimitive),  inputAstNodes))
            };
        }
        
        [IsVisibleInDynamoLibrary(false)]
        public override bool IsConvertible
        {
            get { return true; }
        }
        
        internal void Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            var pb = (PasswordBox)sender;
            _pp = StringCipher.Encrypt(pb.Password, _secret);
            OnNodeModified();
        }
        
        #region Serialization/Deserialization Methods
        
        protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        {
            if (updateValueParams.PropertyName == "_pp")
            {
                _pp = updateValueParams.PropertyValue;
                return true;
            }

            return base.UpdateValueCore(updateValueParams);
        }
        
        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context); // Base implementation must be called

            var xmlDocument = element.OwnerDocument;
            var subNode = xmlDocument.CreateElement(typeof(string).FullName);
            subNode.InnerText = _pp;
            element.AppendChild(subNode);
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context); // Base implementation must be called

            foreach (XmlNode subNode in nodeElement.ChildNodes.Cast<XmlNode>()
                     .Where(subNode => subNode.Name.Equals(typeof(string).FullName)))
            {
                var attrs = subNode.Attributes;

                _pp = attrs != null && attrs["value"] != null
                    ? attrs["value"].Value //Legacy behavior
                    : subNode.InnerText;
            }
        }

        #endregion
    }
    

    public class PassInputViewCustomization : INodeViewCustomization<PassInput>
    {
        private PassInputView _control;
        private MenuItem _editWindowItem;
        private DynamoViewModel _dynamoViewModel;
        private PassInput _nodeModel;
        
        public void CustomizeView(PassInput model, NodeView nodeView)
        {
            _nodeModel = model;
            _dynamoViewModel = nodeView.ViewModel.DynamoViewModel;
            _editWindowItem = new MenuItem
            {
                Header = "Decoding Secret...",
                IsCheckable = false
            };
            nodeView.MainContextMenu.Items.Add(_editWindowItem);
            _editWindowItem.Click += editWindowItem_Click;
            
            _control = new PassInputView();
            _control.passBox.PasswordChanged += model.Changed;
            nodeView.inputGrid.Children.Add(_control);
            _control.DataContext = model;
        }
        
        public void editWindowItem_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditWindow(_dynamoViewModel){ DataContext = _nodeModel };
            editWindow.BindToProperty(
                null,
                new Binding("Secret")
                {
                    Mode = BindingMode.OneWayToSource,
                    Converter = new StringDisplay(),
                    NotifyOnValidationError = false,
                    Source = _nodeModel,
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                });

            editWindow.ShowDialog();
        }
        
        public void Dispose()
        {
            _control.passBox.PasswordChanged -= _nodeModel.Changed;
            _editWindowItem.Click -= editWindowItem_Click;
        }
    }
}
