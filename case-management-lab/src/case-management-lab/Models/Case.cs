using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace case_management_lab.Models
{
    [DynamoDBTable("Case")]
    public class Case
    {
        [DynamoDBHashKey("case_id")]
        public string? case_id { get; set; }

        [DynamoDBProperty("name")]
        public string? Name { get; set; }

        [DynamoDBProperty("description")]
        public string? Description { get; set; }

        [DynamoDBProperty("status")]
        public string? Status { get; set; }

        [DynamoDBProperty("assignedTo")]
        public string? AssignedTo { get; set; }

        [DynamoDBProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [DynamoDBProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DynamoDBProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class DynamoDBContextFactory
    {
        public static DynamoDBContext Create()
        {
            var client = new AmazonDynamoDBClient();
            return new DynamoDBContext(client);
        }
    }
}
