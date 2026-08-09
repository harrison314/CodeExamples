using BcHttpClient.Client;

namespace BcHttpClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting request");

        HttpClient httpClient = new HttpClient(new BouncyCastleHttpMessageHandler());
        string daco = await httpClient.GetStringAsync("https://harrison314.github.io/dev_endpoints/simple.json");

        Console.WriteLine(daco);
    }
}
