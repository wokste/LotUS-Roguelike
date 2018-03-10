using System;
using System.Collections.Generic;

namespace HackConsole
{
    public class MessageListWidget : TextWidget
    {
        private readonly List<String> _messages = new List<string>();

        public void AddMessage(string msg)
        {
            _messages.Add(msg);
            WordWrap(msg, "> ");
            _dirty = true;
        }

        protected override void MakeLines()
        {
            foreach (var msg in _messages)
                WordWrap(msg, "> ");
        }
    }
}
