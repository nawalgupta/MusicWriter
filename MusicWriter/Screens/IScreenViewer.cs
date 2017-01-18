using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public interface IScreenViewer<View>
    {
        bool IsCompatibleWithProduceOf(IScreenFactory<View> factory);

        View CreateView(IScreen<View> screen);
    }
}
