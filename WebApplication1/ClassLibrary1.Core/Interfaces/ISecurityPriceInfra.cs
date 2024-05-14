using ClassLibrary1.Core.Crosscutting;

namespace ClassLibrary1.Core.Interfaces
{
    public interface ISecurityPriceInfra
    {
        Task<Result<decimal>> GetPriceByISIN(string ISIN, CancellationToken cancellationToken);
    }
}
