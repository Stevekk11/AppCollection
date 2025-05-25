using Moq;
using AppCollection.Services;
using System.Net;
using Moq.Protected;

namespace NUnitTests;
[TestFixture]
public class WeatherTest
{
    [Test]
    public async Task GetWeatherAsync_ValidCity_ReturnsWeather()
    {
        // Arrange
        var responseContent = @"{
                ""current_condition"": [{
                    ""temp_C"": ""20"",
                    ""windspeedKmph"": ""10"",
                    ""humidity"": ""50""
                }]
            }";

        var messageHandler = new Mock<HttpMessageHandler>();
        messageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        var httpClient = new HttpClient(messageHandler.Object);

        var service = new WeatherService(httpClient);

        // Act
        var result = await service.GetWeatherAsync("Brno");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TemperatureCelsius > 0);
        Assert.That(result.WindSpeedKmh > 0);
        Assert.That(result.HumidityPercent > 0);
    }
}
