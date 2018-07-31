using System;

namespace EntityCache
{
    public class EntityArgs : EventArgs
    {
        public EntityActions Action { get; }
        public int EntityId { get; }

        public EntityArgs(int entityId, EntityActions action)
        {
            EntityId = entityId;
            Action = action;
        }

    }
}

