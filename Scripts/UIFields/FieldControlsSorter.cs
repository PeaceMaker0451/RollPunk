using Godot;
using RollPunk.Debug;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Scripts.UIFields
{
    internal static class FieldControlsSorter
    {
        public static void Sort(Node controlsParent)
        {
            //RPDebug.Log($"---------------Начало сортировки для ноды {controlsParent.Name}------------------");
            var children = controlsParent.GetChildren();
            List<FieldControl> childrenFieldControls = children.OfType<FieldControl>().ToList();

            childrenFieldControls = childrenFieldControls.OrderByDescending(control => control.GetField().LinePriority).ToList();

            for (int i = 0; i < childrenFieldControls.Count; i++)
            {
                //RPDebug.Log($"Двигаем контрол {childrenFieldControls[i].GetField().Name} на позицию {childrenFieldControls[i].GetField().LinePriority}({i}) под нодой {controlsParent.Name}");
                controlsParent.MoveChild(childrenFieldControls[i], i);
                //RPDebug.Log($"Теперь ее позиция - {childrenFieldControls[i].GetIndex()}");
            }
            //RPDebug.Log($"--------------------------Конец сортировки---------------------------------------");
        }
    }
}
