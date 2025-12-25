using Blade.Core;
using Code.Blocks;

namespace Code.Events
{
    public class BlockEvent
    {
        public static DestroyBlockEvent DestroyBlockEvent = new DestroyBlockEvent();
    }

    public class DestroyBlockEvent : GameEvent
    {
        public Block block;

        public DestroyBlockEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
}