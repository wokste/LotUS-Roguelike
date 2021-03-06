﻿namespace SurvivalHack.ECM
{
    public class StackComponent : IComponent
    {
        public int Count;
        public object Prototype;

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

        public string Describe() => $"Stack consists of {Count} items";

        public bool FitsIn(ESlotType type) => false;
    }
}
