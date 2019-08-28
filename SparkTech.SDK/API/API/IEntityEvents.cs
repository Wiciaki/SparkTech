namespace SparkTech.SDK.Platform.API
{
    using System;

    using SparkTech.SDK.Entities;

    public interface IEntityEvents
    {
        Action<OnDamageEventArgs> Damage { get; set; }

        //Action<IAttackableUnit> OnLeaveVisiblityClient { get; set; }
        
        //OnLeaveTeamVisiblity
        // OnLeaveLocalVisiblityClient
        // ^x2 for enter
    }
}