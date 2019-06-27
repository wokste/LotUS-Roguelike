using System.Xml.Serialization;

namespace SurvivalHack.ECM
{
    public class StackComponent : IActionComponent
    {
        public int Count;
        [XmlIgnore]
        public object Prototype;

        public StackComponent()
        {
        }

        public StackComponent(int count, object prototype)
        {
            Count = count;
            Prototype = prototype;
        }

        private void Consume(BaseEvent msg)
        {
            Count--;
            if (Count <= 0)
            {
                msg.User?.GetOne<Inventory>()?.Remove(msg.Item);
                msg.Item.Destroy();
            }
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source) {
            if (source == EUseSource.Item)
            {
                if (message is ConsumeEvent) // TODO, others
                {
                    message.PostEvent += Consume;
                }
            }
        }
    }
}
