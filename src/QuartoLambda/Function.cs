using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace QuartoLambda
{
    public class Function
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {

        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SNS event object and can be used 
        /// to respond to SNS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"HttpMethod:\"{request.HttpMethod}\"");
            context.Logger.LogLine($"Body:\"{request.Body}\"");
            context.Logger.LogLine($"Headers:\"{request.Headers?.Count.ToString() ?? "<null>"}\"");
            if(request.Headers != null)
            {
                foreach (var h in request.Headers)
                {
                    context.Logger.LogLine($"  {h.Key}=\"{h.Value}\"");
                }
            }
            //foreach (var record in evnt.Records)
            //{
            //    await ProcessRecordAsync(record, context);
            //}
            //await Task.CompletedTask;
            return buildResponse();
        }

        private class Result
        {
            public string Author;
            public string Message;
        }

        private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
        {
            context.Logger.LogLine($"Steve was here...");
            //context.Logger.LogLine($"Processed record {record.Sns.Message}");

            // TODO: Do interesting work based on the new message
            await Task.CompletedTask;
        }

        private APIGatewayProxyResponse buildResponse()
        {
            return new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(new Result()
                {
                    Author = "Steve",
                    Message = "Success!"
                }),
            };
        }
    }
}
