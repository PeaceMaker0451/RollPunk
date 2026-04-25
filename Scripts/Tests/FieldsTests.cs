using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.UIFields;
using System.Text;
using Xunit.Abstractions;

namespace Tests
{
    public class FieldsTests
    {
        private readonly ITestOutputHelper _output;

        public FieldsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void HierarchyLoopTest()
        {
            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");
            EntityField entityField3 = new EntityField("NewEntity3");

            entityField1.AddField(entityField2);
            entityField2.AddField(entityField3);
            
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                entityField3.AddField(entityField1);
            });

            Assert.Equal("Cannot add field: operation would create ownership cycle.", exception.Message);
        }

        [Fact]
        public void HierarchyLoopTestLong()
        {
            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");
            EntityField entityField3 = new EntityField("NewEntity3");
            EntityField entityField4 = new EntityField("NewEntity4");
            EntityField entityField5 = new EntityField("NewEntity5");
            EntityField entityField6 = new EntityField("NewEntity6");
            EntityField entityField7 = new EntityField("NewEntity7");
            EntityField entityField8 = new EntityField("NewEntity8");
            EntityField entityField9 = new EntityField("NewEntity9");
            EntityField entityField10 = new EntityField("NewEntity10");

            entityField1.AddField(entityField2);
            entityField2.AddField(entityField3);
            entityField3.AddField(entityField4);
            entityField4.AddField(entityField5);
            entityField5.AddField(entityField6);
            entityField6.AddField(entityField7);
            entityField7.AddField(entityField8);
            entityField8.AddField(entityField9);
            entityField9.AddField(entityField10);

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                entityField10.AddField(entityField1);
            });

            Assert.Equal("Cannot add field: operation would create ownership cycle.", exception.Message);
        }


        [Fact] 
        public void AddFieldToFieldTest()
        {
            EntityField entityField1 = new EntityField("NewEntity");
            StringField stringField1 = new StringField("Test", "someName", PlayerRole.All, PlayerRole.All, "Value");
            entityField1.AddField(stringField1);

            Assert.Contains(stringField1, entityField1.Fields);
        }

        public void RemoveFieldFromFieldTest()
        {
            EntityField entityField1 = new EntityField("NewEntity");
            StringField stringField1 = new StringField("Test", "someName", PlayerRole.All, PlayerRole.All, "Value");
            entityField1.AddField(stringField1);

            Assert.Contains(stringField1, entityField1.Fields);

            entityField1.RemoveField(stringField1);

            Assert.DoesNotContain(stringField1, entityField1.Fields);
        }
        
        [Fact]
        public void ReconstructionConsistanceTest()
        {
            StringField field1 = new StringField("Test", "someName", PlayerRole.All, PlayerRole.All, "Value");

            var fieldState1 = FieldStateExtractor.ExtractFieldTreeState(field1);
            StringField field2 = new StringField(fieldState1.State);

            Assert.Equal(field1.ID, field2.ID);
            Assert.Equal(field1.Name, field2.Name);
            Assert.Equal(field1.Value, field2.Value);
        }

        [Fact]
        public void ReconstructionHierarchyConsistanceTest()
        {
            EntityField entityField1 = new EntityField("NewEntity");
            StringField stringField1 = new StringField("Test", "someName", PlayerRole.All, PlayerRole.All, "Value");
            entityField1.AddField(stringField1);

            var fieldState1 = FieldStateExtractor.ExtractFieldTreeState(entityField1);

            EntityFactory entityFactory = new EntityFactory();
            entityFactory.Register<EntityField>();
            entityFactory.Register<StringField>();
            entityFactory.Register<IntField>();

            FieldsHierarchyReconstructor fieldsHierarchyReconstructor = new FieldsHierarchyReconstructor(entityFactory);

            EntityField entityField2 = (EntityField)fieldsHierarchyReconstructor.CreateFieldsTree(fieldState1);
            StringField stringField2 = (StringField)entityField2.Fields[0];


            Assert.Equal(entityField1.ID, entityField2.ID);
            Assert.Equal(stringField1.ID, stringField2.ID);

            Assert.Equal(entityField1.Name, entityField2.Name);
            Assert.Equal(stringField1.Name, stringField2.Name);

            Assert.Equal(stringField1.Value, stringField2.Value);
        }


        [Fact]
        public void RegistrySyncTest()
        {
            FieldsContainer<EntityField> fieldsContainer1 = new FieldsContainer<EntityField>();

            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");

            entityField1.AddField(new StringField("SomeString", "someName", PlayerRole.All, PlayerRole.All, "Value"));
            entityField1.AddField(new IntField("SomeInt", "someName", PlayerRole.All, PlayerRole.All, 5));

            entityField2.AddField(new StringField("SomeOtherString", "someName", PlayerRole.All, PlayerRole.All, "Goool"));
            IntField someInt = new IntField("SomeOtherInt", "someName", PlayerRole.All, PlayerRole.All, 46);
            entityField2.AddField(someInt);

            fieldsContainer1.AddField(entityField1);
            fieldsContainer1.AddField(entityField2);

            FieldsRegistry registry = new FieldsRegistry(fieldsContainer1);

            Assert.True(registry.GetField(someInt.ID) != null);
            Assert.Contains(someInt.ID, registry.FieldsDictionary);

            StringField newString = new StringField("BrandNewStringField", "someName", PlayerRole.All, PlayerRole.All, "SomeImportantData");

            bool raisedEntityChanged = false;
            registry.Changed += (field) =>
            {
                if (field == entityField2)
                    raisedEntityChanged = true;
            };

            bool raisedStringAdded = false;
            registry.FieldAdded += (field) =>
            {
                if (field == newString)
                    raisedStringAdded = true;
            };

            entityField2.AddField(newString);

            Assert.True(registry.GetField(newString.ID) != null);
            Assert.Contains(newString.ID, registry.FieldsDictionary);

            Assert.False(raisedEntityChanged);
            Assert.True(raisedStringAdded);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Реестр:");
            foreach(var field in registry.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");
                
                
                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if(field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            _output.WriteLine(stringBuilder.ToString());
        }

        [Fact]
        public void ExtractionTest()
        {
            FieldsContainer<EntityField> fieldsContainer1 = new FieldsContainer<EntityField>();

            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");

            entityField1.AddField(new StringField("SomeString", "someName", PlayerRole.All, PlayerRole.All, "Value"));
            entityField1.AddField(new IntField("SomeInt", "someName", PlayerRole.All, PlayerRole.All, 5));

            entityField2.AddField(new StringField("SomeOtherString", "someName", PlayerRole.All, PlayerRole.All, "Goool"));
            entityField2.AddField(new IntField("SomeOtherInt", "someName", PlayerRole.All, PlayerRole.All, 46));

            fieldsContainer1.AddField(entityField1);
            fieldsContainer1.AddField(entityField2);

            List<TreeState> states = FieldStateExtractor.ExtractFieldsCollectionTreeState(fieldsContainer1.Fields);

            Assert.Equal(2, states.Count);

            Assert.Contains(states, state => state.State.Name == "NewEntity1");
            Assert.Contains(states, state => state.State.Name == "NewEntity2");
        }
        
        
        [Fact]
        public void ReconstractionTest()
        {
            FieldsContainer<EntityField> fieldsContainer1 = new FieldsContainer<EntityField>();

            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");

            entityField1.AddField(new StringField("SomeString", "someName", PlayerRole.All, PlayerRole.All, "Value"));
            entityField1.AddField(new IntField("SomeInt", "someName", PlayerRole.All, PlayerRole.All, 5));

            entityField2.AddField(new StringField("SomeOtherString", "someName", PlayerRole.All, PlayerRole.All, "Goool"));
            entityField2.AddField(new IntField("SomeOtherInt", "someName", PlayerRole.All, PlayerRole.All, 46));

            fieldsContainer1.AddField(entityField1);
            fieldsContainer1.AddField(entityField2);

            List<TreeState> states = FieldStateExtractor.ExtractFieldsCollectionTreeState(fieldsContainer1.Fields);
            
            FieldsContainer<EntityField> fieldsContainer2 = new();

            EntityFactory entityFactory = new EntityFactory();
            entityFactory.Register<EntityField>();
            entityFactory.Register<StringField>();
            entityFactory.Register<IntField>();

            FieldsHierarchyReconstructor reconstructor = new(entityFactory);

            foreach(TreeState state in states)
                reconstructor.ApplyFieldState(state, fieldsContainer2);

            Assert.True(fieldsContainer1.List.Count == fieldsContainer2.List.Count);
            Assert.True(fieldsContainer2.Dictionary.ContainsKey(entityField1.ID));
            Assert.True(fieldsContainer2.Dictionary.ContainsKey(entityField2.ID));

            Assert.True(fieldsContainer2.Dictionary[entityField2.ID].Fields.Count == 2);
            Assert.True(fieldsContainer2.Dictionary[entityField1.ID].Fields.Count == 2);

            Assert.Contains(fieldsContainer2.Fields, field => field.ID == entityField1.ID);
            Assert.Contains(fieldsContainer2.Fields, field => field.ID == entityField2.ID);

            StringBuilder stringBuilder = new StringBuilder();

            FieldsRegistry fieldsRegistry1 = new FieldsRegistry(fieldsContainer1);
            FieldsRegistry fieldsRegistry2 = new FieldsRegistry(fieldsContainer2);

            stringBuilder.AppendLine("Реестр1:");
            foreach (var field in fieldsRegistry1.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("\nРеестр2:");
            foreach (var field in fieldsRegistry2.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            _output.WriteLine(stringBuilder.ToString());
        }

        [Fact] 
        public void UpdateTest()
        {
            FieldsContainer<EntityField> fieldsContainer1 = new FieldsContainer<EntityField>();

            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");

            entityField1.AddField(new StringField("SomeString", "someName", PlayerRole.All, PlayerRole.All, "Value"));
            entityField1.AddField(new IntField("SomeInt", "someName", PlayerRole.All, PlayerRole.All, 5));

            entityField2.AddField(new StringField("SomeOtherString", "someName", PlayerRole.All, PlayerRole.All, "Goool"));
            entityField2.AddField(new IntField("SomeOtherInt", "someName", PlayerRole.All, PlayerRole.All, 46));

            fieldsContainer1.AddField(entityField1);
            fieldsContainer1.AddField(entityField2);

            List<TreeState> states = FieldStateExtractor.ExtractFieldsCollectionTreeState(fieldsContainer1.Fields);

            FieldsContainer<EntityField> fieldsContainer2 = new();

            EntityFactory entityFactory = new EntityFactory();
            entityFactory.Register<EntityField>();
            entityFactory.Register<StringField>();
            entityFactory.Register<IntField>();

            FieldsHierarchyReconstructor reconstructor = new(entityFactory);

            foreach (TreeState state in states)
                reconstructor.ApplyFieldState(state, fieldsContainer2);

            FieldsRegistry registry1 = new FieldsRegistry(fieldsContainer1);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Реестр1 (до обновления):");
            foreach (var field in registry1.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            _output.WriteLine(stringBuilder.ToString());

            StringField newString = new StringField("BrandNewStringField", "someName", PlayerRole.All, PlayerRole.All, "SomeImportantData");
            TreeState fieldState = null;
            
            bool raisedAdded = false;
            registry1.ChildAdded += (field) =>
            {
                fieldState = FieldStateExtractor.ExtractFieldTreeState(field.Parent);
                reconstructor.ApplyFieldState(fieldState, fieldsContainer2);

                raisedAdded = true;
            };

            entityField2.AddField(newString);

            Assert.True(raisedAdded);
            Assert.True(fieldState != null);
            Assert.True(fieldState.State.ID == entityField2.ID);

            stringBuilder = new();

            FieldsRegistry fieldsRegistry2 = new FieldsRegistry(fieldsContainer2);

            stringBuilder.AppendLine("Реестр1:");
            foreach (var field in registry1.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("\nРеестр2:");
            foreach (var field in fieldsRegistry2.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine($"\n{fieldState.ParentID} - {fieldState.State.ToString()} - {fieldState.Children.Last().State.ToString()}");

            _output.WriteLine(stringBuilder.ToString());
        }

        [Fact]
        public void BlankUpdateTest()
        {
            FieldsContainer<EntityField> fieldsContainer1 = new FieldsContainer<EntityField>();

            EntityField entityField1 = new EntityField("NewEntity1");
            EntityField entityField2 = new EntityField("NewEntity2");

            entityField1.AddField(new StringField("SomeString", "someName", PlayerRole.All, PlayerRole.All, "Value"));
            entityField1.AddField(new IntField("SomeInt", "someName", PlayerRole.All, PlayerRole.All, 5));

            entityField2.AddField(new StringField("SomeOtherString", "someName", PlayerRole.All, PlayerRole.All, "Goool"));
            entityField2.AddField(new IntField("SomeOtherInt", "someName", PlayerRole.All, PlayerRole.All, 46));

            fieldsContainer1.AddField(entityField1);
            fieldsContainer1.AddField(entityField2);

            FieldsContainer<EntityField> fieldsContainer2 = new();

            EntityFactory entityFactory = new EntityFactory();
            entityFactory.Register<EntityField>();
            entityFactory.Register<StringField>();
            entityFactory.Register<IntField>();

            FieldsHierarchyReconstructor reconstructor = new(entityFactory);

            FieldsRegistry registry1 = new FieldsRegistry(fieldsContainer1);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Реестр1 (до обновления):");
            foreach (var field in registry1.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            _output.WriteLine(stringBuilder.ToString());

            StringField newString = new StringField("BrandNewStringField", "someName", PlayerRole.All, PlayerRole.All, "SomeImportantData");
            TreeState fieldState = null;

            bool raisedChildAdded = false;
            registry1.ChildAdded += (field) =>
            {
                fieldState = FieldStateExtractor.ExtractFieldTreeState(field.Parent);
                reconstructor.ApplyFieldState(fieldState, fieldsContainer2);

                raisedChildAdded = true;
            };

            entityField2.AddField(newString);

            Assert.True(raisedChildAdded);
            Assert.True(fieldState != null);
            Assert.True(fieldState.State.ID == entityField2.ID);

            Assert.Contains(fieldsContainer2.List, field => field.ID ==  entityField2.ID);

            stringBuilder = new();

            FieldsRegistry fieldsRegistry2 = new FieldsRegistry(fieldsContainer2);

            stringBuilder.AppendLine("Реестр1:");
            foreach (var field in registry1.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("\nРеестр2:");
            foreach (var field in fieldsRegistry2.FieldsDictionary)
            {
                if (field.Value.Parent != null)
                    stringBuilder.Append($"{field.Value.Parent.Name} /");


                stringBuilder.Append($"{field.Key} - {field.Value.Name}");

                if (field.Value is ValueField valueField)
                    stringBuilder.Append($" ({valueField.GetRawValue()})");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine($"\n{fieldState.ParentID} - {fieldState.State.ToString()} - {fieldState.Children.Last().State.ToString()}");

            _output.WriteLine(stringBuilder.ToString());
        }
    }
}
