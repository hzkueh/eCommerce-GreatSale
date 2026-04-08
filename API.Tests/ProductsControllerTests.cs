using System.Net;
using System.Net.Http.Json;
using Core.Entities;
using Xunit;

namespace API.Tests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsOk_WithAllSeededProducts()
    {
        // Arrange
        _factory.SeedDatabase(db => db.Products.AddRange(
            new Product
            {
                Name = "Test Boot",
                Description = "A sturdy boot",
                Price = 99.99m,
                PictureUrl = "/images/boot.jpg",
                Type = "Boots",
                Brand = "TestBrand",
                QuantityInStock = 10
            },
            new Product
            {
                Name = "Test Gloves",
                Description = "Warm gloves",
                Price = 49.99m,
                PictureUrl = "/images/gloves.jpg",
                Type = "Gloves",
                Brand = "TestBrand",
                QuantityInStock = 5
            }
        ));

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenNoProductsExist()
    {
        // Arrange
        _factory.SeedDatabase(_ => { });

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.Empty(products);
    }

    [Fact]
    public async Task GetProductById_ReturnsOk_WithMatchingProduct()
    {
        // Arrange
        _factory.SeedDatabase(db => db.Products.Add(new Product
        {
            Id = 1,
            Name = "Running Shoe",
            Description = "Lightweight running shoe",
            Price = 129.99m,
            PictureUrl = "/images/shoe.jpg",
            Type = "Shoes",
            Brand = "SpeedBrand",
            QuantityInStock = 20
        }));

        // Act
        var response = await _client.GetAsync("/api/products/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Running Shoe", product.Name);
        Assert.Equal(129.99m, product.Price);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _factory.SeedDatabase(_ => { });

        // Act
        var response = await _client.GetAsync("/api/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
