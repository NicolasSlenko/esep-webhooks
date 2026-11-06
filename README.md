# ESEP Assignment 8: Event-Driven Architecture

Opening or changing an issue in this repository triggers a GitHub webhook. API Gateway passes the JSON request to the `EsepWebhook` AWS Lambda function, which posts `Issue Created: <issue link>` to the class Slack channel.

## Project

The .NET 10 Lambda project is in `EsepWebhook/`. The handler reads `issue.html_url` from the event. It ignores GitHub's initial ping because that event has no issue link.

## Deploy

1. Install the .NET 10 SDK, AWS CLI v2, `Amazon.Lambda.Templates`, and `Amazon.Lambda.Tools`.
2. Configure AWS CLI access to the intended class AWS account in `us-east-2`.
3. In `EsepWebhook/`, run `dotnet lambda deploy-function EsepWebhook --region us-east-2 --profile YOUR_PROFILE`.
4. Set the Lambda environment variable `SLACK_URL` to the section's Slack incoming-webhook URL in AWS Lambda configuration. Never commit its value.
5. Use a Regional API Gateway REST API with a root `POST` method and non-proxy Lambda integration. Deploy it to the `default` stage.
6. Add a GitHub webhook pointing to that stage's invoke URL. Use `application/json`, select only Issues events, and keep SSL verification enabled.

## Test

POST `{"issue":{"html_url":"https://github.com/NicolasSlenko/esep-webhooks/issues/1"}}` as `application/json` to the API's `/default` URL. A successful Slack request returns `"ok"`. Then open a real GitHub issue and confirm its link appears in `#eventing-webhook`.

The Slack webhook URL and AWS credentials must stay out of this public repository.
