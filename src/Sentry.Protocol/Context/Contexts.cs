#if !LACKS_CONCURRENT_COLLECTIONS
using System.Collections.Concurrent;
#else
using System.Collections.Generic;
#endif
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace Sentry.Protocol
{
    /// <summary>
    /// Represents Sentry's structured Context
    /// </summary>
    /// <inheritdoc />
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/" />
    [DataContract]
    public class Contexts :
#if !LACKS_CONCURRENT_COLLECTIONS
        ConcurrentDictionary<string, object>
#else
        Dictionary<string, object>
#endif
    {
        /// <summary>
        /// Describes the application.
        /// </summary>
        public App App => this.GetOrCreate<App>(App.Type);
        /// <summary>
        /// Describes the browser.
        /// </summary>
        public Browser Browser => this.GetOrCreate<Browser>(Browser.Type);
        /// <summary>
        /// Describes the device.
        /// </summary>
        public Device Device => this.GetOrCreate<Device>(Device.Type);
        /// <summary>
        /// Defines the operating system.
        /// </summary>
        /// <remarks>
        /// In web contexts, this is the operating system of the browser (normally pulled from the User-Agent string).
        /// </remarks>
        public OperatingSystem OperatingSystem => this.GetOrCreate<OperatingSystem>(OperatingSystem.Type);
        /// <summary>
        /// This describes a runtime in more detail.
        /// </summary>
        public Runtime Runtime => this.GetOrCreate<Runtime>(Runtime.Type);

        /// <summary>
        /// Creates a deep clone of this context
        /// </summary>
        /// <returns></returns>
        internal Contexts Clone()
        {
            var context = new Contexts();

            CopyTo(context);

            return context;
        }

        /// <summary>
        /// Copies the items of the context while cloning the known types
        /// </summary>
        /// <param name="to">To.</param>
        internal void CopyTo(Contexts to)
        {
            if (to == null)
            {
                return;
            }

            foreach (var kv in this)
            {
                object value;
                switch (kv.Key)
                {
                    case App.Type:
                        value = (kv.Value as App)?.Clone();
                        break;
                    case Browser.Type:
                        value = (kv.Value as Browser)?.Clone();
                        break;
                    case Device.Type:
                        value = (kv.Value as Device)?.Clone();
                        break;
                    case OperatingSystem.Type:
                        value = (kv.Value as OperatingSystem)?.Clone();
                        break;
                    case Runtime.Type:
                        value = (kv.Value as Runtime)?.Clone();
                        break;

                    default:
                        value = kv.Value;
                        break;
                }
#if !LACKS_CONCURRENT_COLLECTIONS
                to.TryAdd(kv.Key, value);
#else
                to.Add(kv.Key, value);
#endif
            }
        }
    }
}
