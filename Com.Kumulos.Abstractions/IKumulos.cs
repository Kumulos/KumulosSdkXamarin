using System;
namespace Com.Kumulos.Abstractions
{
    public interface IKumulos
    {
        void Initialize(IKSConfig config);

        string GetInstallId();
    }
}
