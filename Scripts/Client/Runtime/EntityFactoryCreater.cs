using RollPunk.Entities;
using RollPunk.UIFields;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Scripts.Client.Runtime
{
    internal static class EntityFactoryCreater
    {
        public static EntityFactory Create()
        {
            var entityFactory = new EntityFactory();
            entityFactory.RegisterFields();
            entityFactory.RegisterHierarchyFields();
            entityFactory.RegisterLineFields();
            entityFactory.RegisterRules();

            return entityFactory;
        }
    }
}
