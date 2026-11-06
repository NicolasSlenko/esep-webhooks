using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    public string FunctionHandler(object input, ILambdaContext context)
    {
        var json = JsonConvert.DeserializeObject<JObject>(input?.ToString() ?? "");
        var issueUrl = json?["issue"]?["html_url"]?.Value<string>();

        // GitHub sends a ping when the webhook is first added.
        if (string.IsNullOrWhiteSpace(issueUrl))
        {
            context.Logger.LogLine("Ignoring event without an issue link.");
            return "ignored";
        }

        var slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");
        if (string.IsNullOrWhiteSpace(slackUrl))
        {
            throw new InvalidOperationException("SLACK_URL is not configured.");
        }

        var payload = JsonConvert.SerializeObject(new { text = $"Issue Created: {issueUrl}" });
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, slackUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        using var response = client.Send(request);
        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }
}
