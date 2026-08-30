namespace RollPunk.MembersExposing
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class ExposedPropertyAttribute : Attribute
    {
        /// <summary>
        /// Явно задаёт имя метода-сеттера.
        /// Если null — используется convention Set{PropertyName}.
        /// </summary>
        public string? Setter { get; init; }

        /// <summary>
        /// Если true, свойство нельзя изменять через exposed-интерфейс.
        /// </summary>
        public bool ReadOnly { get; init; }

        /// <summary>
        /// Имя, которое будет отображаться в интерфейсе.
        /// Если null — используется имя C# property.
        /// </summary>
        public string? DisplayName { get; init; }
    }
}
