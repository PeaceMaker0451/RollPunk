using System;

namespace RollPunk.Fields
{
    public static class FieldExtensions
    {
        public static bool IsAncestorOf(this Field ancestor, Field descendant)
        {
            if (ancestor == null || descendant == null) return false;

            Field current = descendant;

            while (current != null)
            {
                var parent = current.Parent;

                if (parent == null)
                    return false;

                if (parent == ancestor)
                    return true;

                current = parent;
            }

            return false;
        }

        public static void MoveTo(this Field field, Field newOwner)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (newOwner == field.Parent) return;

            if (field.IsAncestorOf(newOwner))
                throw new InvalidOperationException("Cannot move field: operation would create ownership cycle.");

            var oldOwner = field.Parent;
            oldOwner?.RemoveField(field);
            newOwner?.AddField(field);
        }
    }
}
