﻿using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <see cref="IQueueBuilder" />
internal class QueueBuilder : IQueueBuilder
{
    private readonly QueueName _queueName;
    private bool _isDurable;
    private bool _isExclusive;
    private bool _isAutoDelete;
    private readonly List<Binding> _bindings = [];
    
    public QueueBuilder(QueueName queueName)
    {
        _queueName = queueName;
    }
    
    /// <see cref="IQueueBuilder.Durable" />
    public IQueueBuilder Durable()
    {
        _isDurable = true;
        return this;
    }
    
    /// <see cref="IQueueBuilder.Exclusive" />
    public IQueueBuilder Exclusive()
    {
        _isExclusive = true;
        return this;
    }
    
    /// <see cref="IQueueBuilder.AutoDelete" />
    public IQueueBuilder AutoDelete()
    {
        _isAutoDelete = true;
        return this;
    }
    
    /// <see cref="IQueueBuilder.Bind" />
    public IQueueBuilder Bind(
        string exchangeName,
        string routingKey,
        IDictionary<string, object>? arguments = null)
    {
        var name = new ExchangeName(exchangeName);
        return Bind(name, routingKey, arguments);
    }
    
    /// <see cref="IQueueBuilder.BindDefault" />
    public IQueueBuilder BindDefault(
        string exchangeName,
        IDictionary<string, object>? arguments = null)
    {
        return Bind(exchangeName, routingKey: _queueName.Value, arguments);
    }
    
    /// <see cref="IQueueBuilder.BindToAmqDirect" />
    public IQueueBuilder BindToAmqDirect(
        string routingKey,
        IDictionary<string, object>? arguments = null)
    {
        return Bind(Exchange.AmqDirect.Name, routingKey, arguments);
    }
    
    /// <see cref="IQueueBuilder.BindToAmqDirectDefault" />
    public IQueueBuilder BindToAmqDirectDefault(IDictionary<string, object>? arguments = null) =>
        BindToAmqDirect(_queueName.Value, arguments);
    
    /// <see cref="IQueueBuilder.BindToFanoutDirect" />
    public IQueueBuilder BindToFanoutDirect(IDictionary<string, object>? arguments = null) =>
        Bind(Exchange.AmqFanout.Name, arguments: arguments);
    
    /// <see cref="IQueueBuilder.BindToFanoutDirectDefault" />
    public IQueueBuilder BindToFanoutDirectDefault(IDictionary<string, object>? arguments = null) =>
        BindToFanoutDirect(arguments);
    
    /// <see cref="IQueueBuilder.BindToAmqTopic" />
    public IQueueBuilder BindToAmqTopic(
        string routingKey,
        IDictionary<string, object>? arguments = null)
    {
        return Bind(Exchange.AmqTopic.Name, routingKey, arguments);
    }
    
    /// <see cref="IQueueBuilder.BindToAmqTopicDefault" />
    public IQueueBuilder BindToAmqTopicDefault(IDictionary<string, object>? arguments = null) =>
        BindToAmqTopic(_queueName.Value, arguments);
    
    private QueueBuilder Bind(
        ExchangeName exchangeName,
        string? routingKey = null,
        IDictionary<string, object>? arguments = null)
    {
        var key = new RoutingKey(routingKey);
        
        _bindings.Add(new Binding(exchangeName, key, arguments));
        
        return this;
    }

    public Queue Build()
    {
        return new Queue(
            _queueName,
            _isDurable,
            _isExclusive,
            _isAutoDelete,
            _bindings);
    }
}