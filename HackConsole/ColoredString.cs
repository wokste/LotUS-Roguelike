using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HackConsole
{
    public static class ColoredString
    {
        /*
        Color coding:
            @ca	White    Plain text
            @cb	Blue     Monsters / Traps
            @cc Cyan     Status Effects
            @cd	Red      Damage / Cursed Items / Monsters
            @ce Purple   Spells / Artifacts / MP
            @cf Gray     Mundane Items, stairs, etc.
            @cg	Green    Healing
            @ch Yellow   Consumable Items / Minor enchantments
        */

        public static Color[] colors = new Color[] { new Color(128, 128, 128), Color.Blue, Color.Cyan, Color.Red, Color.Magenta, Color.Cyan, Color.Green, Color.Yellow };

        public static void Write(string text)
        {
            OnMessage?.Invoke(text);
        }

        public static Action<string> OnMessage;

        public static IEnumerable<(char, Color)> Iterate(this string text)
        {
            var state = STATE_TEXT;
            Color color = colors[0];

            foreach (var c in text)
            {
                switch (state)
                {
                    case STATE_TEXT:
                        {
                            if (c == '@')
                                state = STATE_CMD;
                            else
                                yield return (c, color);
                            
                            break;
                        }
                    case STATE_CMD:
                        {
                            switch (c)
                            {
                                case 'c':
                                    {
                                        state = STATE_CMD_COLOR;
                                        break;
                                    }
                                default:
                                    Debug.Assert(false);
                                    state = STATE_TEXT;
                                    break;
                            }
                            break;
                        }
                    case STATE_CMD_COLOR:
                        {
                            var colorId = c - 'a';
                            if (colorId >= 0 && colorId < colors.Length)
                            {
                                color = colors[colorId];
                            }
                            else
                            {
                                Debug.Assert(false);
                            }

                            state = STATE_TEXT;
                            break;
                        }
                    default:
                        Debug.Assert(false);
                        state = STATE_TEXT;
                        break;
                }
            }
        }

        private const int STATE_TEXT = 0;
        private const int STATE_CMD = 1;
        private const int STATE_CMD_COLOR = 2;
    }
}
