using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientAppRemote
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ServerThreadInterfaceImpl : ServerThreadInterface
    {
        
         public ServerThreadInterfaceImpl()
         {
         }
        
    }
}
