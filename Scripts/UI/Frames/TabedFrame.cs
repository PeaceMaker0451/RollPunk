using Godot;
using RollPunk.UI.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.UI.Frames
{
    internal class TabData
    {
        private Action<string> TabButtonToggled;

        public string Name;
        public Button Button;
        public Form Form;
        public int Priority;

        public TabData(string name, Button button, Form form, Action<string> _tabButtonToggled, int priority)
        {
            Name = name;
            Button = button;
            Form = form;
            TabButtonToggled = _tabButtonToggled;
            button.Toggled += _TabButtonToggled;

            this.Priority = priority;
        }

        public void _TabButtonToggled(bool toogled)
        {
            if (toogled)
            {
                TabButtonToggled?.Invoke(Name);
            }
            else
            {
                Button.SetPressedNoSignal(true);
            }
        }
    }

    public partial class TabedFrame : Frame
    {
        public static TabedFrame instance;
        [Export] public Node tabButtonsContainer { get; private set; }
        [Export] public string defaultTabButtonScene;
        public PackedScene defaultTabButton { get; private set; }

        private Dictionary<string, TabData> _tabs = new Dictionary<string, TabData>();
        private TabData _currentTab;

        public IReadOnlyDictionary<string, Form> Tabs
        {
            get
            {
                Dictionary<string, Form> dict = new Dictionary<string, Form>();

                foreach(var tab in _tabs.Values)
                    dict.Add(tab.Name, tab.Form);

                return dict;
            }
        }

        public Form CurrentTab => _currentTab?.Form;

        public override void _Ready()
        {
            base._Ready();
            instance = this;
            defaultTabButton = GD.Load<PackedScene>(defaultTabButtonScene);
        }

        public static TabedFrame GetInstance()
        {
            return instance;
        }

        public bool HasTab(string name)
        {
            return _tabs.ContainsKey(name);
        }

        public void AddTab(Form form, string name, int priority)
        {
            if(form == null)
                throw new ArgumentNullException("form");
            
            if(name == null)
                throw new ArgumentNullException("name");
            
            if (_tabs.ContainsKey(name))
                throw new Exception($"Tab {name} is already exists");

            if (defaultTabButton == null)
                throw new Exception("defaultTabButton is not set!");

            var _button = defaultTabButton.Instantiate() as Button;

            _button.Text = name;
            var tabData = new TabData(name, _button, form, SetTab, priority);

            _tabs.Add(name, tabData);
            tabButtonsContainer.AddChild(_button);

            SortTabsByPriority();

            if (CurrentTab == null)
                SetTab(form);
        }

        public void RemoveTab(Form form)
        {
            KeyValuePair<string, TabData> tabPair = _tabs.Where(t => t.Value.Form == form).First();
            TabData tab = tabPair.Value;

            if (tab == null)
                throw new InvalidOperationException($"Can't find form {form.Title}.");

            _tabs.Remove(tabPair.Key);
            form.GetParent()?.RemoveChild(form);
            tab.Button.QueueFree();

            SortTabsByPriority();
        }

        public void SetTab(string name)
        {
            if (!_tabs.ContainsKey(name))
                throw new InvalidOperationException($"Tab {name} doesn't exists");

            if (_currentTab == _tabs[name])
                throw new InvalidOperationException($"Tab {name} is already setted");

            var tab = _tabs[name];

            SetTab(tab.Form);
        }

        public void SetTab(Form form)
        {
            TabData tab = GetTab(form);
            SetForm(tab.Form);
            _currentTab = tab;

            tab.Button.SetPressedNoSignal(true);
        }

        public void DisableTab(Form form)
        {
            TabData tab = GetTab(form);
            tab.Button.Disabled = true;
        }

        public void EnableTab(Form form)
        {
            TabData tab = GetTab(form);
            tab.Button.Disabled = false;
        }

        public override void SetForm(Form form)
        {
            _currentTab = null;
            DeactivateAllTabButtons();
            HideAllForms();

            base.SetForm(form);
        }

        public Form GetTabForm(string name)
        {
            if (!_tabs.ContainsKey(name))
                throw new InvalidOperationException($"Tab {name} doesn't exists");

            var tab = _tabs[name];
            return tab.Form;
        }

        private void SortTabsByPriority()
        {
            var sortedTabs = _tabs.Values.OrderBy(tab => tab.Priority).ToList();

            foreach (var node in tabButtonsContainer.GetChildren())
                tabButtonsContainer.RemoveChild(node);

            foreach (var tab in sortedTabs)
                tabButtonsContainer.AddChild(tab.Button);
        }

        private TabData GetTab(Form form)
        {
            KeyValuePair<string, TabData> tabPair = _tabs.Where(t => t.Value.Form == form).First();
            TabData tab = tabPair.Value;

            if (tab == null)
                throw new InvalidOperationException($"Can't find form {form.Title}.");

            return tab;
        }

        private void DeactivateAllTabButtons()
        {
            foreach (var tab in _tabs)
            {
                tab.Value.Button.SetPressedNoSignal(false);
            }
        }

        private void HideAllForms()
        {
            if (CurrentForm != null)
                CurrentForm.Hide();
        }
    }
}