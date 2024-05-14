using ClassLibrary1.Core.Crosscutting;
using ClassLibrary1.Core.Interfaces;
using ClassLibrary1.Core.Model;

namespace ClassLibrary1.Core
{
    /// <summary>
    /// https://docs.google.com/document/d/17r6bwZwA-4XSWQ4dhFjwZgQp4f_oqTnfMNGh0NBDbuw/edit
    /// </summary>
    public class FinancialInstrumentService : IFinancialInstrumentService
    {
        private const int ISINLength = 12;

        private readonly ISecurityPriceInfra _securityPriceInfra;
        private readonly IFinancialInstrumentInfra _financialInstrumentInfra;
        private readonly ILog _logger;

        public FinancialInstrumentService(
            ISecurityPriceInfra securityPriceService,
            IFinancialInstrumentInfra financialInstrumentInfra,
            ILog logger)
        {
            _logger = logger;
            _securityPriceInfra = securityPriceService;
            _financialInstrumentInfra = financialInstrumentInfra;
        }

        public Result ProcessFinancialInstruments(IEnumerable<string> ISINCodes, CancellationToken cancellationToken)
        {
            if (ISINCodes?.Any(x => x.Length != ISINLength) ?? true)
                return Result.FromError("One or More ISIN codes are invalid");

            var financialInstrumentsResult = RetrieveFinancialInstruments(ISINCodes, cancellationToken);

            StoreFinancialInstruments(financialInstrumentsResult, cancellationToken);

            return Result.FromSucess();
        }

        private void StoreFinancialInstruments(IEnumerable<FinancialInstrument> financialInstruments, CancellationToken cancellationToken)
        {
            Parallel.ForEach(financialInstruments, async (financialInstrument) =>
            {
                var result = await _financialInstrumentInfra.Store(financialInstrument, cancellationToken);

                if (!result.Success)
                    _logger.LogError("An error occurred when storing ISIN price", new
                    {
                        ISINCode = financialInstrument.ISINCode,
                        ISINPrice = financialInstrument.Price,
                        ErrorMessage = result.Message
                    });
            });
        }

        private IEnumerable<FinancialInstrument> RetrieveFinancialInstruments(IEnumerable<string> ISINCodes, CancellationToken cancellationToken)
        {
            var result = new List<FinancialInstrument>();

            Parallel.ForEach(ISINCodes, async (code) =>
            {
                var isinPriceResult = await _securityPriceInfra.GetPriceByISIN(code, cancellationToken);

                if (isinPriceResult.Success)
                    result.Add(new(code, isinPriceResult.Data));
                else
                    _logger.LogError("An error occurred while retrieving financial instruments", new
                    {
                        ISINCode = code,
                        ErrorMessage = isinPriceResult.Message
                    });

            });

            return result;
        }
    }
}
