using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ItemShop
{
    public List<Cosmetic> DailyItems { get; set; }
    public List<Cosmetic> FeaturedItems { get; set; }
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
    private List<Cosmetic> allItems;

    public CosmeticShopAI(List<Cosmetic> items)
    {
        itemWeights = new Dictionary<string, double>();
        feedbackScores = new Dictionary<string, double>();
        shopHistory = new List<ItemShop>();
        goodShopsHistory = new List<ItemShop>();
        random = new Random();
        itemRelationships = new Dictionary<(string, string), int>();
        allItems = items;
    }

    public void GenerateAndDisplayItemShop()
    {
        List<Cosmetic> dailyItems = SelectUniqueItems(6);
        List<Cosmetic> featuredItems = SelectUniqueItems(2, dailyItems);

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

    public void RateItemShop()
    {
        ItemShop itemShop = GenerateAndDisplayItemShopWithReturn();

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
        Console.WriteLine("The AI will provide two item IDs to train their relationship.");

        for (int i = 0; i < 10; i++) // Assuming 10 pairs for training
        {
            Cosmetic firstItem = allItems[random.Next(allItems.Count)];
            Cosmetic secondItem = allItems[random.Next(allItems.Count)];

            if (firstItem.Id == secondItem.Id) // Skip if same items are selected
                continue;

            Console.WriteLine($"Rate the similarity between {firstItem.Id} and {secondItem.Id} (1-5):");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int similarity) && similarity >= 1 && similarity <= 5)
                {
                    TrainCosmeticRelationship(firstItem.Id, secondItem.Id, similarity);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                }
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

    public void BenchmarkAI()
    {
        int totalShops = 10;
        int correctPredictions = 0;
        List<ItemShop> benchmarkShops = new List<ItemShop>();

        for (int i = 0; i < totalShops; i++)
        {
            var dailyItems = SelectUniqueItems(6);
            var featuredItems = SelectUniqueItems(2, dailyItems);
            ItemShop itemShop = new ItemShop { DailyItems = dailyItems, FeaturedItems = featuredItems };
            benchmarkShops.Add(itemShop);

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

        // Export benchmark shop data
        ExportBenchmarkShops(benchmarkShops);

        double accuracy = (double)correctPredictions / (totalShops * 8) * 100;
        Console.WriteLine($"Benchmark AI Accuracy: {accuracy}%");
    }

    public void ExportBenchmarkShops(List<ItemShop> benchmarkShops)
    {
        using (StreamWriter writer = new StreamWriter("benchmark_shops.txt"))
        {
            foreach (var shop in benchmarkShops)
            {
                writer.WriteLine("Shop:");
                writer.WriteLine("Daily Items:");
                foreach (var item in shop.DailyItems)
                {
                    writer.WriteLine($"{item.Category}:{item.Id},{item.Price}");
                }
                writer.WriteLine("Featured Items:");
                foreach (var item in shop.FeaturedItems)
                {
                    writer.WriteLine($"{item.Category}:{item.Id},{item.Price}");
                }
                writer.WriteLine();
            }
        }
        Console.WriteLine("Benchmark shop data exported successfully.");
    }

    public List<ItemShop> LoadBenchmarkShops()
    {
        List<ItemShop> benchmarkShops = new List<ItemShop>();

        if (File.Exists("benchmark_shops.txt"))
        {
            using (StreamReader reader = new StreamReader("benchmark_shops.txt"))
            {
                while (!reader.EndOfStream)
                {
                    ItemShop shop = new ItemShop();
                    List<Cosmetic> dailyItems = new List<Cosmetic>();
                    List<Cosmetic> featuredItems = new List<Cosmetic>();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == "Shop:")
                            continue;
                        else if (line == "Daily Items:")
                        {
                            while ((line = reader.ReadLine()) != "Featured Items:")
                            {
                                string[] parts = line.Split(new char[] { ':', ',' });
                                dailyItems.Add(new Cosmetic(parts[0], parts[1], int.Parse(parts[2])));
                            }
                        }
                        else if (line == "Featured Items:")
                        {
                            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                            {
                                string[] parts = line.Split(new char[] { ':', ',' });
                                featuredItems.Add(new Cosmetic(parts[0], parts[1], int.Parse(parts[2])));
                            }
                        }
                    }

                    shop.DailyItems = dailyItems;
                    shop.FeaturedItems = featuredItems;
                    benchmarkShops.Add(shop);
                }
            }
            Console.WriteLine("Benchmark shop data loaded successfully.");
        }
        else
        {
            Console.WriteLine("No benchmark shop data found.");
        }

        return benchmarkShops;
    }

    public void ProvideShopFeedback(string itemId, int rating)
    {
        feedbackScores[itemId] = rating;
    }

    public void UpdateModel(bool isGoodFeedback)
    {
        foreach (var entry in feedbackScores)
        {
            string itemId = entry.Key;
            double rating = entry.Value;

            if (!itemWeights.ContainsKey(itemId))
            {
                itemWeights[itemId] = 0;
            }

            if (isGoodFeedback)
            {
                itemWeights[itemId] += (rating - itemWeights[itemId]) * 0.1;
            }
            else
            {
                itemWeights[itemId] -= (itemWeights[itemId] - rating) * 0.1;
            }
        }

        feedbackScores.Clear();
    }

    public void ExportModel(string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var entry in itemWeights)
            {
                writer.WriteLine($"{entry.Key}:{entry.Value}");
            }
        }
        Console.WriteLine("Model exported successfully.");
    }

    public void LoadModel(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(':');
                    itemWeights[parts[0]] = double.Parse(parts[1]);
                }
            }
            Console.WriteLine("Model loaded successfully.");
        }
        else
        {
            Console.WriteLine("No model file found.");
        }
    }

    private List<Cosmetic> SelectUniqueItems(int count, List<Cosmetic> excludeItems = null)
    {
        excludeItems ??= new List<Cosmetic>();
        HashSet<string> selectedIds = new HashSet<string>(excludeItems.Select(item => item.Id));
        List<Cosmetic> selectedItems = new List<Cosmetic>();

        while (selectedItems.Count < count)
        {
            Cosmetic randomItem = allItems[random.Next(allItems.Count)];
            if (!selectedIds.Contains(randomItem.Id))
            {
                selectedItems.Add(randomItem);
                selectedIds.Add(randomItem.Id);
            }
        }

        return selectedItems;
    }

    private ItemShop GenerateAndDisplayItemShopWithReturn()
    {
        List<Cosmetic> dailyItems = SelectUniqueItems(6);
        List<Cosmetic> featuredItems = SelectUniqueItems(2, dailyItems);

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
        List<Cosmetic> items = ReadItems("items.txt");
        CosmeticShopAI ai = new CosmeticShopAI(items);

        ai.LoadModel("model.txt");
        ai.LoadBenchmarkShops();

        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ai.GenerateAndDisplayItemShop();
                    break;
                case "2":
                    ai.RateItemShop();
                    break;
                case "3":
                    ai.TrainCosmeticRelationship();
                    break;
                case "4":
                    ai.AnalyzeShopPatterns();
                    break;
                case "5":
                    ai.BenchmarkAI();
                    break;
                case "6":
                    ai.ExportModel("model.txt");
                    break;
                case "7":
                    ai.LoadModel("model.txt");
                    break;
                case "8":
                    ai.ExportBenchmarkShops(ai.LoadBenchmarkShops());
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("1. Generate Item Shop");
        Console.WriteLine("2. Rate Item Shop");
        Console.WriteLine("3. Train Relationships");
        Console.WriteLine("4. Analyze Shop Patterns");
        Console.WriteLine("5. Benchmark AI");
        Console.WriteLine("6. Export Model");
        Console.WriteLine("7. Load Model");
        Console.WriteLine("8. Export Benchmark Data");
        Console.WriteLine("9. Exit");
        Console.Write("Enter your choice: ");
    }

    static List<Cosmetic> ReadItems(string fileName)
    {
        List<Cosmetic> items = new List<Cosmetic>();

        foreach (var line in File.ReadLines(fileName))
        {
            var parts = line.Split(new char[] { ':', ',' });
            items.Add(new Cosmetic(parts[0], parts[1], int.Parse(parts[2])));
        }

        return items;
    }
}
