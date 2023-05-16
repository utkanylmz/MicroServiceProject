using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; set; }
        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ;
        }

       
        // Dışarıdan gelen verilerin içerde tutulması için kullanıcaz.Bize Gönderilen IntegrationEvent'in typını burada tutacaz ve daha sonra
        // o type'a ulaşıp.Handler metpdonu çağıracağız.
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}
