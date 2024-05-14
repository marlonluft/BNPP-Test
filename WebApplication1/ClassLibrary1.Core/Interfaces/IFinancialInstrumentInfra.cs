using ClassLibrary1.Core.Crosscutting;
using ClassLibrary1.Core.Model;

namespace ClassLibrary1.Core.Interfaces
{
    public interface IFinancialInstrumentInfra
    {
        Task<Result> Store(FinancialInstrument financialInstrument, CancellationToken cancellationToken);
    }
}
