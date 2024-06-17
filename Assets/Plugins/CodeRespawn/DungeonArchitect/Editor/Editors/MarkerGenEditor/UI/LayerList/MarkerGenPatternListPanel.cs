using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.UI
{
    class MarkerGenPatternListViewConstants
    {
        public static readonly string DragDropID = "RuleDragOp";
        public static readonly Color ThemeColor = new Color(0.3f, 0.3f, 0.2f);
    }

    public class MarkerGenPatternListViewItem : ListViewTextItemWidget
    {
        public MarkerGenPatternListViewItem(MarkerGenPattern pattern)
            : base(pattern, () => pattern.patternName)
        {
        }
        
    }
    
    public class MarkerGenPatternListViewSource : ListViewSource<MarkerGenPattern>
    {
        MarkerGeneratorAsset asset;
        public MarkerGenPatternListViewSource(MarkerGeneratorAsset asset)
        {
            this.asset = asset;
        }
        public override MarkerGenPattern[] GetItems()
        {
            return asset != null ? asset.patterns : null;
        }

        public override IWidget CreateWidget(MarkerGenPattern item)
        {
            var itemWidget = new MarkerGenPatternListViewItem(item);
            itemWidget.TextStyle.fontSize = 16;

            itemWidget.SelectedTextStyle = new GUIStyle(itemWidget.TextStyle);
            itemWidget.SelectedTextStyle.normal.textColor = Color.black;
            itemWidget.SelectedColor = MarkerGenPatternListViewConstants.ThemeColor * 2;

            return itemWidget;
        }
    }

    
    public delegate void MarkerGenPatternEvent();
    public delegate void MarkerGenPatternObjEvent(MarkerGenPattern pattern);

    public class MarkerGenPatternListPanel : WidgetBase
    {
        MarkerGeneratorAsset asset;

        IWidget host;

        public ListViewWidget<MarkerGenPattern> ListView;
        ToolbarWidget toolbar;

        public event MarkerGenPatternEvent OnAddItem;
        public event MarkerGenPatternObjEvent OnRemoveItem;

        readonly static string BTN_ADD_ITEM = "AddItem";
        readonly static string BTN_REMOVE_ITEM = "RemoveItem";
        readonly static string BTN_MOVE_UP = "MoveUp";
        readonly static string BTN_MOVE_DOWN = "MoveDown";

        public MarkerGenPatternListPanel(MarkerGeneratorAsset asset)
        {
            this.asset = asset;

            toolbar = new ToolbarWidget();
            toolbar.ButtonSize = 24;
            toolbar.Padding = 4;
            toolbar.Background = new Color(0, 0, 0, 0);
            toolbar.AddButton(BTN_ADD_ITEM, UIResourceLookup.ICON_PLUS_16x);
            toolbar.AddButton(BTN_REMOVE_ITEM, UIResourceLookup.ICON_CLOSE_16x);
            toolbar.AddButton(BTN_MOVE_UP, UIResourceLookup.ICON_MOVEUP_16x);
            toolbar.AddButton(BTN_MOVE_DOWN, UIResourceLookup.ICON_MOVEDOWN_16x);
            toolbar.ButtonPressed += Toolbar_ButtonPressed;
            var toolbarSize = new Vector2(toolbar.Padding * 2 + toolbar.ButtonSize * 4, toolbar.Padding * 2 + toolbar.ButtonSize);

            ListView = new ListViewWidget<MarkerGenPattern>();
            ListView.ItemHeight = 45;
            ListView.Bind(new MarkerGenPatternListViewSource(asset));

            IWidget toolWidget = new StackPanelWidget(StackPanelOrientation.Horizontal)
                                .AddWidget(new NullWidget())
                                .AddWidget(toolbar, toolbarSize.x);

            toolWidget = new BorderWidget(toolWidget)
                .SetPadding(0, 0, 0, 0)
                .SetDrawOutline(false)
                .SetColor(new Color(0, 0, 0, 0.25f));

            host = new BorderWidget()
                   .SetTitle("Patterns")
                   .SetColor(MarkerGenPatternListViewConstants.ThemeColor)
                   .SetContent(
                        new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(toolWidget, toolbarSize.y)
                        .AddWidget(ListView)
                    );
        }


        private void Toolbar_ButtonPressed(UISystem uiSystem, string id)
        {
            if (asset == null)
            {
                return;
            }

            if (id == BTN_ADD_ITEM)
            {
                if (OnAddItem != null)
                {
                    OnAddItem.Invoke();
                }
            }
            else if (id == BTN_REMOVE_ITEM)
            {
                var pattern = ListView.GetSelectedItem();
                if (pattern != null) 
                {
                    if (OnRemoveItem != null)
                    {
                        OnRemoveItem.Invoke(pattern);
                    }
                }
                
            }
            else if (id == BTN_MOVE_UP)
            {
                var nodeType = ListView.GetSelectedItem();
                var list = new List<MarkerGenPattern>(asset.patterns);
                int index = list.IndexOf(nodeType);
                if (index > 0)
                {
                    list.RemoveAt(index);
                    index--;
                    list.Insert(index, nodeType);
                    asset.patterns = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
            }
            else if (id == BTN_MOVE_DOWN)
            {
                var rule = ListView.GetSelectedItem();
                var list = new List<MarkerGenPattern>(asset.patterns);
                int index = list.IndexOf(rule);
                if (index + 1 < list.Count)
                {
                    list.RemoveAt(index);
                    index++;
                    list.Insert(index, rule);
                    asset.patterns = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (host != null)
            {
                var childBounds = new Rect(Vector2.zero, bounds.size);
                host.UpdateWidget(uiSystem, childBounds);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            host.Draw(uiSystem, renderer);
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            host.HandleInput(e, uiSystem);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { host };
        }
    }
}