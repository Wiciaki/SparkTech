namespace SparkTech.SDK.EventData
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class IssueOrderEventArgs : BlockableEventArgs, IEventArgsSource<IUnit>, IEventArgsTarget<IGameObject>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public GameObjectOrder Order { get; }

        public Vector3 TargetPosition { get; }

        public int TargetId { get; }

        public IGameObject Target => ObjectManager.GetById(this.TargetId);

        public bool IsAttackMove { get; }

        public bool IsPetCommand { get; }

        public IssueOrderEventArgs(int sourceId, GameObjectOrder order, Vector3 targetPosition, int targetId, bool isAttackMove, bool isPetCommand)
        {
            this.SourceId = sourceId;
            this.Order = order;
            this.TargetPosition = targetPosition;
            this.TargetId = targetId;
            this.IsAttackMove = isAttackMove;
            this.IsPetCommand = isPetCommand;
        }
    }
}