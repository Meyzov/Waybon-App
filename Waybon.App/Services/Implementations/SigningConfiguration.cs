using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class SigningConfiguration : ISigningConfiguration
    {
        public string PrivateKeyPem => SecurityConfig.PrivateKeyPem;
    }
}