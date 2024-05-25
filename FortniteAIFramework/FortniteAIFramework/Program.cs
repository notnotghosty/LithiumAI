using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ItemShop
{
    public List<Cosmetic> DailyItems { get; set; }
    public List<Cosmetic> FeaturedItems { get; set; }
    public string Rating { get; set; }
}

class Cosmetic
{
    public string Category { get; set; }
    public string Id { get; set; }
    public int Price { get; set; }

    public Cosmetic(string category, string id, int price)
    {
        Category = category;
        Id = id;
        Price = price;
    }
}

class CosmeticShopAI
{
    private Dictionary<string, double> itemWeights;
    private Dictionary<string, double> feedbackScores;
    private List<ItemShop> shopHistory;
    private List<ItemShop> goodShopsHistory;
    private Random random;
    private Dictionary<(string, string), int> itemRelationships;

    public CosmeticShopAI()
    {
        itemWeights = new Dictionary<string, double>();
        feedbackScores = new Dictionary<string, double>();
        shopHistory = new List<ItemShop>();
        goodShopsHistory = new List<ItemShop>();
        random = new Random();
        itemRelationships = new Dictionary<(string, string), int>();
    }

    public void GenerateAndDisplayItemShop(List<Cosmetic> items)
    {
        List<Cosmetic> dailyItems = SelectItems(items, 6);
        List<Cosmetic> featuredItems = SelectItems(items, 2);

        ItemShop itemShop = new ItemShop { DailyItems = dailyItems, FeaturedItems = featuredItems };
        shopHistory.Add(itemShop);

        Console.WriteLine("Generated Item Shop:");
        Console.WriteLine("Daily Items:");
        foreach (var item in dailyItems)
        {
            Console.WriteLine($"{item.Category}: {item.Id}, Price: {item.Price}");
        }
        Console.WriteLine("Featured Items:");
        foreach (var item in featuredItems)
        {
            Console.WriteLine($"{item.Category}: {item.Id}, Price: {item.Price}");
        }
    }

    public void RateItemShop(List<Cosmetic> items)
    {
        ItemShop itemShop = GenerateAndDisplayItemShopWithReturn(items);

        Console.WriteLine("Please rate the item shop:");
        Console.WriteLine("Is it good or bad?");
        string feedback = Console.ReadLine().ToLower();
        bool isGoodFeedback = feedback == "good";
        Console.WriteLine("Thank you for your feedback!");

        foreach (var item in itemShop.DailyItems.Concat(itemShop.FeaturedItems))
        {
            while (true)
            {
                Console.WriteLine($"How do you rate the placement of {item.Id}?");
                Console.WriteLine("1. Belongs in Daily");
                Console.WriteLine("2. Can be in Both Daily and Featured");
                Console.WriteLine("3. Belongs in Featured");
                Console.Write("Enter your rating: ");
                if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 3)
                {
                    ProvideShopFeedback(item.Id, rating);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 3.");
                }
            }
        }

        if (isGoodFeedback)
        {
            goodShopsHistory.Add(itemShop);
        }

