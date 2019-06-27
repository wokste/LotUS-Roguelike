using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SurvivalHack.ECM
{

    [XmlInclude(typeof(Combat.Blockable))]
    [XmlInclude(typeof(Combat.Armor))]
    [XmlInclude(typeof(Combat.ElementalResistance))]
    [XmlInclude(typeof(Combat.StatBlock))]
    [XmlInclude(typeof(Combat.MeleeWeapon))]
    [XmlInclude(typeof(Combat.SweepWeapon))]
    [XmlInclude(typeof(Combat.RangedWeapon))]

    [XmlInclude(typeof(FieldOfView))]
    [XmlInclude(typeof(StackComponent))]
    [XmlInclude(typeof(Inventory))]
    [XmlInclude(typeof(Potion))]
    [XmlInclude(typeof(Stairs))]
    public interface IComponent
    {
    }

    public interface INoSerialize {
    }

    public class ComponentList : List<IComponent> , IXmlSerializable
    {
        public T GetOne<T>() where T : class, IComponent
        {
            return Get<T>().FirstOrDefault();
        }

        public IEnumerable<T> Get<T>() where T : class, IComponent
        {
            foreach (var c in this)
                if (c is T cT)
                    yield return cT;
        }

        public void GetNested<T>(IList<T> list) where T : class, IComponent
        {
            foreach (var c in this)
            {
                if (c is T cT)
                    list.Add(cT);

                if (c is INestedComponent cNested)
                    cNested.GetNested(list);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var c in this)
            {
                if (c is INoSerialize)
                    continue;
                var serializer = new XmlSerializer(c.GetType());
                serializer.Serialize(writer, c);
            }

        }
    }

    public interface IActionComponent : IComponent
    {
        void GetActions(Entity self, BaseEvent message, EUseSource source);
    }

    public interface INestedComponent : IComponent
    {
        void GetNested<T>(IList<T> list) where T : class, IComponent;      
    }

    public interface IEquippableComponent : IComponent
    {
        ESlotType SlotType { get; }
    }

    public enum EUseSource
    {
        Item,
    }
}
