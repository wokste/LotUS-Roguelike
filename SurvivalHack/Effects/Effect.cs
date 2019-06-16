using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    public struct EffectList : IXmlSerializable
    {
        public IEffect[] Effects;

        public EffectList(IEffect effect)
        {
            Effects = new[] { effect };
        }

        public void Use(Entity instignator, Entity target, StringBuilder sb, EntityTarget filter)
        {
            if (Effects == null)
                return;

            foreach (var e in Effects)
            {
                if ((e.UseOn & filter) != 0)
                    e.Use(instignator, target, sb);
            }
        }

        public float Efficiency(Entity instignator, Entity target, EntityTarget filter)
        {
            if (Effects == null)
                return 0;

            // TODO: Check whether something is usable
            return Effects.Average(e => e.Efficiency(instignator, target));
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            // TODO: XML
            throw new NotImplementedException();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (Effects == null)
                return;

            foreach (var e in Effects)
            {
                XmlSerializer serial = new XmlSerializer(e.GetType());
                serial.Serialize(writer, e);
            }
            
        }
    }

    [XmlInclude(typeof(ApplyStatusEffect))]
    [XmlInclude(typeof(HarmEffect))]
    [XmlInclude(typeof(HealEffect))]
    [XmlInclude(typeof(MapRevealEffect))]
    [XmlInclude(typeof(TeleportEffect))]
    public interface IEffect
    {
        [XmlAttribute]
        EntityTarget UseOn { get; set; }

        void Use(Entity instignator, Entity target, StringBuilder sb);
        float Efficiency(Entity instignator, Entity target);
    }

    [Flags]
    public enum EntityTarget
    {
        Self = 1,
        Inventory = 2,
        Others = 4,
        MultiTarget = 8,
    }
}
