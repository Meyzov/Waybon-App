using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class SigningConfiguration : ISigningConfiguration
    {
        public string PrivateKeyPem
        {
            get => SecurityConfig.PrivateKeyPem;
        }
    }
}