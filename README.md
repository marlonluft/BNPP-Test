# BNPP-Test

## Exercice Description:
As a software developer you should create a service class/layer that allows you to retrieve and store the prices of a list of securities. Please note that:

- A security is a financial instrument identified by an ISIN, an alphanumeric code of 12 characters.
- The service should be written according to SOLID principles with the usage of Dependency Injection.
- The service should have a method which receives as input a list of ISIN.
- The service has to retrieve and store the price for each ISIN in a SQL server database.
- The price of an ISIN must be retrieved through an external web API: https://securities.dataprovider.com/securityprice/{isin}
- The service should be unit tested

## My Observations:
This code has small changes since the live coding that couldn't be finished on time, so I just made it build with success, finished the unit tests, and some minor fixes. The infrastructure connection with the database or consuming the external API was not done as it was not started during the live coding.

- FinancialInstrumentService has the main login;
- ISecurityPriceInfra is the contract that would communicate with the external web API;
- IFinancialInstrumentInfra is the contract with the database to store the financial instrument.

## Next steps:
- Implement ISecurityPriceInfra and IFinancialInstrumentInfra;
- Create an ISINCode record struct to keep the validation of length inside the struct instead in the FinancialInstrumentService, this way we can prevent creating a FinancialInstrument object with an invalid ISIN code and keep the validation responsibility inside the ISIN code.
