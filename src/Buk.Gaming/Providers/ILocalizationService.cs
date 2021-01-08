using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface ILocalizationService
    {
        Task<object> GetJsonAsync(string lang, string module = "");
    }
}
