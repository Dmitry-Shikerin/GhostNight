using DungeonArchitect.Editors.MarkerGenerator.UI.NodeRenderers;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Nodes.Actions;
using DungeonArchitect.MarkerGenerator.Nodes.Condition;
using DungeonArchitect.MarkerGenerator.Pins;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.UI
{
    public class MarkerGenRuleGraphEditor : GraphEditor
    {
        public bool drawInvalidGraphMessage = false;
        
        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);
            EditorStyle.branding = "Rule Graph";
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new MarkerGenRuleGraphContextMenu(this);
        }
        
        public override GraphSchema GetGraphSchema()
        {
            return new MarkerGenRuleGraphSchema();
        }
        
        protected override void InitializeNodeRenderers(GraphNodeRendererFactory renderers)
        {
            renderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
            renderers.RegisterNodeRenderer(typeof(MarkerGenRuleGraphNode), new MarkerGenRuleNodeRenderer());
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new SplineGraphLinkRenderer();
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = userdata as MarkerGenRuleGraphContextMenuUserData;
            var mouseScreen = lastMousePosition;
            GraphNode node = null;
            if (action != null)
            {
                if (action.Action == MarkerGenRuleGraphEditorAction.CreateCommentNode)
                {
                    node = CreateNode<CommentNode>(mouseScreen, e.uiSystem);
                }
                else if (action.Action == MarkerGenRuleGraphEditorAction.CreateRuleNode)
                {
                    node = CreateNode(mouseScreen, action.NodeType, e.uiSystem);
                }

                if (node != null)
                {
                    SelectNode(node, e.uiSystem);
                }
            }

            if (node != null)
            {
                // Check if the menu was created by dragging out a link
                if (e.sourcePin != null)
                {
                    GraphPin targetPin = (e.sourcePin.PinType == GraphPinType.Input) 
                            ? node.OutputPin 
                            : node.InputPin;

                    if (targetPin != null)
                    {

                        // Align the target pin with the mouse position where the link was dragged and released
                        node.Position = e.mouseWorldPosition - targetPin.Position;

                        GraphPin inputPin, outputPin;
                        if (e.sourcePin.PinType == GraphPinType.Input)
                        {
                            inputPin = e.sourcePin;
                            outputPin = targetPin;
                        }
                        else
                        {
                            inputPin = targetPin;
                            outputPin = e.sourcePin;
                        }

                        CreateLinkBetweenPins(outputPin, inputPin, e.uiSystem);
                    }
                    
                }
            }
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Select a rule block to edit";
        }


        protected override void DrawHUD(UISystem uiSystem, UIRenderer renderer, Rect bounds)
        {
            base.DrawHUD(uiSystem, renderer, bounds);

            if (drawInvalidGraphMessage)
            {
                var offset = new Vector2(10, 30);
                string errorMessage = "Warning: Compilation failed. Invalid graph";
                var style = new GUIStyle(GUI.skin.GetStyle("label"));
                style.normal.textColor = Color.red;
                
                var textSize = style.CalcSize(new GUIContent(errorMessage));
                
                var x = bounds.x + offset.x;
                var y = bounds.yMax - textSize.y - offset.y;
                var textBounds = new Rect(new Vector2(x, y), textSize);
                renderer.DrawRect(MathUtils.ExpandRect(textBounds, 4), new Color(0, 0, 0, 0.5f));
                renderer.Label(textBounds, errorMessage, style);
            }
        }
    }
    
    public class MarkerGenRuleGraphSchema : GraphSchema
    {
        public override bool CanCreateLink(GraphPin output, GraphPin input, out string errorMessage)
        {
            errorMessage = "";
            if (input == output)
            {
                return false;
            }
            
            if (output == null || input == null)
            {
                errorMessage = "Invalid connection";
                return false;
            }

            if (input.PinType == output.PinType)
            {
                errorMessage = "Not Allowed";
                return false;
            }

            if (input.GetType() != output.GetType())
            {
                errorMessage = "Not Allowed";
                return false;
            }

            var graph = output.Node.Graph;
            foreach (var link in graph.Links)
            {
                if (link.Input == input && link.Output == output)
                {
                    errorMessage = "Not Allowed: Already connected";
                    return false;
                }
            }

            return true;
        }

        public override T TryCreateLink<T>(Graph graph, GraphPin output, GraphPin input)
        {
            bool isConditionalNode = output is MarkerGenRuleGraphPinBool;
            bool isExecNode = output is MarkerGenRuleGraphPinExec;
            
            if (isConditionalNode)
            {
                // Only one input, multiple outputs
                GraphOperations.DestroyPinLinks(graph, input, null);
            }
            else if (isExecNode)
            {
                // multiple inputs, one output
                GraphOperations.DestroyPinLinks(graph, output, null);
            }
            
            return base.TryCreateLink<T>(graph, output, input);
        }
    }

    public enum MarkerGenRuleGraphEditorAction
    {
        CreateCommentNode,
        CreateRuleNode
    }
    
    class MarkerGenRuleGraphContextMenuUserData
    {
        public MarkerGenRuleGraphContextMenuUserData(UISystem uiSystem, MarkerGenRuleGraphEditorAction action)
            : this(uiSystem, action, null)
        {
        }

        public MarkerGenRuleGraphContextMenuUserData(UISystem uiSystem, MarkerGenRuleGraphEditorAction action, System.Type nodeType)
        {
            this.uiSystem = uiSystem;
            this.Action = action;
            this.NodeType = nodeType;
        }

        public MarkerGenRuleGraphEditorAction Action { get; set; }
        public System.Type NodeType { get; set; }
        public UISystem uiSystem { get; set; }
    }

    
    class MarkerGenRuleGraphContextMenu : GraphContextMenu
    {
        private MarkerGenRuleGraphEditor host;
        
        struct MenuItemInfo
        {
            public MenuItemInfo(string title, float weight, System.Type handlerType)
            {
                this.title = title;
                this.weight = weight;
                this.handlerType = handlerType;
            }

            public string title;
            public float weight;
            public System.Type handlerType;
        }

        public MarkerGenRuleGraphContextMenu(MarkerGenRuleGraphEditor host)
        {
            this.host = host;
        }

        void AddMenuItem<T>(string name, string subMenu, IContextMenu menu, UISystem uiSystem)
        {
            menu.AddItem(subMenu + name, HandleContextMenu, new MarkerGenRuleGraphContextMenuUserData(uiSystem, MarkerGenRuleGraphEditorAction.CreateRuleNode, typeof(T)));
        }

        void PopulateConditionItems(IContextMenu menu, UISystem uiSystem, string subMenu = "")
        {
            AddMenuItem<MarkerGenRuleNodeMarkerExists>("Marker Exists", subMenu, menu, uiSystem);
            AddMenuItem<MarkerGenRuleNodeConditionScript>("Script Node", subMenu, menu, uiSystem);
            AddMenuItem<MarkerGenRuleNodeAnd>("And", subMenu, menu, uiSystem);
            AddMenuItem<MarkerGenRuleNodeOr>("Or", subMenu, menu, uiSystem);
            AddMenuItem<MarkerGenRuleNodeNot>("Not", subMenu, menu, uiSystem);
        }
        
        void PopulateActionItems(IContextMenu menu, UISystem uiSystem, string subMenu = "")
        {
            AddMenuItem<MarkerGenRuleNodeAddMarker>("Add Marker", subMenu, menu, uiSystem);
            AddMenuItem<MarkerGenRuleNodeRemoveMarker>("Remove Marker", subMenu, menu, uiSystem);
        }

        void PopulateDebugItems(IContextMenu menu, UISystem uiSystem)
        {
            menu.AddItem("Debug/Result Node", HandleContextMenu, new MarkerGenRuleGraphContextMenuUserData(uiSystem, MarkerGenRuleGraphEditorAction.CreateRuleNode, typeof(MarkerGenRuleNodeResult)));
            menu.AddItem("Debug/OnSelected Node", HandleContextMenu, new MarkerGenRuleGraphContextMenuUserData(uiSystem, MarkerGenRuleGraphEditorAction.CreateRuleNode, typeof(MarkerGenRuleNodeOnPass)));
        }

        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            this.sourcePin = sourcePin;
            this.mouseWorldPosition = mouseWorld;
            
            var menu = uiSystem.Platform.CreateContextMenu();

            if (sourcePin != null && sourcePin is MarkerGenRuleGraphPinBool)
            {
                PopulateConditionItems(menu, uiSystem);
            }
            else if (sourcePin != null && sourcePin is MarkerGenRuleGraphPinExec)
            {
                PopulateActionItems(menu, uiSystem);
            }
            else 
            {
                PopulateConditionItems(menu, uiSystem, "Condition/");
                PopulateActionItems(menu, uiSystem, "Actions/");
                menu.AddSeparator("");
                menu.AddItem("Add Comment Node", HandleContextMenu, new MarkerGenRuleGraphContextMenuUserData(uiSystem, MarkerGenRuleGraphEditorAction.CreateCommentNode));
            }

            menu.Show();
        }

        void HandleContextMenu(object action)
        {
            var item = action as MarkerGenRuleGraphContextMenuUserData;
            if (item != null)
            {
                DispatchMenuItemEvent(action, BuildEvent(null, item.uiSystem));
            }
        }
    }
}