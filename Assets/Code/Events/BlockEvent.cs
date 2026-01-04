using Blade.Core;
using Code.Blocks;

namespace Code.Events
{
    public class BlockEvent
    {
        public static PushBlockEvent PushBlockEvent = new PushBlockEvent();
        public static CoundBlockEvent CoundBlockEvent = new CoundBlockEvent();
        public static SpawnBlockEvent SpawnBlockEvent = new SpawnBlockEvent();
    }

    public class PushBlockEvent : GameEvent
    {
        public Block block;

        public PushBlockEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
    
    public class SpawnBlockEvent : GameEvent
    {
        public Block block;

        public SpawnBlockEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
    
    public class CoundBlockEvent : GameEvent
    {
        public int count;

        public CoundBlockEvent Initialize(int count)
        {
            this.count = count;
            return this;
        }
    }
}