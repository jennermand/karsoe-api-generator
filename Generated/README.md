# Manager API - Full Access

Complete API for administrative clients

This is an auto-generated C# API client.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Authentication](#authentication)
- [Configuration](#configuration)
- [Usage Examples](#usage-examples)
- [Available Endpoints](#available-endpoints)
- [Models](#models)
- [Error Handling](#error-handling)
- [Advanced Features](#advanced-features)

## Installation

### Prerequisites

- .NET 8.0 or higher

### Required NuGet Packages

```bash
dotnet add package Microsoft.Extensions.Http
dotnet add package System.Text.Json
dotnet add package Microsoft.Extensions.Http.Polly
dotnet add package Polly
dotnet add package Microsoft.Extensions.Logging
```

## Quick Start

### 1. Add to Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using ManagerApi;

// In your startup/program configuration
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7405/");
});
```

### 2. Inject and Use

```csharp
public class MyService
{
    private readonly ApiClient _apiClient;

    public MyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task DoSomething()
    {
        // Call API methods
        var result = await _apiClient.GetSomeDataAsync();
    }
}
```

## Authentication

This API uses token-based authentication. Configure authentication when setting up the client:

```csharp
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7405/");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "your-token-here");
});
```

Or obtain a token dynamically:

```csharp
// First, authenticate to get a token
var token = await _apiClient.AuthTokenAsync(new TokenRequest
{
    Username = "your-username",
    Password = "your-password"
});

// Then configure the client with the token
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);
```

## Configuration

### Base URL

The default base URL is `http://localhost:7405/`. You can override it:

```csharp
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://your-custom-url.com");
});
```

### Retry Policy

The client includes automatic retry logic for transient failures:

```csharp
using Polly;
using Polly.Extensions.Http;

services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7405/");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

### Logging

The client supports logging through Microsoft.Extensions.Logging:

```csharp
// Logging is automatically configured if ILogger<ApiClient> is registered
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
```

## Usage Examples

### Admin

#### Get /gins

```csharp
var gins = await _apiClient.GetGinsAsync();
```

#### Get /product/{id}

```csharp
var product = await _apiClient.GetProductAsync();
```

### Public

#### Get /ginById/{id}

```csharp
var ginById = await _apiClient.GetGinByIdAsync();
```

#### Get /web/gins

```csharp
var webGins = await _apiClient.GetWebGinsAsync();
```

### Auth

#### Post /auth/token

```csharp
await _apiClient.GetTokenAsync();
```

## Available Endpoints

### Admin

| Method | HTTP | Path | Returns |
|--------|------|------|---------|
| `GetCompleteProductInfoAsync` | GET | `/completeProductInfo/{id}` | `GinModel` |
| `UpdateCreateRelatedProductImageAlbumsAsync` | PUT | `/createRelatedProductImageAlbums/{id}` | `RelatedProductDTO` |
| `CreateDistilleryAsync` | POST | `/distillery` | `void` |
| `GetDistilleryAsync` | GET | `/distillery/{id}` | `void` |
| `UpdateDistilleryAsync` | PUT | `/distillery/{id}` | `void` |
| `DeleteDistilleryAsync` | DELETE | `/distillery/{id}` | `void` |
| `CreateGenerateDescriptionAsync` | POST | `/generateDescription` | `RelatedProductDTO` |
| `GetGetAllDistilleriesAsync` | GET | `/GetAllDistilleries` | `List<DistilleryDTO` |
| `GetGinsAsync` | GET | `/gins` | `List<GinsDTO` |
| `DeleteHardrelatedproductsAsync` | DELETE | `/hardrelatedproducts/{id}` | `void` |
| `GetImageAlbumsAsync` | GET | `/imageAlbums` | `List<SharedAlbumDTO` |
| `GetImagesAsync` | GET | `/images` | `List<ImageDTO` |
| `GetProductAsync` | GET | `/product/{id}` | `ProductDTO` |
| `GetProductsAsync` | GET | `/products` | `List<ProductDTO` |
| `GetProductsShortAsync` | GET | `/productsShort` | `List<ProductShortDTO` |
| `GetAllProductTypesAsync` | GET | `/productTypes` | `List<ProductTypeDto` |
| `CreateProductTypeAsync` | POST | `/productTypes` | `void` |
| `GetProductTypeByIdAsync` | GET | `/productTypes/{id}` | `void` |
| `UpdateProductTypeAsync` | PUT | `/productTypes/{id}` | `void` |
| `GetProductvariantsAsync` | GET | `/productvariants` | `List<ProductVariantDTO` |
| `CreateProductvariantsAsync` | POST | `/productvariants` | `void` |
| `UpdateProductvariantsAsync` | PUT | `/productvariants/{id}` | `void` |
| `DeleteProductvariantsAsync` | DELETE | `/productvariants/{id}` | `void` |
| `GetProductvariantsByProductIdAsync` | GET | `/productvariantsByProductId/{id}` | `List<ProductVariantDTO` |
| `GetRelatedproductsAsync` | GET | `/relatedproducts` | `List<RelatedProductDTO` |
| `CreateRelatedproductsAsync` | POST | `/relatedproducts` | `void` |
| `UpdateRelatedproductsAsync` | PUT | `/relatedproducts/{id}` | `void` |
| `DeleteRelatedproductsAsync` | DELETE | `/relatedproducts/{id}` | `void` |
| `GetRelatedproductsNameAsync` | GET | `/relatedproducts/name/{name}` | `void` |
| `GetRelatedproductsProducttypeAsync` | GET | `/relatedproducts/producttype/{productTypeId}` | `void` |
| `GetRelatedproductsProducttypeNameAsync` | GET | `/relatedproducts/producttype/{productTypeId}/name/{name}` | `void` |
| `GetRelatedproductsByIdAsync` | GET | `/relatedproductsById/{id}` | `RelatedProductDTO` |
| `GetTestAsync` | GET | `/Test` | `void` |
| `UpdateToggleSuggestionAsync` | PUT | `/toggleSuggestion/{id}` | `bool` |
| `CreateUpdateProductsProductTypeAsync` | POST | `/updateProductsProductType` | `ProductShortDTO` |

### Auth

| Method | HTTP | Path | Returns |
|--------|------|------|---------|
| `GetTokenAsync` | POST | `/auth/token` | `void` |

### Debug

| Method | HTTP | Path | Returns |
|--------|------|------|---------|
| `GetDebugTokenAsync` | GET | `/debug/token` | `void` |

### Diagnostics

| Method | HTTP | Path | Returns |
|--------|------|------|---------|
| `GetAuthConfigAsync` | GET | `/auth/config` | `void` |
| `CreateAuthDecodetokenAsync` | POST | `/auth/decode-token` | `void` |
| `GetAuthPingAsync` | GET | `/auth/ping` | `AnonymousTypeOfstringAndstringAndDateTime` |
| `GetAuthWhoamiAsync` | GET | `/auth/whoami` | `void` |

### Public

| Method | HTTP | Path | Returns |
|--------|------|------|---------|
| `GetFilterCountryNamesAsync` | GET | `/filter/CountryNames` | `List<string` |
| `GetFilterDistilleryNamesAsync` | GET | `/filter/DistilleryNames` | `List<KeyValuePairOflongAndstring` |
| `GetFilterGinVariantsAsync` | GET | `/filter/GinVariants` | `List<ProductVariantDTO` |
| `GetGinByIdAsync` | GET | `/ginById/{id}` | `RelatedProductDTO` |
| `GetWebGinsAsync` | GET | `/web/gins` | `List<GinsDTO` |

## Models

The client includes 18 data transfer object (DTO) models:

### AnonymousTypeOfstringAndstringAndDateTime

AnonymousTypeOfstringAndstringAndDateTime data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Status` | `string` |  |
| `Message` | `string` |  |
| `Timestamp` | `DateTime` |  |

### DecodeTokenRequest

DecodeTokenRequest data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Token` | `string` |  |

### DistilleryDTO

DistilleryDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Country` | `string?` |  |
| `Description` | `string?` |  |
| `FoundedYear` | `object?` |  |
| `Name` | `string?` |  |
| `Region` | `string?` |  |
| `Website` | `string?` |  |

### GinModel

GinModel data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string?` |  |
| `Type` | `string?` |  |
| `Country` | `string?` |  |
| `Region` | `string?` |  |
| `Abv` | `string?` |  |
| `Distillery` | `string?` |  |
| `DistilleryGuess` | `string?` |  |
| `DistilleryHomePage` | `string?` |  |

### GinsDTO

GinsDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string` |  |
| `ProductType` | `string?` |  |
| `ProductVariant` | `string?` |  |
| `Products` | `List<ProductDTO>` |  |
| `Created` | `DateTime?` |  |
| `Updated` | `DateTime?` |  |
| `Deleted` | `DateTime?` |  |
| `Suggested` | `DateTime?` |  |
| `Description` | `string?` |  |
| `ImageUrl` | `string?` |  |
| `Problems` | `object?` |  |
| `SharedAlbum` | `object?` |  |
| `Distillery` | `object?` |  |
| `Webshops` | `List<WebShopDTO>` |  |

### ImageDTO

ImageDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `ImageId` | `object?` |  |
| `Imageguid` | `string?` |  |
| `ImageType` | `string?` |  |
| `ImageUrl` | `string?` |  |
| `RecordedAt` | `DateTime?` |  |
| `ProductId` | `object?` |  |
| `LocalPath` | `string?` |  |

### KeyValuePairOflongAndstring

KeyValuePairOflongAndstring data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Key` | `object` |  |
| `Value` | `string` |  |

### PriceDTO

PriceDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `RecordedAt` | `DateTime?` |  |
| `Value` | `object?` |  |
| `ProductId` | `object?` |  |
| `WebshopId` | `object?` |  |

### ProblemsDTO

ProblemsDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Duplicates` | `List<ProductRelationsDuplicateDTO>` |  |
| `Errors` | `List<string>` |  |
| `Warnings` | `List<string>` |  |
| `Infos` | `List<string>` |  |
| `HasProblems` | `bool?` |  |

### ProductDTO

ProductDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string?` |  |
| `Created` | `DateTime?` |  |
| `Updated` | `DateTime?` |  |
| `ProductType` | `string?` |  |
| `ProductTypeId` | `object?` |  |
| `WebshopName` | `string?` |  |
| `RelatedProductId` | `object?` |  |
| `ImageUrl` | `string?` |  |
| `Visible` | `bool?` |  |
| `Description` | `string?` |  |
| `Notes` | `string?` |  |
| `Price` | `object?` |  |
| `SourceUrl` | `string?` |  |
| `DistilleryId` | `object?` |  |
| `WebshopId` | `object?` |  |
| `LastUpdate` | `DateTime?` |  |
| `Json` | `string?` |  |
| `Imageguid` | `string?` |  |
| `Distillery` | `object?` |  |
| `Images` | `List<ImageDTO>` |  |
| `Prices` | `List<PriceDTO>` |  |
| `ProducttypeNavigation` | `object?` |  |
| `Webshop` | `object?` |  |

### ProductRelationsDuplicateDTO

ProductRelationsDuplicateDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Productid` | `object?` |  |
| `Relation` | `object?` |  |
| `Created` | `DateTime?` |  |
| `Updated` | `DateTime?` |  |
| `Deleted` | `DateTime?` |  |

### ProductShortDTO

ProductShortDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string?` |  |
| `Created` | `DateTime?` |  |
| `Updated` | `DateTime?` |  |
| `ProductType` | `string?` |  |
| `ProductTypeId` | `object?` |  |
| `Webshop` | `string?` |  |
| `RelatedProductId` | `object?` |  |
| `ImageUrl` | `string?` |  |
| `Visible` | `bool?` |  |

### ProductTypeDto

ProductTypeDto data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string?` |  |
| `Priority` | `object?` |  |

### ProductVariantDTO

ProductVariantDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string?` |  |
| `Describtion` | `string?` |  |
| `Produkttypeid` | `object?` |  |

### RelatedProductDTO

RelatedProductDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string` |  |
| `ProductType` | `string?` |  |
| `ProductVariant` | `string?` |  |
| `Products` | `List<ProductDTO>` |  |
| `Created` | `DateTime?` |  |
| `Updated` | `DateTime?` |  |
| `Deleted` | `DateTime?` |  |
| `Suggested` | `DateTime?` |  |
| `Description` | `string?` |  |
| `ImageUrl` | `string?` |  |
| `Problems` | `object?` |  |
| `SharedAlbum` | `object?` |  |
| `Distillery` | `object?` |  |
| `Webshops` | `List<WebShopDTO>` |  |

### SharedAlbumDTO

SharedAlbumDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Text` | `string?` |  |
| `Images` | `List<ImageDTO>` |  |

### TokenRequest

TokenRequest data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Username` | `string` |  |
| `Password` | `string` |  |

### WebShopDTO

WebShopDTO data transfer object

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `object?` |  |
| `Name` | `string?` |  |
| `Url` | `string?` |  |
| `Description` | `string?` |  |
| `LogoUrl` | `string?` |  |
| `NickName` | `string?` |  |

## Error Handling

The client throws `HttpRequestException` for failed requests:

```csharp
try
{
    var result = await _apiClient.GetDataAsync();
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // Handle 404 Not Found
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // Handle 401 Unauthorized
}
catch (HttpRequestException ex)
{
    // Handle other HTTP errors
}
```

## Advanced Features

### Cancellation Tokens

All methods support cancellation:

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var result = await _apiClient.GetDataAsync(cancellationToken: cts.Token);
```

### Custom Headers

Add custom headers to the HttpClient:

```csharp
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7405/");
    client.DefaultRequestHeaders.Add("X-Custom-Header", "value");
});
```

---

*Generated by karsoe-api-generator on 2026-02-14 08.42.13*

