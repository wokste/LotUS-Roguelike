using System;
using System.Collections.Generic;

namespace HackConsole
{
    public class MessageListWidget : TextWidget
    {
        private readonly List<Message> _messages = new List<Message>();

        public void AddMessage(Message msg)
        {
            _messages.Add(msg);
            PosY += WordWrap(msg.Text, "> ", msg.Color);
            Dirty = true;
        }

        protected override void MakeLines()
        {
            Lines.Clear();
            foreach (var msg in _messages)
                WordWrap(msg.Text, "> ", msg.Color);
        }
    }

    public struct Message
    {
        public string Text;
        public Vec Pos; // Where the message comes from
        public Color Color;

        private Message(string text, Vec pos, Color color)
        {
            Text = text;
            Pos = pos;
            Color = color;
        }

        public static void Write(string text, Vec pos, Color color)
        {
            OnMessage?.Invoke(new Message(text, pos, color));
        }

        public static Action<Message> OnMessage;
    }
}
