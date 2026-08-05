using System.Security.Cryptography;
using System.Text;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations;

public class SigningHandler(ISigningConfiguration config) : DelegatingHandler
{
    private readonly ISigningConfiguration _config = config;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        string? body = null;
        if (request.Content != null)
        {
            body = await request.Content.ReadAsStringAsync(cancellationToken);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        var pathAndQuery = request.RequestUri?.PathAndQuery ?? "/";
        var messageToSign = $"{request.Method}:{pathAndQuery}:{timestamp}:{body ?? string.Empty}";

        using var rsa = RSA.Create();
        rsa.ImportFromPem(_config.PrivateKeyPem);

        var signatureBytes = rsa.SignData
        (
            Encoding.UTF8.GetBytes(messageToSign),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        request.Headers.Add("X-Timestamp", timestamp);
        request.Headers.Add("X-Signature", Convert.ToBase64String(signatureBytes));

        return await base.SendAsync(request, cancellationToken);
    }
}