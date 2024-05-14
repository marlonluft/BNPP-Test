using ClassLibrary1.Core.Crosscutting;

namespace ClassLibrary1.Core.Interfaces
{
    public interface IFinancialInstrumentService
    {
        Result ProcessFinancialInstruments(IEnumerable<string> ISINCodes, CancellationToken cancellationToken);
    }
}
