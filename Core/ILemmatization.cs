using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public interface ILemmatization
    {
        Dictionary<string, int> GetDictFromLemms(string text);
    }

}
