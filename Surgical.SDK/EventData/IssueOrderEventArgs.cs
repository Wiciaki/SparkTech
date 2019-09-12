namespace Surgical.SDK.EventData
{
    using SharpDX;

    using Surgical.SDK.Entities;

    public class IssueOrderEventArgs : BlockableEventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public GameObjectOrder Order { get; }

        public Vector3 TargetPosition { get; }

        public IGameObject Target { get; }

        public bool IsAttackMove { get; }

        public IssueOrderEventArgs(IUnit source, GameObjectOrder order, Vector3 targetPosition, IGameObject target, bool isAttackMove)
        {
            this.Source = source;

            this.Order = order;

            this.TargetPosition = targetPosition;

            this.Target = target;

            this.IsAttackMove = isAttackMove;
        }
    }
}