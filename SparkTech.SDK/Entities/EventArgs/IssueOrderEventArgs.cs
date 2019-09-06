namespace SparkTech.SDK.Entities.EventArgs
{
    using SharpDX;

    public class IssueOrderEventArgs : BlockableEventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly GameObjectOrder Order;

        public readonly Vector3 TargetPosition;

        public readonly IGameObject Target;

        public readonly bool IsAttackMove;

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