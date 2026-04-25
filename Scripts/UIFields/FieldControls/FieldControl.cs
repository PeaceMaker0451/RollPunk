using Godot;
using SquadfestBot;
using System;
using System.Threading.Tasks;

namespace RollPunk.UIFields
{
    public abstract partial class FieldControl : Control
    {
        protected Func<LineField, bool> ViewCheck;
        protected Func<LineField, bool> EditCheck;

        public event Action ViewCheckChanged;
        public event Action EditCheckChanged;

        private bool _isSubscriptionsAdded = false;
        
        public override void _ExitTree()
        {
            if (_isSubscriptionsAdded)
                RemoveSubscriptions();

            OnExittingTree();
        }


        public new string GetName()
        {
            return StringTemplate.Format(GetField().VisibleName, GetField().AdditionalData);
        }

        public void SetViewCheck(Func<LineField, bool> check)
        {
            ViewCheck = check;
            CheckVisibility();
            ViewCheckChanged?.Invoke();
        }

        public void SetEditCheck(Func<LineField, bool> check)
        {
            EditCheck = check;
            CheckEditability();
            EditCheckChanged?.Invoke();
        }

        protected void AddSubscriptions()
        {
            var field = GetField();
            field.VisibleNameChanged += UpdateName;
            field.ValueChanged += UpdateValue;
            field.ViewAccessLevelChanged += CheckVisibility;
            field.EditAccessLevelChanged += CheckEditability;
            field.AdditionalDataChanged += OnAdditionalDataChanged;

            _isSubscriptionsAdded = true;
        }

        protected void RemoveSubscriptions()
        {
            var field = GetField();
            field.VisibleNameChanged -= UpdateName;
            field.ValueChanged -= UpdateValue;
            field.ViewAccessLevelChanged -= CheckVisibility;
            field.EditAccessLevelChanged -= CheckEditability;
            field.AdditionalDataChanged -= OnAdditionalDataChanged;

            _isSubscriptionsAdded = false;
        }

        protected void CheckVisibility()
        {
            if (ViewCheck == null)
                return;

            bool result = ViewCheck.Invoke(GetField());
            SetVisible(result);
        }

        protected void CheckEditability()
        {
            if (EditCheck == null)
                return;

            bool result = EditCheck.Invoke(GetField());
            SetEditable(result);
        }

        protected void UpdateName()
        {
            SetName(GetName());
        }

        protected async Task PlayLabelAnimation(Label label)
        {
            //label.VisibleRatio = 0;
            //while(label.VisibleRatio < 1)
            //{
            //    label.VisibleRatio += 1f * (float)GetProcessDeltaTime();
            //    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            //}
        }

        public abstract LineField GetField();

        protected new abstract void SetVisible(bool visible);
        protected abstract void SetEditable(bool editable);
        protected new abstract void SetName(string name);
        protected abstract void UpdateValue();
        protected virtual void OnAdditionalDataChanged(string dataName) { }

        protected virtual void OnExittingTree() { }
    }
}
