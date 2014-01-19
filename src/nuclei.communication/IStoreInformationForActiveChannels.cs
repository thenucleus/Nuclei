using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclei.Communication
{
    internal interface IStoreInformationForActiveChannels
    {
        IEnumerable<ChannelInformation> ActiveChannels();
    }
}
