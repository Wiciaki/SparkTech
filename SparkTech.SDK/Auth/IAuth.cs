﻿namespace SparkTech.SDK.Auth
{
    using System.Threading.Tasks;

    public interface IAuth
    {
        Task<AuthResult> Auth(string product);
    }
}