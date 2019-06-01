using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HackConsole
{
    public struct ColoredString
    {
        public string Text;
        public Colour Color;
        public int Length => Text.Length;

        public static Colour[] colors = new Colour[] { Colour.White, Colour.Red, Colour.Blue, Colour.Green, Colour.Gray, Colour.Cyan, Colour.Orange, Colour.Pink };

        private ColoredString(string text, Colour color)
        {
            Text = text;
            Color = color;
        }

        public static void Write(string text, Colour color)
        {
            OnMessage?.Invoke(new ColoredString(text, color));
        }

        public Symbol this[int index] {
            get {
                return new Symbol(Text[index], Color, Colour.Transparent);
            }
        }

        public static Action<ColoredString> OnMessage;

        internal IEnumerable<(char, Colour)> Iterate()
        {
            var state = STATE_TEXT;
            Colour color = Colour.White;

            foreach (var c in Text)
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
