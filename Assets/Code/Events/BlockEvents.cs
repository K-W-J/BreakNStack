using Code.Blocks;
using Code.Core;

namespace Code.Events
{
    public class BlockEvents
    {
        public static BlockPushEvent BlockPushEvent = new BlockPushEvent();
        public static BlockLandEvent BlockLandEvent = new BlockLandEvent();
        public static BlockSpawnEvent BlockSpawnEvent = new BlockSpawnEvent();
        public static BlockMoveEvent BlockMoveEvent = new BlockMoveEvent();
        public static BlockDropEvent BlockDropEvent = new BlockDropEvent();
    }
    
    public class BlockMoveEvent : GameEvent
    {
        public Block block;

        public BlockMoveEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }

    public class BlockPushEvent : GameEvent
    {
        public Block block;

        public BlockPushEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
    
    public class BlockSpawnEvent : GameEvent
    {
        public Block block;

        public BlockSpawnEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
    
    public class BlockLandEvent : GameEvent
    {
        public Block block;
        
        public BlockLandEvent Initialize(Block block)
        {
            this.block = block;
            return this;
        }
    }
    
    public class BlockDropEvent : GameEvent
    {
        public BlockDropEvent Initialize()
        {
            return this;
        }
    }
}