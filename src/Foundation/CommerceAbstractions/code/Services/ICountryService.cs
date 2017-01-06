using System.Collections.Generic;
using Valtech.Foundation.CommerceAbstractions.Models;

namespace Valtech.Foundation.CommerceAbstractions.Services
{
    public interface ICountryService
    {
		IEnumerable<Country> GetAvailableCountries();
	}
}
