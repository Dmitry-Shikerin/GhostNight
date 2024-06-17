using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Pins;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.MarkerGenerator.UI.NodeRenderers
{
    public class MarkerGenRuleNodeRenderer : GraphNodeRenderer
    {
        private GUIStyle styleBody;
        private GUIStyle styleSelectBorder;
        private GUIStyle styleTitle;
        private GUIStyle styleToggle;
        private Texture2D pinTexDefault;
        private Texture2D pinTexDefaultHollow;
        private Texture2D pinTexExec;
        private Texture2D pinTexExecHollow;

        bool IsStateInvalid()
        {
            return styleBody == null || styleTitle == null;
        }

        void InitializeState(UIRenderer renderer)
        {
            GUISkin skin = renderer.GetResource<GUISkin>("skins/graph_editor/GuiSkinGraphNode") as GUISkin;
            if (skin == null)
            {
                skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            }

            if (skin != null) 
            {
                styleBody = new GUIStyle(skin.box);
                styleBody.normal.background = renderer.GetResource<Texture2D>("skins/graph_editor/NodeBorderTex") as Texture2D;
                
                styleTitle = new GUIStyle(skin.box);
                styleTitle.normal.background = renderer.GetResource<Texture2D>("skins/graph_editor/NodeTitleTex") as Texture2D;
                
                styleSelectBorder = new GUIStyle(skin.box);
                styleSelectBorder.normal.background = renderer.GetResource<Texture2D>("skins/graph_editor/NodeBorderSelectTex") as Texture2D;

                styleToggle = new GUIStyle(skin.toggle);
                
                pinTexDefault = renderer.GetResource<Texture2D>("skins/graph_editor/pin_12") as Texture2D;
                pinTexDefaultHollow = renderer.GetResource<Texture2D>("skins/graph_editor/pin_hollow_12") as Texture2D;
                
                pinTexExec = renderer.GetResource<Texture2D>("skins/graph_editor/pin_exec_12") as Texture2D;
                pinTexExecHollow = renderer.GetResource<Texture2D>("skins/graph_editor/pin_exec_hollow_12") as Texture2D;
            }
        }

        private Dictionary<GraphPin, List<GraphPin>> linkedTo = new Dictionary<GraphPin, List<GraphPin>>();

        public override void BeginFrame(Graph graph)
        {
            if (graph == null) return;

            foreach (var link in graph.Links)
            {
                var pinInput = link.Input as MarkerGenRuleGraphPin;
                var pinOutput = link.Output as MarkerGenRuleGraphPin;
                if (pinInput == null || pinOutput == null) continue;
                
                if (!linkedTo.ContainsKey(pinInput))
                {
                    linkedTo.Add(pinInput, new List<GraphPin>());
                }

                if (!linkedTo.ContainsKey(pinOutput))
                {
                    linkedTo.Add(pinOutput, new List<GraphPin>());
                }
                
                linkedTo[pinInput].Add(pinOutput);
                linkedTo[pinOutput].Add(pinInput);
            }
        }

        bool IsPinConnected(GraphPin pin)
        {
            if (pin == null || !linkedTo.ContainsKey(pin))
            {
                return false;
            }

            return linkedTo[pin].Count > 0;
        }

        public override void EndFrame()
        {
            linkedTo.Clear();
        }

        Texture2D GetPinTexture(MarkerGenRuleGraphPin pin)
        {
            bool connected = IsPinConnected(pin);
            if (pin is MarkerGenRuleGraphPinExec)
            {
                return connected ? pinTexExec : pinTexExecHollow;
            }
            return connected ? pinTexDefault : pinTexDefaultHollow;
        }

        private Vector2 pinPadding = new Vector2(4, 4);
        
        Vector2 CalcSize(MarkerGenRuleGraphPin pin, GUIStyle style)
        {
            if (pin == null) return Vector2.zero;
            var pinTex = GetPinTexture(pin);
            var textureSize = new Vector2(pinTex.width, pinTex.height);
            var size = textureSize + pinPadding * 2;
            
            if (pin is MarkerGenRuleGraphPinBool boolPin)
            {
                if (pin.PinType == GraphPinType.Input)
                {
                    var toggleSize = new Vector2(16, 16);
                    size.x += toggleSize.x + pinPadding.x;
                    size.y = Mathf.Max(size.y, toggleSize.y);
                }

                if (boolPin.text.Length > 0)
                {
                    var textSize = style.CalcSize(new GUIContent(boolPin.text));
                    size.x += textSize.x;
                    size.y = Mathf.Max(size.y, textSize.y);
                }
            }

            size.y += pinPadding.y;

            return size;
        }

        private void DrawPin(UIRenderer renderer, MarkerGenRuleGraphPin pin, GUIStyle style, Rect bounds, GraphCamera camera)
        {
            float cameraZoomLevel = camera.ZoomLevel;
            bool isInput = pin.PinType == GraphPinType.Input;
            var pinTex = GetPinTexture(pin);
            var texSize = new Vector2(pinTex.width, pinTex.height) / cameraZoomLevel;

            var centerY = bounds.y + bounds.size.y * 0.5f;
            var currentX = isInput
                ? bounds.x 
                : bounds.xMax - texSize.x;
            
            var pinPos = new Vector2(currentX, centerY - pinTex.height * 0.5f);
            var pinBounds = new Rect(pinPos, texSize);
            renderer.DrawTexture(pinBounds, pinTex, ScaleMode.ScaleToFit, true, pin.GetPinColor());

            // Update the bounds offset
            {
                var hoverBounds = MathUtils.ExpandRect(pinBounds, pinPadding.x);
                var localBounds = camera.ScreenToWorld(hoverBounds);
                localBounds.position -= pin.Node.Position;
                if (isInput)
                {
                    pin.Position = new Vector2(localBounds.x, localBounds.y + localBounds.height * 0.5f);
                    pin.BoundsOffset = new Rect(new Vector2(0, -localBounds.height * 0.5f), localBounds.size);
                    pin.Tangent = new Vector2(-1, 0);
                }
                else
                {
                    pin.Position = new Vector2(localBounds.xMax, localBounds.y + localBounds.height * 0.5f);
                    pin.BoundsOffset = new Rect(new Vector2(-localBounds.width, -localBounds.height * 0.5f), localBounds.size);
                    pin.Tangent = new Vector2(1, 0);
                }
            }

            if (isInput)
            {
                currentX += texSize.x + pinPadding.x / cameraZoomLevel;
            }
            else
            {
                currentX -= pinPadding.x / cameraZoomLevel;
            }

            if (pin is MarkerGenRuleGraphPinBool)
            {
                var boolPin = pin as MarkerGenRuleGraphPinBool;
                
                bool checkboxVisible = isInput && !IsPinConnected(pin);

                if (checkboxVisible)
                {
                    const float checkboxSize = 16;
                    Rect toggleBounds = new Rect();
                    toggleBounds.width = checkboxSize / cameraZoomLevel;
                    toggleBounds.height = checkboxSize / cameraZoomLevel;
                    toggleBounds.x = currentX;
                    toggleBounds.y = centerY - toggleBounds.height * 0.5f;

                    renderer.backgroundColor = Color.white;
                    renderer.color = Color.white;
                    bool oldValue = boolPin.defaultValue; 
                    boolPin.defaultValue = renderer.Toggle(toggleBounds, boolPin.defaultValue, "");
                    if (oldValue != boolPin.defaultValue)
                    {
                        GraphStateChanged = true;
                    }

                    currentX += toggleBounds.width;
                }
            }

            var pinTextSize = style.CalcSize(new GUIContent(pin.text));

            var pinTextBounds = new Rect();
            {
                pinTextBounds.x = currentX;
                pinTextBounds.y = centerY - pinTextSize.y * 0.5f;
                pinTextBounds.width = pinTextSize.x;
                pinTextBounds.height = pinTextSize.y;
            }

            if (!isInput)
            {
                pinTextBounds.x -= pinTextBounds.width;
            }
                
            style.normal.textColor = Color.white;
            renderer.Label(pinTextBounds, pin.text, style);
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            if (IsStateInvalid())
            {
                InitializeState(renderer);
            }
            
            var ruleNode = node as MarkerGenRuleGraphNode;
            if (ruleNode == null)
            {
                return;
            }
            
            var title = ruleNode.Title;
            var titleColor = Color.white;

            var style = CreateStyle(node.Selected, camera.ZoomLevel);

            var guiState = new GUIState(renderer);

            
            // Update the node bounds
            Vector2 nodeSize;
            Vector2 nodeHeaderSize;
            var screenTitlePadding = new Vector2(5, 5) / camera.ZoomLevel;
            var screenBodyPadding = new Vector2(4, 4) / camera.ZoomLevel;
            
            const float pinLaneGap = 0;

            var desiredPinSizes = new Dictionary<MarkerGenRuleGraphPin, Vector2>();
            Vector2 desiredBodySize;
            float maxRightPinWidth = 0;
            {
                var bodySizeLeft = Vector2.zero;
                foreach (var pin in node.InputPins)
                {
                    var rulePin = pin as MarkerGenRuleGraphPin;
                    if (rulePin == null) continue;
                    var pinSize = CalcSize(rulePin, style);
                    bodySizeLeft.x = Mathf.Max(bodySizeLeft.x, pinSize.x);
                    bodySizeLeft.y += pinSize.y;

                    desiredPinSizes.Add(rulePin, pinSize);
                }
                var bodySizeRight = Vector2.zero;
                foreach (var pin in node.OutputPins)
                {
                    var rulePin = pin as MarkerGenRuleGraphPin;
                    if (rulePin == null) continue;
                    var pinSize = CalcSize(rulePin, style);
                    bodySizeRight.x = Mathf.Max(bodySizeRight.x, pinSize.x);
                    bodySizeRight.y += pinSize.y;
                    
                    desiredPinSizes.Add(rulePin, pinSize);
                    maxRightPinWidth = Mathf.Max(maxRightPinWidth, pinSize.x);
                }

                desiredBodySize = new Vector2(bodySizeLeft.x + pinLaneGap + bodySizeRight.x, Mathf.Max(bodySizeLeft.y, bodySizeRight.y));
                desiredBodySize /= camera.ZoomLevel;
                maxRightPinWidth /= camera.ZoomLevel;
            }

            float minWidth = 20;
            var titleSize = style.CalcSize(new GUIContent(title));
            desiredBodySize.x = Mathf.Max(desiredBodySize.x, titleSize.x);
            titleSize.x = Mathf.Max(titleSize.x, desiredBodySize.x);
            {
                int maxPins = Mathf.Max(node.InputPins.Length, node.OutputPins.Length);
                nodeHeaderSize = new Vector2(
                    Mathf.Max(titleSize.x, minWidth) + screenTitlePadding.x * 2,
                    titleSize.y + screenTitlePadding.y * 2);

                nodeSize = new Vector2(
                    Mathf.Max(titleSize.x, minWidth) + screenBodyPadding.x * 2,
                    nodeHeaderSize.y + desiredBodySize.y + screenBodyPadding.y * 2);


                nodeSize.x = Mathf.Max(nodeSize.x, nodeHeaderSize.x);
                nodeSize.y = Mathf.Max(nodeSize.y, nodeHeaderSize.y + 10);
            }

            Rect titleBounds;
            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position);
                var sizeScreen = nodeSize;
                boxBounds = new Rect(positionScreen, sizeScreen);
                titleBounds = new Rect(positionScreen, nodeHeaderSize);
            }

            Rect textBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + screenTitlePadding);
                var sizeScreen = titleSize;
                textBounds = new Rect(positionScreen, sizeScreen);
            }

            // Draw the body
            {
                renderer.backgroundColor = ruleNode.BodyColor;
                renderer.Box(boxBounds, new GUIContent(), styleBody);
            }

            // Draw the title
            {
                renderer.backgroundColor = ruleNode.TitleColor;
                renderer.Box(titleBounds, new GUIContent(), styleTitle);
            }

            // Draw the input pins
            {
                float currentX = boxBounds.xMin + screenBodyPadding.x;
                float currentY = titleBounds.yMax + screenBodyPadding.y;
                foreach (var pin in node.InputPins)
                {
                    var rulePin = pin as MarkerGenRuleGraphPin;
                    if (rulePin == null || !desiredPinSizes.ContainsKey(rulePin)) continue;

                    var pinSizeScreen = desiredPinSizes[rulePin] / camera.ZoomLevel;
                    var pinBounds = new Rect(new Vector2(currentX, currentY), pinSizeScreen);
                    DrawPin(renderer, rulePin, style, pinBounds, camera);
                    
                    
                    currentY += pinSizeScreen.y;
                }
                
            }
            // Draw the output pins
            {
                float currentX = boxBounds.xMax - screenBodyPadding.x - maxRightPinWidth;
                float currentY = titleBounds.yMax + screenBodyPadding.y;
                foreach (var pin in node.OutputPins)
                {
                    var rulePin = pin as MarkerGenRuleGraphPin;
                    if (rulePin == null || !desiredPinSizes.ContainsKey(rulePin)) continue;

                    var pinSizeScreen = desiredPinSizes[rulePin] / camera.ZoomLevel;
                    var pinBounds = new Rect(new Vector2(currentX, currentY), pinSizeScreen);

                    DrawPin(renderer, rulePin, style, pinBounds, camera);
                    
                    currentY += pinSizeScreen.y;
                }
            }
            
            // Draw the selection
            if (node.Selected)
            {
                var selectionColor = new Color(1.0f, 0.5f, 0.0f);
                renderer.backgroundColor = selectionColor;
                var selectionBounds = MathUtils.ExpandRect(boxBounds, 1);
                renderer.Box(selectionBounds, new GUIContent(), styleSelectBorder);
            }
            
            style.normal.textColor = titleColor;
            renderer.Label(textBounds, title, style);

            var updateWorldSize = nodeSize * camera.ZoomLevel;
            {
                var nodeBounds = node.Bounds;
                nodeBounds.size = updateWorldSize;
                node.Bounds = nodeBounds;
            }

            guiState.Restore();
        }

        private GUIStyle CreateStyle(bool selected, float zoomLevel)
        {
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.UpperLeft;
            style.normal.textColor = selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;
            
            style.font = EditorStyles.standardFont;
            float scaledFontSize = style.fontSize == 0 ? style.font.fontSize : style.fontSize;
            scaledFontSize = Mathf.Max(1.0f, scaledFontSize / zoomLevel);
            style.fontSize = Mathf.RoundToInt(scaledFontSize);

            return style;
        }
    }
}