        UpdateModel(isGoodFeedback);
    }

    public void TrainCosmeticRelationship()
    {
        Console.WriteLine("Please provide two item IDs to train their relationship:");
        Console.Write("First item ID: ");
        string firstItemId = Console.ReadLine();
        Console.Write("Second item ID: ");
        string secondItemId = Console.ReadLine();
        while (true)
        {
            Console.WriteLine("Please rate the similarity of these items (1-5):");
            if (int.TryParse(Console.ReadLine(), out int similarity) && similarity >= 1 && similarity <= 5)
            {
                TrainCosmeticRelationship(firstItemId, secondItemId, similarity);
                break;
            }
            else
            {
                Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            }
        }
    }

    public void TrainCosmeticRelationship(string firstItemId, string secondItemId, int similarity)
    {
        var key = (firstItemId, secondItemId);
        if (itemRelationships.ContainsKey(key))
        {
            itemRelationships[key] = (itemRelationships[key] + similarity) / 2;
        }
        else
        {
            itemRelationships[key] = similarity;
        }
        Console.WriteLine($"Trained relationship between {firstItemId} and {secondItemId} with similarity {similarity}");
    }

    public void AnalyzeShopPatterns()
    {
        var categoryCounts = new Dictionary<string, int>();
        var priceDistribution = new Dictionary<int, int>();
        var itemAssociations = new Dictionary<string, Dictionary<string, int>>();

        foreach (var shop in goodShopsHistory)
        {
            foreach (var item in shop.DailyItems.Concat(shop.FeaturedItems))
            {
                if (!categoryCounts.ContainsKey(item.Category))
                    categoryCounts[item.Category] = 0;
                categoryCounts[item.Category]++;

                if (!priceDistribution.ContainsKey(item.Price))
                    priceDistribution[item.Price] = 0;
                priceDistribution[item.Price]++;

                foreach (var associatedItem in shop.DailyItems.Concat(shop.FeaturedItems))
                {
                    if (item.Id == associatedItem.Id) continue;

                    if (!itemAssociations.ContainsKey(item.Id))
                        itemAssociations[item.Id] = new Dictionary<string, int>();

                    if (!itemAssociations[item.Id].ContainsKey(associatedItem.Id))
                        itemAssociations[item.Id][associatedItem.Id] = 0;

                    itemAssociations[item.Id][associatedItem.Id]++;
                }
            }
        }

        Console.WriteLine("Category Distribution in Good Shops:");
        foreach (var entry in categoryCounts)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }

        Console.WriteLine("Price Distribution in Good Shops:");
        foreach (var entry in priceDistribution)
        {
            Console.WriteLine($"Price: {entry.Key}, Count: {entry.Value}");
        }

        Console.WriteLine("Item Associations in Good Shops:");
        foreach (var entry in itemAssociations)
        {
            Console.WriteLine($"Item: {entry.Key}");
            foreach (var assoc in entry.Value)
            {
                Console.WriteLine($"  Associated with {assoc.Key}: {assoc.Value} times");
            }
        }
    }

    public void BenchmarkAI(List<Cosmetic> items)
    {
        int totalShops = 10;
        int correctPredictions = 0;

        for (int i = 0; i < totalShops; i++)
        {
            var dailyItems = SelectItems(items, 6);
            var featuredItems = SelectItems(items, 2);

            foreach (var item in dailyItems)
            {
                if (itemWeights.ContainsKey(item.Id) && itemWeights[item.Id] < 2.0)
                {
                    correctPredictions++;
                }
            }

            foreach (var item in featuredItems)
            {
                if (itemWeights.ContainsKey(item.Id) && itemWeights[item.Id] >= 2.0)
                {
                    correctPredictions++;
                }
            }
        }

        double accuracy = (double)correctPredictions / (totalShops * 8) * 100;
        Console.WriteLine($"Benchmark AI Accuracy: {accuracy}%");
    }

    public List<Cosmetic> SelectItems(List<Cosmetic> items, int count)
    {
        List<Cosmetic> selectedItems = new List<Cosmetic>();
        HashSet<string> selectedIds = new HashSet<string>();

        while (selectedItems.Count < count)
        {
            Cosmetic item = items[random.Next(items.Count)];

            if (!selectedIds.Contains(item.Id))
            {
                selectedItems.Add(item);
                selectedIds.Add(item.Id);
            }
        }

        return selectedItems;
    }

    public void ProvideShopFeedback(string itemId, int rating)
    {
        if (feedbackScores.ContainsKey(itemId))
        {
            feedbackScores[itemId] = (feedbackScores[itemId] + rating) / 2.0;
        }
        else
        {
            feedbackScores[itemId] = rating;
        }
    }

    public void UpdateModel(bool isGoodFeedback)
    {
        foreach (var feedback in feedbackScores)
        {
            if (itemWeights.ContainsKey(feedback.Key))
            {
                itemWeights[feedback.Key] = (itemWeights[feedback.Key] + feedback.Value) / 2.0;
            }
            else
            {
                itemWeights[feedback.Key] = feedback.Value;
            }
        }
    }

    public void ExportModel()
    {
        using (StreamWriter writer = new StreamWriter("model.txt"))
        {
            foreach (var item in itemWeights)
            {
                writer.WriteLine($"{item.Key}:{item.Value}");
            }
        }
        Console.WriteLine("Model exported successfully.");
    }

    public void LoadModel()
    {
        if (File.Exists("model.txt"))
        {
            string[] lines = File.ReadAllLines("model.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && double.TryParse(parts[1], out double weight))
                {
                    itemWeights[parts[0]] = weight;
                }
            }
            Console.WriteLine("Model loaded successfully.");
        }
        else
        {
            Console.WriteLine("Model file not found.");
        }
    }

    private ItemShop GenerateAndDisplayItemShopWithReturn(List<Cosmetic> items)
    {
        List<Cosmetic> dailyItems = SelectItems(items, 6);
        List<Cosmetic> featuredItems = SelectItems(items, 2);

        ItemShop itemShop = new ItemShop { DailyItems = dailyItems, FeaturedItems = featuredItems };
        shopHistory.Add(itemShop);

        Console.WriteLine("Generated Item Shop:");
        Console.WriteLine("Daily Items:");
        foreach (var item in dailyItems)
        {
            Console.WriteLine($"{item.Category}: {item.Id}, Price: {item.Price}");
        }
        Console.WriteLine("Featured Items:");
        foreach (var item in featuredItems)
        {
            Console.WriteLine($"{item.Category}: {item.Id}, Price: {item.Price}");
        }

        return itemShop;
    }
}

