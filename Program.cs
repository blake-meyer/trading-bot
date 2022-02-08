using System;
using System.Threading.Tasks;
using Binance.Spot;
using System.Text.Json;
//using BitcoinBot;

// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
namespace BitcoinBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiKey = "ZxNHyzK4aOz2lWoyWYFbozgJ3N0GvRr17LPMAimMiv8VRIuSZWImlKlSEEKcChtU";
            string secretKey = "K3Hupa1kBrUyLgQn2dMpeoZtlQnbtICAZFkEIt8feajb5xLycoODuf4oYRiUQ6U8";

                        

            Market market = new Market("https://testnet.binance.vision", apiKey, secretKey);

            var priceTickerString = await market.SymbolPriceTicker("BTCUSDT");


            PriceTicker? priceTickerObj = JsonSerializer.Deserialize<PriceTicker>(priceTickerString);

            if (priceTickerObj?.price != null)
            {
                Console.WriteLine("BTC Price: " + priceTickerObj.price);
            }
        }
    }
}


