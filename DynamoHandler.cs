using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Controllers
{
    public class DynamoDataProvider : IDataProvider
    {
        public DynamoDataProvider(IAmazonDynamoDB dynamoClient)
        {
            DynamoClient = dynamoClient;
        }

        public IAmazonDynamoDB DynamoClient { get; }

        public async Task<CountryDetail> Add(CountryDetail country)
        {
            var expressionAttributeValue = new Dictionary<string, AttributeValue>
            {
                { "code", new AttributeValue(country.code) },
                { "name", new AttributeValue(country.name) },
                { "area", new AttributeValue {N = country.area.ToString() } },
                { "population", new AttributeValue {N = country.population.ToString() } },
                { "flag", new AttributeValue(country.flag) }
            };

            var result = await DynamoClient.PutItemAsync(new PutItemRequest
            {
                TableName = "CountriesTable",
                Item = expressionAttributeValue
            });

            //  return await Get(country.code);
            return country;
        }

        public async Task<CountryDetail> Get(string code)
        {
            var result = await DynamoClient.GetItemAsync(new GetItemRequest
            {
                TableName = "CountriesTable",
                Key = new Dictionary<string, AttributeValue> { { "code", new AttributeValue(code) } },
            });

            return GetCountryDetails(result.Item);
        }

        public async Task<IList<CountryDetail>> GetAll()
        {
            var countries = new List<CountryDetail>();

            var result = await DynamoClient.ScanAsync(new ScanRequest
            {
                TableName = "CountriesTable"
            });


            foreach (var item in result.Items)
            {
                CountryDetail countryDetail = GetCountryDetails(item);
                countries.Add(countryDetail);
            }

            return countries;
        }

        private static CountryDetail GetCountryDetails(Dictionary<string, AttributeValue> item)
        {
            var countryDetail = new CountryDetail();
            item.TryGetValue("area", out var areaValue);
            Int32.TryParse(areaValue?.N, out var area);
            countryDetail.area = area;


            item.TryGetValue("code", out var codeValue);
            countryDetail.code = codeValue?.S;

            item.TryGetValue("flag", out var flagValue);
            countryDetail.flag = flagValue?.S;

            item.TryGetValue("name", out var nameValue);
            countryDetail.name = nameValue?.S;

            item.TryGetValue("population", out var populationValue);
            Int32.TryParse(populationValue?.N, out var population);
            countryDetail.population = population;
            return countryDetail;
        }
    }
}