class Program
{
    static void Main(string[] args)
    {
        CosmeticShopAI ai = new CosmeticShopAI();
        List<Cosmetic> items = ReadItems("cosmetics.txt");

        while (true)
        {
            DisplayMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ai.GenerateAndDisplayItemShop(items);
                    break;
                case "2":
                    ai.RateItemShop(items);
                    break;
                case "3":
                    ai.TrainCosmeticRelationship();
                    break;
                case "4":
                    ai.ExportModel();
                    break;
                case "5":
                    ai.LoadModel();
                    break;
                case "6":
                    ai.AnalyzeShopPatterns();
                    break;
                case "7":
                    ai.BenchmarkAI(items);
                    break;
                case "8":
                    Console.WriteLine("Exiting the program...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("Welcome to the Cosmetic Shop AI Trainer!");
        Console.WriteLine("1. Generate Item Shop");
        Console.WriteLine("2. Generate and Rate Item Shop");
        Console.WriteLine("3. Train Cosmetic Relationship");
        Console.WriteLine("4. Export AI Model");
        Console.WriteLine("5. Load AI Model");
        Console.WriteLine("6. Analyze Shop Patterns");
        Console.WriteLine("7. Benchmark AI");
        Console.WriteLine("8. Exit");
        Console.Write("Please select an option: ");
    }

    static List<Cosmetic> ReadItems(string filename)
    {
        List<Cosmetic> items = new List<Cosmetic>();

        if (!File.Exists(filename))
        {
            Console.WriteLine($"Error: File {filename} not found.");
            return items;
        }

        string[] lines = File.ReadAllLines(filename);

        foreach (string line in lines)
        {
            try
            {
                string[] parts = line.Split(':');
                if (parts.Length != 2)
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    continue;
                }

                string category = parts[0];
                string[] idAndPrice = parts[1].Split(',');
                if (idAndPrice.Length != 2 || !int.TryParse(idAndPrice[1], out int price))
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    continue;
                }

                string id = idAndPrice[0];

                items.Add(new Cosmetic(category, id, price));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line: {line}. Error: {ex.Message}");
            }
        }
        return items;
    }
}
