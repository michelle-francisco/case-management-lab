using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using case_management_lab.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace case_management_lab;

public class Functions
{
    private readonly DynamoDBContext _dBContext;
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
        var client = new AmazonDynamoDBClient();
        _dBContext = new DynamoDBContext(client);
    }

    // for testing
    public Functions(DynamoDBContext dbContext)
    {
        _dBContext = dbContext;
    }

    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The API Gateway response.</returns>
    public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get Request\n");

        // Extract the id from the request body
        if (request.PathParameters == null || !request.PathParameters.TryGetValue("id", out string? id))
        {
            context.Logger.LogError("Missing id parameter in request path");
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Missing id parameter in request path",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
        }

        try
        {
            var document = await _dBContext.LoadAsync<Case>(id);

            if (document == null)
            {
                context.Logger.LogError("Record not found");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Body = "Record not found",
                    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                };
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = document.ToJson(),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error retrieving record from DynamoDB: {ex.Message} {ex.StackTrace}");

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = "An error occurred while retrieving the record",
                Headers = new Dictionary<string, string> { { "Content-Type", "text-plain" } }
            };
        }
    }
}
