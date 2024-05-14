using ClassLibrary1.Core;
using ClassLibrary1.Core.Crosscutting;
using ClassLibrary1.Core.Interfaces;
using ClassLibrary1.Core.Model;
using Moq;

namespace TestProject1
{
    public class Tests
    {
        private const string ValidISINCode = "A345678H90IO";

        private readonly Mock<ISecurityPriceInfra> _securityPriceInfraMock;
        private readonly Mock<IFinancialInstrumentInfra> _financialInstrumentInfraMock;
        private readonly Mock<ILog> _logger;
        private readonly CancellationToken _validCancellationToken;

        public Tests()
        {
            _securityPriceInfraMock = new Mock<ISecurityPriceInfra>();
            _financialInstrumentInfraMock = new Mock<IFinancialInstrumentInfra>();
            _logger = new Mock<ILog>();
            _validCancellationToken = new CancellationToken(false);
        }

        [Test]
        public void GivenAGetPriceByISINError_WhenProcessFinancialInstruments_ThenLogTheError()
        {
            _securityPriceInfraMock
                .Setup(x => x.GetPriceByISIN(ValidISINCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Result<decimal>.FromError("Loggin validation error")));

            var securityService = new FinancialInstrumentService(
                _securityPriceInfraMock.Object,
                _financialInstrumentInfraMock.Object,
                _logger.Object);

            var result = securityService.ProcessFinancialInstruments([ValidISINCode], _validCancellationToken);

            _logger.Verify(x => x.LogError("An error occurred while retrieving financial instruments", It.IsAny<object?>()), Times.Once, "Log not reached");
        }

        [Test]
        public void GivenAFinancialInstrumentStoreError_WhenProcessFinancialInstruments_ThenLogTheError()
        {
            var getPriceByISINResult = 12m;
            var expectedErrorMessage = "Loggin validation error";

            _securityPriceInfraMock
                .Setup(x => x.GetPriceByISIN(ValidISINCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Result<decimal>.FromSucess(getPriceByISINResult)));

            _financialInstrumentInfraMock
                .Setup(x => x.Store(It.IsAny<FinancialInstrument>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Result.FromError(expectedErrorMessage)));

            var securityService = new FinancialInstrumentService(
                _securityPriceInfraMock.Object,
                _financialInstrumentInfraMock.Object,
                _logger.Object);

            var result = securityService.ProcessFinancialInstruments([ValidISINCode], _validCancellationToken);

            _logger.Verify(x => x.LogError("An error occurred when storing ISIN price", It.IsAny<object?>()), Times.Once, "Log not reached");
        }

        [Test]
        [TestCase(ValidISINCode)]
        public void GivenAListWithOneValidISINCode_WhenProcessFinancialInstruments_ThenReturnsSuccess(string ISINCode)
        {
            var getPriceByISINResult = 12m;

            _securityPriceInfraMock
                .Setup(x => x.GetPriceByISIN(ISINCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Result<decimal>.FromSucess(getPriceByISINResult)));

            var isinCodes = new List<string>
            {
                ISINCode
            };

            var securityService = new FinancialInstrumentService(
                _securityPriceInfraMock.Object,
                _financialInstrumentInfraMock.Object,
                _logger.Object);

            var result = securityService.ProcessFinancialInstruments(isinCodes, _validCancellationToken);

            Assert.IsTrue(result.Success);
        }

        [Test]
        [TestCase("")]
        public void GivenAListWithOneEmptyValue_WhenProcessFinancialInstruments_ThenReturnsError(string ISINCode) => ValidateWrongISINCode([ISINCode]);

        [Test]
        public void GivenAListWithOnNullValue_WhenProcessFinancialInstruments_ThenReturnsError() => ValidateWrongISINCode([null]);

        [Test]
        [TestCase("12A")]
        public void GivenAListWithAnInvalidISINShorterThan12Characters_WhenProcessFinancialInstruments_ThenReturnsError(string ISINCode) => ValidateWrongISINCode([ISINCode]);

        [Test]
        [TestCase("1234567890ABCNMN")]
        public void GivenAListWithAnInvalidISINGreaterThan12Characters_WhenProcessFinancialInstruments_ThenReturnsError(string ISINCode) => ValidateWrongISINCode([ISINCode]);

        [Test]
        public void GivenANullList_WhenProcessFinancialInstruments_ThenReturnsError() => ValidateWrongISINCode(null);

        private void ValidateWrongISINCode(IEnumerable<string>? ISINCodes)
        {
            var expectedErrorMessgae = "One or More ISIN codes are invalid";

            var securityService = new FinancialInstrumentService(
                _securityPriceInfraMock.Object,
                _financialInstrumentInfraMock.Object,
                _logger.Object);

            var result = securityService.ProcessFinancialInstruments(ISINCodes, _validCancellationToken);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(expectedErrorMessgae, result.Message);
        }
    }
}