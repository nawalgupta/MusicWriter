using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class IOListener
    {
        readonly IOMessage filter;
        readonly Action<IOMessage> responder;

        public IOMessage Filter {
            get { return filter; }
        }

        public Action<IOMessage> Responder {
            get { return responder; }
        }

        public IOListener(
                IOMessage filter,
                Action<IOMessage> responder
            ) {
            this.filter = filter;
            this.responder = responder;
        }

        public void Process(IOMessage message) {
            if (message.Matches(filter))
                responder(message);
        }
    }
}
