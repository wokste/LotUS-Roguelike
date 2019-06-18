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

        public void Use(Entity instignator, Entity target, StringBuilder sb, TargetFilter filter)
        {
            if (Effects == null)
                return;

            foreach (var e in Effects)
            {
                if ((e.Filter & filter) != 0)
                    e.Use(instignator, target, sb);
            }
        }

        public float Efficiency(Entity instignator, Entity target, TargetFilter filter)
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
        TargetFilter Filter { get; set; }

        void Use(Entity instignator, Entity target, StringBuilder sb);
        float Efficiency(Entity instignator, Entity target);
    }

    [Flags]
    public enum TargetFilter
    {
        Self = 0x1,
        Item = 0x2,
        Map = 0x4,

        // TODO: Implement this
        FlagHit = 0x100,
        FlagKill = 0x200,

        // TODO: Implement this
        FlagCursed = 0x1000,
        FlagNoncursed = 0x2000,
        FlagBlessed = 0x4000,
        FlagNonblesed = 0x8000,

        // TODO: Implement this
        TargetInstinator = 0x10000,
        TargetWeapon = 0x20000,
        TargetRandomItem = 0x40000,
        TargetAllItems = 0x80000,
    }
}
