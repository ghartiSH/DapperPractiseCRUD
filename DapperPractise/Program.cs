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
        Console.WriteLine("Welcome to Dapper Test App \n Please select any Option to perform Dapper task");
        Console.WriteLine("1 to 'Get all the products'");
        Console.WriteLine("2 to 'Get product by Product Name'");
        Console.WriteLine("3 to 'Insert New Product'");
        Console.WriteLine("4 to 'Update an existing Product'");
        Console.WriteLine("5 to 'Delete a product'");

        int option = Convert.ToInt32(Console.ReadLine());

        switch (option)
        {
            case 1:
                var productList = GetAllProducts();

                foreach (var item in productList)
                {
                    Console.WriteLine($"{item.ProductID} {item.ProductName} {item.Price}");
                }
                break;

            case 2:
                Console.WriteLine("Enter the name of product");
                string name = Console.ReadLine();

                var prod = GetProductByName(name);
                Console.WriteLine($"{prod.ProductID} {prod.ProductName} {prod.Price}");
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
        var sql = "select * from products";

        products = conn.Query<Product>(sql).ToList();
       
    }
    return products;
}

Product GetProductByName(string pdName)
{
    Product product = null;
    using (IDbConnection conn = new SqlConnection(connectionString))
    {
        var sql = "select * from products where productname = @productName";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@productName", pdName);
        product = conn.QueryFirstOrDefault<Product>(sql, parameters);

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

        using (IDbConnection conn = new SqlConnection(connectionString))
        {
            var sql = "insert into products values (@productName, @price)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@productName", productName);
            parameters.Add("@price", price);

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

        using (IDbConnection conn = new SqlConnection(connectionString))
        {
            var sql = "update products set ProductName = @updatedProductName, Price = @UpdatedPrice where ProductID = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@updatedProductName", productName);
            parameters.Add("@updatedPrice", price);
            parameters.Add("@id", prodID);
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