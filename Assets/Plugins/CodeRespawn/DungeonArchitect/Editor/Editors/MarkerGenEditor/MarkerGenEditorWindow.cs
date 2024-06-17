using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Editors.MarkerGenerator.Editors;
using DungeonArchitect.Editors.MarkerGenerator.TextGen;
using DungeonArchitect.Editors.MarkerGenerator.UI;
using DungeonArchitect.Editors.MarkerGenerator.UI.Viewport;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Nodes;
using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Impl.UnityEditor;
using DungeonArchitect.UI.Widgets;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator
{
    public class MarkerGenEditorWindow : EditorWindow
    {
        private UIRenderer renderer;
        private bool requestRepaint = false;

        private MarkerGenPatternListPanel patternListPanel;
        private PatternViewportWidget patternViewport;
        private ToolbarWidget patternViewportToolbar;
        private GraphPanel<MarkerGenRuleGraphEditor> ruleGraphPanel;
        private BorderWidget ruleGraphEditorHost;
        private IWidget ruleGraphNullWidget;
        private SpacerWidget toolbarPadding;
        private MarkerGenEditor editor;
        
        [SerializeField]
        private MarkerGenPattern activePattern = null;
        
        [SerializeField]
        private MarkerGenRule activeRule = null;
        
        static readonly string BTN_RECENTER_VIEW = "RecenterView";

        public UISystem uiSystem { get; private set; }
        public MarkerGeneratorAsset Asset { get; set; }
        
        private List<IDeferredUICommand> deferredCommands = new List<IDeferredUICommand>();

        private double lastUpdateTimestamp = 0;
        public void Init(MarkerGeneratorAsset asset)
        {
            titleContent = new GUIContent("Pattern Editor");
            Asset = asset;

            if (asset == null)
            {
                BuildEmptyLayout();
                return;
            }

            lastUpdateTimestamp = EditorApplication.timeSinceStartup;
            CreateUISystem();
            
            // Build the Rule graph
            {
                ruleGraphEditorHost = new BorderWidget()
                    .SetPadding(0, 0, 0, 0);
                
                ruleGraphNullWidget =  new BorderWidget()
                    .SetPadding(0, 0, 0, 0)
                    .SetColor(new Color(0.2f, 0.2f, 0.2f))
                    .SetContent(
                        new LabelWidget("Select a rule block to view the graph")
                            .SetColor(new Color(1, 1, 1, 0.5f))
                            .SetFontSize(24)
                            .SetTextAlign(TextAnchor.MiddleCenter));

                ruleGraphEditorHost.SetContent(ruleGraphNullWidget);
            }
            
            // Build the pattern list view panel
            {
                patternListPanel = new MarkerGenPatternListPanel(asset);
                patternListPanel.ListView.SelectionChanged += PatternListView_SelectionChanged;
                patternListPanel.ListView.ItemClicked += PatternListView_ItemClicked;
                patternListPanel.OnAddItem += OnAddPatternClicked;
                patternListPanel.OnRemoveItem += OnRemoveItemClicked;
            }

            // Build the pattern viewport
            {
                patternViewport = new PatternViewportWidget();
                patternViewport.SetClearState(true, true, new Color(0.90f, 0.95f, 1.0f));
                patternViewport.MoveSpeed = 8;
                
                patternViewportToolbar = new ToolbarWidget();
                patternViewportToolbar.ButtonSize = 24;
                patternViewportToolbar.Padding = 0;
                patternViewportToolbar.Background = new Color(0, 0, 0, 0);
                patternViewportToolbar.AddButton(BTN_RECENTER_VIEW, UIResourceLookup.ICON_ZOOMFIT_16x);
                patternViewportToolbar.ButtonPressed += RecenterPatternViewport;
            }
            
            editor = asset != null ? MarkerGenEditorFactory.Create(asset) : null;
            if (editor != null)
            {
                editor.PatternViewport = patternViewport;
                editor.RuleSelected += OnRuleSelected;
            }

            BuildLayout();

            if (activePattern == null)
            {
                activePattern = asset.patterns.Length > 0 ? asset.patterns[0] : null;
            }

            if (activePattern != null)
            {
                patternListPanel.ListView.SetSelectedItem(uiSystem, activePattern);
            }
        }

        private void OnRuleSelected(MarkerGenRule rule)
        {
            SetActiveRule(rule);
        }

        private void OnAddPatternClicked()
        {
            if (editor != null)
            {
                var pattern = MarkerGenEditorUtils.AddNewPattern(editor.PatternType, Asset, uiSystem.Platform);
                int index = Array.FindIndex(Asset.patterns, l => l == pattern);
                patternListPanel.ListView.NotifyDataChanged();
                patternListPanel.ListView.SetSelectedIndex(index);
            }
        }

        private void OnRemoveItemClicked(MarkerGenPattern pattern)
        {
            if (pattern != null)
            {
                string message = string.Format("Are you sure you want to delete the pattern \'{0}\'?", pattern.patternName);
                bool removeItem = EditorUtility.DisplayDialog("Delete Pattern?", message, "Delete", "Cancel");
                if (removeItem)
                {
                    int index = System.Array.FindIndex(Asset.patterns, r => r == pattern);
                    MarkerGenEditorUtils.RemovePattern(Asset, pattern);
                    patternListPanel.ListView.NotifyDataChanged();

                    if (index >= Asset.patterns.Length)
                    {
                        index = Asset.patterns.Length - 1;
                    }
                    patternListPanel.ListView.SetSelectedIndex(index);
                }
            }
        }

        private void OnEnable()
        {
            this.wantsMouseMove = true;
            if (ruleGraphPanel != null && ruleGraphPanel.GraphEditor != null)
            {
                ruleGraphPanel.GraphEditor.OnEnable();
            }
            
            DungeonPropertyEditorHook.Get().DungeonBuilt += OnLinkedDungeonBuilt;
            InspectorNotify.MarkerGenPropertyChanged += OnPropertyChanged;
        }
        
        private void OnDisable()
        {
            if (ruleGraphPanel != null && ruleGraphPanel.GraphEditor != null)
            {
                ruleGraphPanel.GraphEditor.OnDisable();
            }

            if (patternViewport != null)
            {
                patternViewport.Release();
            }

            DungeonPropertyEditorHook.Get().DungeonBuilt -= OnLinkedDungeonBuilt;
            InspectorNotify.MarkerGenPropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object obj)
        {
            if (obj is GraphNode || obj is GraphPin || obj is Graph)
            {
                RuleGraphChanged();
            }
            if (editor != null && obj is MarkerGenRule rule)
            {
                editor.HandleRulePropertyChange(rule);
            }

            requestRepaint = true;
        }

        private void OnLinkedDungeonBuilt(Dungeon dungeon)
        {
        }

        private void RecenterPatternViewport(UISystem uiSystem, string id)
        {
            if (id == BTN_RECENTER_VIEW)
            {
                patternViewport.RecenterView();
            }
        }

        void Update()
        {
            double frameTime = EditorApplication.timeSinceStartup - lastUpdateTimestamp;
            lastUpdateTimestamp = EditorApplication.timeSinceStartup;
            
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (Asset == null)
            {
                return;
            }
            
            if (IsEditorStateInvalid())
            {
                Init(Asset);
            }

            var bounds = new Rect(Vector2.zero, position.size);
            if (uiSystem != null)
            {
                uiSystem.Update(bounds);
            }

            if (editor != null)
            {
                editor.Update(frameTime);
                if (editor.RequestRepaint)
                {
                    editor.RequestRepaint = false;
                    patternViewport?.Invalidate();
                    requestRepaint = true;
                }
            }
            
            if (patternViewport != null)
            {
                requestRepaint |= patternViewport.RequiresRepaint();
            }
            
            if (requestRepaint)
            {
                Repaint();
                requestRepaint = false;
            }
            
            ProcessDeferredCommands();
        }

        void OnGUI()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (Asset == null)
            {
                return;
            }

            if (uiSystem.Layout == null)
            {
                BuildLayout();
            }
            
            if (ruleGraphPanel != null && ruleGraphPanel.GraphEditor != null)
            {
                ruleGraphPanel.GraphEditor.Update();
            }

            if (IsEditorStateInvalid())
            {
                // Wait for the next update cycle to fix this
                return;
            }
            
            var bounds = new Rect(Vector2.zero, position.size);
            renderer.DrawRect(bounds, new Color(0.5f, 0.5f, 0.5f));

            DrawToolbar();
            uiSystem.Draw(renderer);
            
            var e = Event.current;
            if (e != null)
            {
                if (e.isScrollWheel)
                {
                    requestRepaint = true;
                }

                switch (e.type)
                {
                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                    case EventType.MouseDown:
                    case EventType.MouseUp:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.MouseEnterWindow:
                    case EventType.MouseLeaveWindow:
                        requestRepaint = true;
                        break;
                }
            }
            
            HandleInput(Event.current);
        }

        void DrawToolbar()
        {
            var guiState = new GUIState(renderer);
            var rect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(30));

            var iconBuild = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_PLAY_16xb) as Texture2D;
            renderer.backgroundColor = new Color(1, 0.25f, 0.25f, 1);

            if (GUILayout.Button(new GUIContent("Rebuild Dungeon", iconBuild), EditorStyles.toolbarButton))
            {
                HandleRebuildDungeonButtonPressed();
            }
            GUILayout.Space(5);
            renderer.backgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0.5f, 0.5f, 0.5f, 1.0f)
                : new Color(0.85f, 0.85f, 0.85f, 1.0f);

            GUILayout.FlexibleSpace();

            {
                renderer.color = Color.white;
                
                var iconDiscord = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DISCORD_16x) as Texture2D;
                if (GUILayout.Button(new GUIContent(" Discord Support", iconDiscord), DungeonEditorStyles.discordToolButtonStyle))
                {
                    ExternalLinks.LaunchUrl(ExternalLinks.DiscordInvite);
                }

                renderer.backgroundColor = new Color(0.25f, 0.25f, 1, 1);
                renderer.color = Color.white;
                var iconDocs = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DOCS_16x) as Texture2D;
                if (GUILayout.Button(new GUIContent("Documentation", iconDocs), DungeonEditorStyles.discordToolButtonStyle))
                {
                    ExternalLinks.LaunchUrl(ExternalLinks.DocumentationPatternEditor);
                }

            }
            
            EditorGUILayout.EndHorizontal();

            if (toolbarPadding != null && rect.height > 0)
            {
                toolbarPadding.SetSize(new Vector2(1, rect.height));
            }
            
            guiState.Restore();
        }

        void FindAndRebuildTrackedDungeons()
        {
            var trackedDungeons = UnityEngine.Object.FindObjectsOfType<Dungeon>();
            trackedDungeons = trackedDungeons.Where(d => d.patterns.Contains(Asset)).ToArray();
            foreach (var trackedDungeon in trackedDungeons)
            {
                trackedDungeon.Build(new EditorDungeonSceneObjectInstantiator());
            }
        }

        private void HandleRebuildDungeonButtonPressed()
        {
            FindAndRebuildTrackedDungeons();
        }

        void ProcessDeferredCommands()
        {
            // Execute the deferred UI commands
            foreach (var command in deferredCommands)
            {
                command.Execute(uiSystem);
            }

            deferredCommands.Clear();
        }

        void HandleInput(Event e)
        {
            if (uiSystem != null)
            {
                if (renderer == null)
                {
                    CreateUISystem();
                }

                if (uiSystem.Layout != null)
                {
                    var layout = uiSystem.Layout;
                    if (e.type == EventType.MouseDown || e.type == EventType.ScrollWheel || e.type == EventType.MouseMove)
                    {
                        WidgetUtils.ProcessInputFocus(e.mousePosition, uiSystem, layout);
                    }

                    if (uiSystem.IsDragDrop)
                    {
                        WidgetUtils.ProcessDragOperation(e, layout, uiSystem);
                    }

                    UpdateDragDropState(e);

                    if (uiSystem.FocusedWidget != null)
                    {
                        Vector2 resultMousePosition = Vector2.zero;
                        if (WidgetUtils.BuildWidgetEvent(e.mousePosition, layout, uiSystem.FocusedWidget, ref resultMousePosition))
                        {
                            Event widgetEvent = new Event(e);
                            widgetEvent.mousePosition = resultMousePosition;
                            uiSystem.FocusedWidget.HandleInput(widgetEvent, uiSystem);

                            if (uiSystem.FocusedWidget == patternViewport && editor != null)
                            {
                                editor.HandleInput(widgetEvent, uiSystem);
                            }
                        }
                    }
                }
            }
        }
        
        void UpdateDragDropState(Event e)
        {
            if (uiSystem != null)
            {
                if (e.type == EventType.DragUpdated)
                {
                    uiSystem.SetDragging(true);
                }
                else if (e.type == EventType.DragPerform || e.type == EventType.DragExited)
                {
                    uiSystem.SetDragging(false);
                }
            }
        }
        
        private void PatternListView_SelectionChanged(MarkerGenPattern item)
        {
            SetActivePattern(item);
            Selection.activeObject = item;
        }

        private void PatternListView_ItemClicked(MarkerGenPattern item)
        {
            SetActivePattern(item);
            Selection.activeObject = item;
        }

        private void SetActivePattern(MarkerGenPattern pattern)
        {
            activePattern = pattern;
            LoadScene(pattern);
            SetActiveRule(null);
        }

        private void LoadScene(MarkerGenPattern pattern)
        {
            if (patternViewport != null && uiSystem != null)
            {
                patternViewport.World.Clear();

                if (editor != null)
                {
                    editor.LoadScene(pattern, uiSystem.Platform);
                }
                
                patternViewport.RecenterView();
            }
        }
        
        private void SetActiveRule(MarkerGenRule rule)
        {
            activeRule = rule;
            SetRuleGraph(rule);
        }

        private void SetRuleGraph(MarkerGenRule rule)
        {
            var ruleGraph = rule != null ? rule.ruleGraph : null;
            if (ruleGraph != null)
            {
                ruleGraphPanel = new GraphPanel<MarkerGenRuleGraphEditor>(ruleGraph, Asset, uiSystem);
                ruleGraphPanel.Border.SetTitle("Rule Graph");
                ruleGraphPanel.Border.SetColor(new Color(0.2f, 0.3f, 0.2f));
                ruleGraphPanel.GraphEditor.supportInteractiveWidgets = true;
                ruleGraphPanel.GraphEditor.GraphChanged += (graph, system) => RuleGraphChanged();
                ruleGraphEditorHost.SetContent(ruleGraphPanel);

                InitializeRuleGraphCamera();
                UpdateCompileStatus();
            }
            else
            {
                ruleGraphEditorHost.SetContent(ruleGraphNullWidget);
            }            
        }

        private void RuleGraphChanged()
        {
            if (activeRule == null)
            {
                return;
            }
            
            // Compile the rule program whenever the graph changes
            CompileProgram(uiSystem.Platform);
            UpdateCompileStatus();
            
            if (editor != null)
            {
                editor.OnRuleGraphChanged(activeRule);
            }
        }

        private void UpdateCompileStatus()
        {
            if (activeRule != null && ruleGraphPanel != null && ruleGraphPanel.GraphEditor != null)
            {
                ruleGraphPanel.GraphEditor.drawInvalidGraphMessage = (activeRule.program != null && !activeRule.program.compiled);
            }
        }
        
        public void CompileProgram(UIPlatform platform)
        {
            if (activeRule != null && activeRule.ruleGraph != null)
            {
                MarkerGenEditorUtils.CompileRule(activeRule, Asset, platform);
            }
        }
        
        void InitializeRuleGraphCamera()
        {
            if (uiSystem != null && uiSystem.Layout != null)
            {
                deferredCommands.Add(new EditorCommand_InitializeGraphCameras(uiSystem.Layout));
            }
        }
        
        void CreateUISystem()
        {
            uiSystem = new UnityEditorUISystem();
            renderer = new UnityEditorUIRenderer();
        }

        bool IsEditorStateInvalid()
        {
            return patternListPanel == null 
                   || ruleGraphEditorHost == null
                   || editor == null
                   || patternViewport == null
                   || patternViewport.Renderer == null
                   || patternViewport.Renderer.Texture == null
        ;
    }

        void BuildLayout()
        {
            IWidget layout = new Splitter(SplitterDirection.Vertical)
                    .AddWidget(
                        new Splitter(SplitterDirection.Horizontal)
                            .AddWidget(patternListPanel)
                            .AddWidget(new OverlayPanelWidget()
                                .AddWidget(patternViewport)
                                .AddWidget(patternViewportToolbar, OverlayPanelHAlign.Right, OverlayPanelVAlign.Top, new Vector2(24, 24), new Vector2(10, 10))
                                , 4)
                    )
                    .AddWidget(ruleGraphEditorHost)
                ;

            toolbarPadding = new SpacerWidget(new Vector2(20, 20)); 
            layout = new StackPanelWidget(StackPanelOrientation.Vertical)
                .AddWidget(toolbarPadding, 0, true)
                .AddWidget(layout);
            
            uiSystem.SetLayout(layout);

            deferredCommands.Add(new EditorCommand_InitializeGraphCameras(layout));
        }

        private void BuildEmptyLayout()
        {
            var layout = new LabelWidget("Open a Theme Pattern to start editing")
                .SetColor(new Color(1, 1, 1, 0.5f))
                .SetFontSize(24)
                .SetTextAlign(TextAnchor.MiddleCenter);
            
            uiSystem.SetLayout(layout);
        }


        private void ButtonOnButtonPressed(UISystem uisystem)
        {
            Debug.Log("Button pressed");
        }
    }
}