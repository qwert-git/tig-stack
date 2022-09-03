using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfluxDbSample.Controllers
{
    internal class RequestsCountInfluxDbFilterAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var client = InfluxDBClientFactory.Create("http://influxdb:8086", "admin", "admin".ToCharArray());

            var api = client.GetWriteApiAsync();
            var point = PointData
                .Measurement("api_requests")
                .Tag("app", nameof(InfluxDbSample))
                .Field("endpoint", context.HttpContext.Request.Path)
                .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

            await api.WritePointAsync(point, "influx", "org");

            await next();
        }
    }
}