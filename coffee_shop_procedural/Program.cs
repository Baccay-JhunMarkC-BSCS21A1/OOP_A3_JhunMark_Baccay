using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace coffee_shop_procedural;

public class Menu
{
    public List<string> MenuItems { get; private set; } = new List<string>();  // Initialized to an empty list
    public List<decimal> MenuPrices { get; private set; } = new List<decimal>();  // Initialized to an empty list

    public Menu(string menuFile)
    {
        LoadMenu(menuFile);
    }

    void LoadMenu(string menuFile)
    {
        if (File.Exists(menuFile))
        {
            string json = File.ReadAllText(menuFile);
            var menu = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);

            // Ensure menu is not null before adding to lists
            if (menu != null)
            {
                MenuItems = new List<string>(menu.Keys);
                MenuPrices = new List<decimal>(menu.Values);
            }
        }
    }


public void EditMenuItem(int index, string newName, decimal newPrice)
    {
        if (index >= 0 && index < MenuItems.Count)
        {
            MenuItems[index] = newName;
            MenuPrices[index] = newPrice;
            Console.WriteLine("Item edited successfully.");
        }
        else
        {
            Console.WriteLine("Invalid item index.");
        }
    }

    public void RemoveMenuItem(int index)
    {
        if (index >= 0 && index < MenuItems.Count)
        {
            MenuItems.RemoveAt(index);
            MenuPrices.RemoveAt(index);
            Console.WriteLine("Item removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid item index.");
        }
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        List<string> menuItems = new List<string>();  
        List<decimal> menuPrices = new List<decimal>();
        List<int> orderItems = [];
        Dictionary<string, string> users = [];
        string? currentUser = null;
        PointOfSale pos = new PointOfSale();

        string dataDirectory = "data";
        string usersFile = Path.Combine(dataDirectory, "users.json");
        string menuFile = Path.Combine(dataDirectory, "menu.json");
        string ordersFile = Path.Combine(dataDirectory, "orders.json");

        Directory.CreateDirectory(dataDirectory);

        Console.WriteLine("Do you want to (1) Login or (2) Register?");
        string loginChoice = Console.ReadLine();

        if (loginChoice == "2")
        {
            Console.WriteLine("Enter new username: ");
            string newUsername = Console.ReadLine();
            Console.WriteLine("Enter new password: ");
            string newPassword = Console.ReadLine();
            currentUser = new Identity(usersFile).Register(newUsername, newPassword);
            if (string.IsNullOrEmpty(currentUser))
            {
                Console.WriteLine("Registration failed.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            currentUser = new Identity(usersFile).Login(username, password);
            if (string.IsNullOrEmpty(currentUser))
            {
                Console.WriteLine("Invalid login. Exiting...");
                return;
            }
        }

        Menu menu = new Menu(menuFile);
        menuItems = menu.MenuItems;
        menuPrices = menu.MenuPrices;

        LoadOrders();

        bool running = true;

        while (running)
        {
            Console.WriteLine("Welcome to the Coffee Shop!");
            Console.WriteLine("1. Add Menu Item");
            Console.WriteLine("2. View Menu");
            Console.WriteLine("3. Place Order");
            Console.WriteLine("4. View Order");
            Console.WriteLine("5. Calculate Total");
            Console.WriteLine("6. Exit");
            Console.WriteLine("7. Edit Menu Item");
            Console.WriteLine("8. Remove Menu Item");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddMenuItem();
                    break;
                case "2":
                    ViewMenu();
                    break;
                case "3":
                    PlaceOrder();
                    break;
                case "4":
                    ViewOrder();
                    break;
                case "5":
                    pos.CalculateTotal(orderItems, menuPrices, currentUser);
                    break;
                case "6":
                    running = false;
                    break;
                case "7":
                    Console.WriteLine("Enter the item number to edit: ");
                    int editIndex = int.Parse(Console.ReadLine()) - 1;
                    Console.WriteLine("Enter new item name: ");
                    string newName = Console.ReadLine();
                    Console.WriteLine("Enter new item price: ");
                    decimal newPrice = decimal.Parse(Console.ReadLine());
                    menu.EditMenuItem(editIndex, newName, newPrice);
                    break;
                case "8":
                    Console.WriteLine("Enter the item number to remove: ");
                    int removeIndex = int.Parse(Console.ReadLine()) - 1;
                    menu.RemoveMenuItem(removeIndex);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveMenu();
        SaveOrders();

        void SaveMenu()
        {
            var menu = new Dictionary<string, decimal>();
            for (int i = 0; i < menuItems.Count; i++)
            {
                menu.Add(menuItems[i], menuPrices[i]);
            }
            string json = JsonSerializer.Serialize(menu);
            File.WriteAllText(menuFile, json);
        }

        void LoadOrders()
        {
            if (File.Exists(ordersFile))
            {
                string json = File.ReadAllText(ordersFile);
                orderItems = JsonSerializer.Deserialize<List<int>>(json);
            }
        }

        void SaveOrders()
        {
            string json = JsonSerializer.Serialize(orderItems);
            File.WriteAllText(ordersFile, json);
        }

        void AddMenuItem()
        {
            Console.Write("Enter item name: ");
            string itemName = Console.ReadLine();
            Console.Write("Enter item price: ");
            decimal itemPrice = decimal.Parse(Console.ReadLine());

            menuItems.Add(itemName);
            menuPrices.Add(itemPrice);

            Console.WriteLine("Item added successfully!");
        }

        void ViewMenu()
        {
            Console.Clear();
            Console.WriteLine("Menu:");
            for (int i = 0; i < menuItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menuItems[i]} - {menuPrices[i]:C}");
            }
        }

        void PlaceOrder()
        {
            ViewMenu();
            Console.Write("Enter the item number to order: ");
            int itemNumber = int.Parse(Console.ReadLine()) - 1;

            if (itemNumber >= 0 && itemNumber < menuItems.Count)
            {
                orderItems.Add(itemNumber);
                Console.WriteLine("Item added to order!");
            }
            else
            {
                Console.WriteLine("Invalid item number. Please try again.");
            }
        }

        void ViewOrder()
        {
            Console.WriteLine("Your Order:");
            foreach (int itemIndex in orderItems)
            {
                Console.WriteLine($"{menuItems[itemIndex]} - {menuPrices[itemIndex]:C}");
            }
        }
    }
}
