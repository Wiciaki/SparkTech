namespace SparkTech.SDK.API.Fragments
{
    using System;

    using SparkTech.SDK.Entities.Events;

    public interface IEntityEvents
    {
        Action<OnDamageEventArgs> Damage { get; set; }

        //Action<IAttackableUnit> OnLeaveVisiblityClient { get; set; }
        
        //OnLeaveTeamVisiblity
        // OnLeaveLocalVisiblityClient
        // ^x2 for enter
    }
}