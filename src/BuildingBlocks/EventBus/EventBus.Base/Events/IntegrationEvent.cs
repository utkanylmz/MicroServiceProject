using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        // IntegrationEvent : Sistemizde AzureServiceBus veya RabbitMQ aracılığı ile diğer servislere haber ulaştıran Eventlerdir.
        //IntegrationEvent mimarileride servisler arası iletişimde kullanılacak olan classlar/objeler olarak düşünebiliriz.

       
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }

        // Json kullanarak bir serilaze ve deserilaze işlemi set edildiği zaman  Id ve CreatedDate'i constratordan alıp set edelim diye 
        //Newtonsoft.json paketinden gelen JsonConstructor,JsonProperty attributelerini kullandık.
        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id = id;
            CreatedDate = createdDate;
        }

        [JsonProperty]
        public Guid Id { get;private set; }
        public DateTime CreatedDate { get; private set; }
        
    }
}
