using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;

namespace Adnc.Infra.EventBus.Cap
{
    public class CapPublisher : IEventPublisher
    {
        private readonly ICapPublisher _eventBus;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CapPublisher(ICapPublisher capPublisher,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventBus = capPublisher;
            this._httpContextAccessor = httpContextAccessor;
        }

        public virtual async Task PublishAsync<T>(T eventObj, string? callbackName = null, CancellationToken cancellationToken = default)
            where T : class
        {
            var requestHeader = _httpContextAccessor?.HttpContext?.Request?.Headers;
            if (requestHeader != null)
            {
                Dictionary<string, string> capHeaders = new Dictionary<string, string>();
                foreach (var header in requestHeader)
                {
                    capHeaders.Add(header.Key, header.Value);
                }
                if (capHeaders.IsNotNullOrEmpty())
                {
                    await _eventBus.PublishAsync(typeof(T).Name, eventObj, capHeaders!, cancellationToken);
                    return;
                }
            }

            await _eventBus.PublishAsync(typeof(T).Name, eventObj, callbackName, cancellationToken);
        }

        public virtual async Task PublishAsync<T>(T eventObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
            where T : class
            => await _eventBus.PublishAsync<T>(typeof(T).Name, eventObj, headers, cancellationToken);

        public virtual void Publish<T>(T eventObj, string? callbackName = null)
            where T : class
            => _eventBus.Publish(typeof(T).Name, eventObj, callbackName);

        public virtual void Publish<T>(T eventObj, IDictionary<string, string?> headers)
            where T : class
            => _eventBus.Publish(typeof(T).Name, eventObj, headers);
    }
}