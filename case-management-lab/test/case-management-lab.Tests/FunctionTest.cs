using Xunit;
using case_management_lab;
using case_management_lab.Models;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;

namespace case_management_lab.Tests;

public class FunctionTest
{
    private readonly Functions _functions;

    public FunctionTest()
    {
        // Use a new instance of the DynamoDBContext for each test
        var dbContext = new Amazon.DynamoDBv2.DataModel.DynamoDBContext(
            new AmazonDynamoDBClient());
        _functions = new Functions(dbContext);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenIdParameterIsMissing()
    {
        // Arrange
        var request = new APIGatewayProxyRequest
        {
        };

        var context = new TestLambdaContext();

        // Act
        var response = await _functions.Get(request, context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Missing id parameter in request path", response.Body);
    }
}