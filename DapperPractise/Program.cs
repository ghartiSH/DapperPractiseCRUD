using Dapper;
using DapperPractise;
using System.Data;
using System.Data.SqlClient;

var connectionString = @"Server=DESKTOP-PQF1NAD; Database=FileTest;Trusted_Connection=True;TrustServerCertificate=True";

RunApp();

void RunApp()
{
    string ans = null;
    do
    {
        Console.WriteLine("Welcome to Dapper Test App\nPlease select any Option to perform Dapper task");
        Console.WriteLine("1 to 'Get all the products'");
        Console.WriteLine("2 to 'Get product by Product Name'");
        Console.WriteLine("3 to 'Insert New Product'");
        Console.WriteLine("4 to 'Update an existing Product'");
        Console.WriteLine("5 to 'Delete a product'");
        Console.WriteLine("6 to 'Get product by Product Id'\n");

        Console.WriteLine("Enter your option");
        int option = Convert.ToInt32(Console.ReadLine());

        switch (option)
        {
            case 1:
                var productList = GetAllProducts();

                foreach (var item in productList)
                {
                    Console.WriteLine($"ID: {item.ProductID} , Name: {item.ProductName}, Price:{item.Price}, Category:{item.Category.CategoryName}");
                }
                break;

            case 2:
                Console.WriteLine("Enter the name of product");
                string name = Console.ReadLine();

                var prod = GetProductByName(name);
                Console.WriteLine($"ID: {prod.ProductID} , Name: {prod.ProductName}, Price:{prod.Price}, Category:{prod.Category.CategoryName}");
                break;

            case 3:
                InsertProduct();
                break;

            case 4:
                Console.WriteLine("Enter the ID of the product to update");
                int id = Convert.ToInt32(Console.ReadLine());
                UpdateProduct(id);
                break;

            case 5:
                Console.WriteLine("Enter the ID of the product to delete");
                int pid = Convert.ToInt32(Console.ReadLine());
                DeleteProduct(pid);
                break;
            case 6:
                Console.WriteLine("Enter the ID of the product to Get");
                int spid = Convert.ToInt32(Console.ReadLine());
                GetProductByID(spid);
                break;

            default:
                Console.WriteLine("Enter the valid Option");
                break;

        }
        Console.WriteLine("Do you want to continue? y/n ");
        ans = Console.ReadLine();

    } while (ans!= "n");


}

List<Product>GetAllProducts()
{
    List<Product> products = new List<Product>();

    using (IDbConnection conn = new SqlConnection(connectionString))
    {
        var sql = "select * from products p join categories c on p.catID = c.categoryID";

        products = conn.Query<Product, Category, Product>(sql, (product, category) =>
        {
            product.Category = category;
            return product;
        }, splitOn: "CategoryID").ToList();
    }
    return products;
}

void GetProductByID(int id)
{
    using (IDbConnection conn = new SqlConnection(connectionString))
    {
        var sql = "exec [ProductDetailsByID] @pid";
        var values = new { pid = id };
        var results =conn.Query(sql, values).ToList();

        if (results.Count < 1)
        {
            Console.WriteLine("Sorry! No records found");
        }
        else
        {
            results.ForEach(x => Console.WriteLine($"ID:{x.ProductName}, Name:{x.ProductName}, Price:{x.Price}, Category:{x.CategoryName}"));
        }

        
    }
}

Product GetProductByName(string pdName)
{
    Product product = null;
    using (IDbConnection conn = new SqlConnection(connectionString))
    {
        var sql = "select * from products p join categories c on p.catID = c.categoryID where productname = '" + pdName + "'";
        var products = conn.Query<Product, Category, Product>(sql,(product, category) =>
        {
            product.Category = category;
            return product;
        }, splitOn: "CategoryID").ToList();
        products.ForEach(p => product = p);
    }
    return product;
}

void InsertProduct()
{
    try
    {
        Console.WriteLine("Enter the name of product");
        string productName = Console.ReadLine();

        Console.WriteLine("Enter the price of product");
        int price = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter the ID of Category");
        int cat = Convert.ToInt32(Console.ReadLine());

        using (IDbConnection conn = new SqlConnection(connectionString))
        {
            var sql = "insert into products values (@productName, @price, @categoryID)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@productName", productName);
            parameters.Add("@price", price);
            parameters.Add("@categoryID", cat);

            conn.Execute(sql, parameters);

        }
        Console.WriteLine("New Product Added SuccessFully");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void UpdateProduct(int prodID)
{
    try
    {
        Console.WriteLine("Enter Updated name of the product");
        string productName = Console.ReadLine();

        Console.WriteLine("Enter Updated price of the product");
        int price = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Enter ID of Updtated Category");
        int catID = Convert.ToInt32(Console.ReadLine());

        using (IDbConnection conn = new SqlConnection(connectionString))
        {
            var sql = "update products set ProductName = @updatedProductName, Price = @UpdatedPrice, CatID =@UpdatedCatID where ProductID = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@updatedProductName", productName);
            parameters.Add("@updatedPrice", price);
            parameters.Add("@id", prodID);
            parameters.Add("@UpdatedCatID", catID);

            conn.Execute(sql, parameters);

        }
        Console.WriteLine("Product Updated Successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

void DeleteProduct(int pid)
{
    try
    {
        using (IDbConnection conn = new SqlConnection(connectionString))
        {
            var sql = "delete from products where ProductID = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", pid);
            conn.Execute(sql, parameters);

        }
        Console.WriteLine("Product Deleted Successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
