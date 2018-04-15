using SurvivalHack.ECM;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack.Mapgen
{
    class Prototypes
    {
        private readonly Dictionary<string, Entity> _prototypes = new Dictionary<string, Entity>();

        public Entity Make(string name) {
            throw new NotImplementedException("Cloning of entities needed for this feature");
        }

        public void Load(string filename) {
            var doc = new XmlDocument();
            doc.Load(filename);

            var root = doc.LastChild;
            foreach (XmlNode child in root.ChildNodes)
            {
                var tag = child.Attributes["tag"].Value;

                var prototype = LoadEntity(child);
                _prototypes.Add(tag, prototype);
            }
        }

        private Entity LoadEntity(XmlNode elem) {
            var entity = new Entity
            {
                Name = elem.Attributes["name"].Value,
                Description = elem.Attributes["decription"].Value,
            };

            foreach (XmlNode child in elem)
            {
                switch (child.Name) {
                    case "attack":
                        Debug.Assert(entity.Attack == null);
                        entity.Attack = LoadAttack(child);
                        break;
                    case "attitude":
                        Debug.Assert(entity.Attack == null);
                        throw new NotImplementedException();
                        break;
                    case "move":
                        throw new NotImplementedException();
                        //entity.Move = LoadMove(child);
                        break;
                    case "symbol":
                        entity.Symbol = LoadSymbol(child);
                        break;
                }
            }
            return entity;
        }

        private AttackComponent LoadAttack(XmlNode child)
        {
            throw new NotImplementedException();
        }

        private Symbol LoadSymbol(XmlNode child)
        {
            return new Symbol
            {
                Ascii = child.Attributes["char"].Value[0],
                TextColor = Color.TryParse(child.Attributes["color"]?.Value) ?? Color.White,
                BackgroundColor = Color.TryParse(child.Attributes["bg_color"]?.Value) ?? Color.Black,
            };
        }
    }
}
