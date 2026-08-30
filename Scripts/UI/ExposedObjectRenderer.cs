using Godot;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.MembersExposing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.UI
{
    internal partial class ExposedObjectRenderer : Control
    {
        private Container _root;
        private Dictionary<ItemList, List<object>> _collectionObjects = new();

        public event Action<object> CollectionObjectSelected;
        
        public override void _Ready()
        {
            _root = new HFlowContainer();
            AddChild(_root);
        }
        
        public void Render(ExposedObject exposed)
        {
            Clear();

            foreach (var member in exposed.Members)
            {
                switch (member.Kind)
                {
                    case ExposedMemberKind.Property:
                        DrawProperty(exposed.Target, (ExposedProperty)member);
                        break;

                    case ExposedMemberKind.Collection:
                        DrawCollection(exposed.Target, (ExposedCollection)member);
                        break;
                }
            }
        }

        private void Clear()
        {
            foreach (var child in _root.GetChildren())
                child.QueueFree();

            _collectionObjects.Clear();
        }

        private void DrawProperty(object target, ExposedProperty property)
        {
            object? value = property.GetValue(target);

            if (value == null)
                DrawStringProperty(target, property, "null", true);
            else if (property.ValueType == typeof(string))
                DrawStringProperty(target, property, value as string);
            else if (property.ValueType == typeof(int))
                DrawIntProperty(target, property, value as int?);
            else if (property.ValueType == typeof(Guid))
                DrawStringProperty(target, property, value.ToString(), true);
            else if (typeof(Field).IsAssignableFrom(property.ValueType))
                DrawStringProperty(target, property, (value as Field).Name, true);
            else
                DrawStringProperty(target, property, value.ToString(), true);
        }

        private void DrawCollection(object target, ExposedCollection property)
        {
            VBoxContainer vBoxContainer = new();
            ScrollContainer scrollContainer = new();
            Label title = new();
            ItemList itemList = new ItemList();

            scrollContainer.AddChild(vBoxContainer);
            vBoxContainer.AddChild(title);
            vBoxContainer.AddChild(itemList);

            title.Text = property.DisplayName;
            scrollContainer.CustomMinimumSize = new(350, 150);
            scrollContainer.CustomMaximumSize = new(500, 350);
            scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            scrollContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            scrollContainer.SizeFlagsVertical = SizeFlags.ExpandFill;
            itemList.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            itemList.SizeFlagsVertical = SizeFlags.ExpandFill;
            vBoxContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            vBoxContainer.SizeFlagsVertical = SizeFlags.ExpandFill;

            _collectionObjects[itemList] = new();

            foreach (var item in property.GetItems(target))
            {
                if(item == null)
                    itemList.AddItem("null");
                else if(typeof(Entity).IsAssignableFrom(item.GetType()))
                    itemList.AddItem((item as Entity).Name);
                else
                    itemList.AddItem(item.ToString());

                _collectionObjects[itemList].Add(item);
            }

            itemList.ItemSelected += (index) => CollectionObjectSelected?.Invoke(_collectionObjects[itemList][(int)index]);

            _root.AddChild(scrollContainer);
        }

        private void DrawStringProperty(object target, ExposedProperty property, string? value, bool isForceReadonly = false)
        {
            string currentValue = value ?? string.Empty;

            VBoxContainer vBoxContainer = new();
            MarginContainer margin = new();
            Label title = new();
            TextEdit text = new();

            margin.AddChild(vBoxContainer);
            vBoxContainer.AddChild(title);
            vBoxContainer.AddChild(text);

            _root.AddChild(margin);

            title.Text = property.DisplayName;
            text.Text = currentValue;
            margin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.SizeFlagsVertical = SizeFlags.ExpandFill;

            if (isForceReadonly)
                text.Editable = false;
            else
                text.Editable = property.ReadOnly == false;

            text.ScrollFitContentHeight = true;
            text.ScrollFitContentWidth = true;

            text.TextChanged += () => property.SetValue(target, text.Text);
        }

        private void DrawIntProperty(object target, ExposedProperty property, int? value, bool isForceReadonly = false)
        {
            int currentValue = value ?? 0;

            VBoxContainer vBoxContainer = new();
            MarginContainer margin = new();
            Label title = new();
            SpinBox number = new();

            number.MaxValue = int.MaxValue;
            number.MinValue = int.MinValue;
            margin.AddChild(vBoxContainer);
            vBoxContainer.AddChild(title);
            vBoxContainer.AddChild(number);

            _root.AddChild(margin);

            title.Text = property.DisplayName;
            number.Value = currentValue;
            margin.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            margin.SizeFlagsVertical = SizeFlags.ExpandFill;

            if (isForceReadonly)
                number.Editable = false;
            else
                number.Editable = property.ReadOnly == false;

            number.ValueChanged += (value) => property.SetValue(target, (int)number.Value);
        }
    }
}
