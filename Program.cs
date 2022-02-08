using System;
using System.Threading.Tasks;
using Binance.Spot;
using System.Text.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BitcoinBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            string apiKey = config["apiKey"];
            string secretKey = config["secretKey"];

            string baseUrl = "https://testnet.binance.vision";


            // Get BTC Price and print to console
            Market market = new Market(baseUrl, apiKey, secretKey);

            var priceTickerString = await market.SymbolPriceTicker("BTCUSDT");

            PriceTicker? priceTickerObj = JsonSerializer.Deserialize<PriceTicker>(priceTickerString);

            if (priceTickerObj?.price != null)
            {
                Console.WriteLine("Symbol: " + priceTickerObj.symbol);
                Console.WriteLine("BTC Price: " + priceTickerObj.price);
            }


            // Check current balance
            var account = new SpotAccountTrade(baseUrl, apiKey, secretKey);

            var result = await account.AccountInformation();

            Console.WriteLine(result);
        }
    }
}


