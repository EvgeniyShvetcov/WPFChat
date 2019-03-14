using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Helpers
{
    public enum MessageTypes : int
    {
        Other = 0,
        UserMoving = 1,
        Self = 2,
        HistoryMessage = 3
    }
}
