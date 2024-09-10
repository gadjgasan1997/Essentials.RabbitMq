using RabbitMQ.Client;
using System.Net.Security;
using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using Essentials.RabbitMq.Options;
using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.RabbitMqConnections.Implementations;

/// <summary>
/// Сервис для работы с криптографией
/// </summary>
internal class CryptoService
{
    private readonly ILogger _logger;
    
    public CryptoService(ILogger logger)
    {
        _logger = logger.CheckNotNull();
    }
    
    /// <summary>
    /// Настраивает Ssl
    /// </summary>
    /// <param name="connectionFactory"></param>
    /// <param name="sslOptions"></param>
    public void ConfigureSsl(
        ConnectionFactory connectionFactory,
        SslOptions sslOptions)
    {
        if (!sslOptions.Enable)
            return;
        
        connectionFactory.Ssl.Enabled = true;
        connectionFactory.Ssl.ServerName = sslOptions.SslServerName;
        connectionFactory.Ssl.Version = SslProtocols.Tls12;
        connectionFactory.Ssl.CertificateValidationCallback = (_, _, _, _) => true;
        
        if (string.IsNullOrWhiteSpace(sslOptions.CertPath))
            return;
        
        connectionFactory.Ssl.CertPath = sslOptions.CertPath;
        connectionFactory.Ssl.AcceptablePolicyErrors |=
            SslPolicyErrors.RemoteCertificateNameMismatch |
            SslPolicyErrors.RemoteCertificateChainErrors;
        
        if (string.IsNullOrWhiteSpace(sslOptions.CertPassphrase))
        {
            _logger.LogError("Для сертификата по пути '{path}' не задан пароль", sslOptions.CertPath);
            return;
        }
        
        connectionFactory.Ssl.CertPassphrase = sslOptions.CertPassphrase;
    }
}