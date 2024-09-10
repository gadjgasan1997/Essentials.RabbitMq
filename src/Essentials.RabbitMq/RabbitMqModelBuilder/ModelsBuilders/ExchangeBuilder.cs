using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <see cref="IExchangeBuilder" />
internal class ExchangeBuilder : IExchangeBuilder
{
    private readonly ExchangeName _exchangeName;
    private readonly ExchangeType _exchangeType;
    
    private bool _isDurable;
    private bool _isAutoDelete;
    
    public ExchangeBuilder(ExchangeName exchangeName, ExchangeType exchangeType)
    {
        _exchangeName = exchangeName;
        _exchangeType = exchangeType;
    }
    
    /// <see cref="IExchangeBuilder.Durable" />
    public IExchangeBuilder Durable()
    {
        _isDurable = true;
        return this;
    }
    
    /// <see cref="IExchangeBuilder.AutoDelete" />
    public IExchangeBuilder AutoDelete()
    {
        _isAutoDelete = true;
        return this;
    }

    internal Exchange Build()
    {
        return new Exchange(
            _exchangeName,
            _exchangeType,
            _isDurable,
            _isAutoDelete);
    }
}