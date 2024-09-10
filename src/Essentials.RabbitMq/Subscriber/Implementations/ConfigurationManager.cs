using LanguageExt;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IConfigurationManager" />
internal class ConfigurationManager : IConfigurationManager
{
    private static readonly ReaderWriterLockSlim _readerWriterLock = new();
    private static readonly List<Type> _subscribedTypes = [];
    private static readonly Dictionary<InternalSubscriptionKey, SubscriptionOptions> _subscriptionsOptionsMap = [];
    
    /// <inheritdoc cref="IConfigurationManager.SaveSubscriptionOptions{TEvent}" />
    public Try<Unit> SaveSubscriptionOptions<TEvent>(InternalSubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent
    {
        return () =>
        {
            var eventType = typeof(TEvent);
            
            using var upgradeableReadLocker = _readerWriterLock.UseUpgradeableReadLock();
            if (_subscribedTypes.Contains(eventType))
            {
                throw new InvalidOperationException(
                    $"Для события с типом '{eventType.FullName}' уже зарегистрирован обработчик. " +
                    "Невозможно зарегистрировать два обработчика одного и того же события");
            }
            
            if (_subscriptionsOptionsMap.ContainsKey(key))
            {
                throw new InvalidOperationException(
                    $"Для события с ключом '{key}' уже зарегистрирован обработчик. " +
                    "Невозможно зарегистрировать два обработчика по одному и тому же ключу");
            }
            
            using var writeLocker = _readerWriterLock.UseWriteLock();
            
            _subscribedTypes.Add(eventType);
            _subscriptionsOptionsMap.Add(key, options);
            
            return Unit.Default;
        };
    }
    
    /// <inheritdoc cref="IConfigurationManager.GetSubscriptionOptionsByKey" />
    public Try<SubscriptionOptions> GetSubscriptionOptionsByKey(InternalSubscriptionKey key)
    {
        return () =>
        {
            using var locker = _readerWriterLock.UseReadLock();
            
            if (!_subscriptionsOptionsMap.TryGetValue(key, out var options))
                throw new KeyNotFoundException($"Не найдены опции обработки события с ключом '{key}'");
            
            return options;
        };
    }
}