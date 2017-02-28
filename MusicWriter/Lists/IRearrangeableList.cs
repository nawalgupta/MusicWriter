using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IRearrangeableList
    {
        void Move(int oldindex, int newindex);
    }
}
