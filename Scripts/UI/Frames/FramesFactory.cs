using Godot;
using PunkCommandSystem;
using System;
using System.Collections.Generic;

namespace RollPunk.UI.Frames
{
    public class FramesFactory
    {
        
        public Frame LoadFrame(string path)
        {
            PackedScene scene = GD.Load<PackedScene>(path);
            Frame frame = GetFrameFromPackedScene(scene);

            return frame;
        }

        

        public void AddCommands(CommandManager commandManager)
        {
            //var setTabCommand = new Command(
            //_name: "mainframe-set-tab",
            //_description: "Sets tab with this name enabled",
            //_action: _CommandSetTab,
            //_runOtherCommandMethod: commandManager.ExecuteCommandAsync,
            //_parameters: new List<RequiredParameter>
            //{
            //new RequiredParameter(new StringParameterType(), "tab-name")
            //}
            //);
            //commandManager.AddCommand(setTabCommand);

            //var addTabCommand = new Command(
            //_name: "mainframe-add-tab",
            //_description: "Adds tab with this name and this form to TabBar",
            //_action: _CommandAddTab,
            //_runOtherCommandMethod: commandManager.ExecuteCommandAsync,
            //_parameters: new List<RequiredParameter>
            //{
            //new RequiredParameter(new StringParameterType(), "tab-name"),
            //new RequiredParameter(new StringParameterType(), "tab-scene-path"),
            //new RequiredParameter(new IntParameterType(), "tab-priority")
            //}
            //);
            //commandManager.AddCommand(addTabCommand);

            //var setFormCommand = new Command(
            //_name: "mainframe-set-form",
            //_description: "Set this form enabled",
            //_action: _CommandSetForm,
            //_runOtherCommandMethod: commandManager.ExecuteCommandAsync,
            //_parameters: new List<RequiredParameter>
            //{
            //new RequiredParameter(new StringParameterType(), "form-scene-path")
            //}
            //);
            //commandManager.AddCommand(setFormCommand);
        }

        protected Frame GetFrameFromPackedScene(PackedScene packedScene)
        {
            var _frame = packedScene.Instantiate() as Frame;
            if (_frame == null)
            {
                throw new InvalidOperationException("Scene is not Frame Type");
            }
            return _frame;
        }
    }
}