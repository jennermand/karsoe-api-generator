using System.Text;
using System.Text.Json;
using ManagerApi.Models;
using Microsoft.Extensions.Logging;

namespace ManagerApi;

/// <summary>
/// API client for Manager API - Full Access.
/// Complete API for administrative clients
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the ApiClient.
    /// </summary>
    public ApiClient(HttpClient httpClient, ILogger<ApiClient>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    #region Admin

    /// <summary>
    /// Get /gins
    /// </summary>
    public async Task<List<GinsDTO>> GetGinsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetGinsAsync");

        var url = $"/gins";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<GinsDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /ginById/{id}
    /// </summary>
    public async Task<RelatedProductDTO> GetGinByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetGinByIdAsync");

        var url = $"/ginById/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RelatedProductDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /product/{id}
    /// </summary>
    public async Task<ProductDTO> GetProductAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductAsync");

        var url = $"/product/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ProductDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /products
    /// </summary>
    public async Task<List<ProductDTO>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductsAsync");

        var url = $"/products";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Post /updateProductsProductType
    /// </summary>
    public async Task<ProductShortDTO> CreateUpdateProductsProductTypeAsync(ProductDTO request, object productTypeId = default!, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateUpdateProductsProductTypeAsync");

        var url = $"/updateProductsProductType";
        var queryParams = new List<string>();
        if (productTypeId != default) queryParams.Add($"productTypeId={productTypeId}");
        if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ProductShortDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /productsShort
    /// </summary>
    public async Task<List<ProductShortDTO>> GetProductsShortAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductsShortAsync");

        var url = $"/productsShort";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductShortDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Gets all product types.
    /// </summary>
    public async Task<List<ProductTypeDto>> GetAllProductTypesAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetAllProductTypesAsync");

        var url = $"/productTypes";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductTypeDto>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Creates a new product type.
    /// </summary>
    public async Task CreateProductTypeAsync(ProductTypeDto request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateProductTypeAsync");

        var url = $"/productTypes";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Gets a product type by its ID.
    /// </summary>
    public async Task GetProductTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductTypeByIdAsync");

        var url = $"/productTypes/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Updates an existing product type.
    /// </summary>
    public async Task UpdateProductTypeAsync(int id, ProductTypeDto request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateProductTypeAsync");

        var url = $"/productTypes/{id}";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /relatedproducts
    /// </summary>
    public async Task<List<RelatedProductDTO>> GetRelatedproductsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetRelatedproductsAsync");

        var url = $"/relatedproducts";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<RelatedProductDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Post /relatedproducts
    /// </summary>
    public async Task CreateRelatedproductsAsync(RelatedProductDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateRelatedproductsAsync");

        var url = $"/relatedproducts";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /relatedproductsById/{id}
    /// </summary>
    public async Task<RelatedProductDTO> GetRelatedproductsByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetRelatedproductsByIdAsync");

        var url = $"/relatedproductsById/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RelatedProductDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /relatedproducts/name/{name}
    /// </summary>
    public async Task GetRelatedproductsNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetRelatedproductsNameAsync");

        var url = $"/relatedproducts/name/{name}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /relatedproducts/producttype/{productTypeId}
    /// </summary>
    public async Task GetRelatedproductsProducttypeAsync(long productTypeId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetRelatedproductsProducttypeAsync");

        var url = $"/relatedproducts/producttype/{productTypeId}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /relatedproducts/producttype/{productTypeId}/name/{name}
    /// </summary>
    public async Task GetRelatedproductsProducttypeNameAsync(long productTypeId, string name, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetRelatedproductsProducttypeNameAsync");

        var url = $"/relatedproducts/producttype/{productTypeId}/name/{name}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }



    /// <summary>
    /// Put /relatedproducts/{id}
    /// </summary>
    public async Task UpdateRelatedproductsAsync(long id, RelatedProductDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateRelatedproductsAsync");

        var url = $"/relatedproducts/{id}";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Delete /relatedproducts/{id}
    /// </summary>
    public async Task DeleteRelatedproductsAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling DeleteRelatedproductsAsync");

        var url = $"/relatedproducts/{id}";

        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Delete /hardrelatedproducts/{id}
    /// </summary>
    public async Task DeleteHardrelatedproductsAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling DeleteHardrelatedproductsAsync");

        var url = $"/hardrelatedproducts/{id}";

        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Put /toggleSuggestion/{id}
    /// </summary>
    public async Task<bool> UpdateToggleSuggestionAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateToggleSuggestionAsync");

        var url = $"/toggleSuggestion/{id}";

        var response = await _httpClient.PutAsync(url, null, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<bool>(responseContent, _jsonOptions);
    }

    /// <summary>
    /// Post /generateDescription
    /// </summary>
    public async Task<RelatedProductDTO> CreateGenerateDescriptionAsync(string ginName = default!, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateGenerateDescriptionAsync");

        var url = $"/generateDescription";
        var queryParams = new List<string>();
        if (ginName != default) queryParams.Add($"ginName={ginName}");
        if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

        var response = await _httpClient.PostAsync(url, null, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RelatedProductDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /Test
    /// </summary>
    public async Task GetTestAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetTestAsync");

        var url = $"/Test";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /completeProductInfo/{id}
    /// </summary>
    public async Task<GinModel> GetCompleteProductInfoAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetCompleteProductInfoAsync");

        var url = $"/completeProductInfo/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GinModel>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /productvariants
    /// </summary>
    public async Task<List<ProductVariantDTO>> GetProductvariantsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductvariantsAsync");

        var url = $"/productvariants";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductVariantDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Post /productvariants
    /// </summary>
    public async Task CreateProductvariantsAsync(ProductVariantDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateProductvariantsAsync");

        var url = $"/productvariants";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }


    /// <summary>
    /// Put /productvariants/{id}
    /// </summary>
    public async Task UpdateProductvariantsAsync(long id, ProductVariantDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateProductvariantsAsync");

        var url = $"/productvariants/{id}";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Delete /productvariants/{id}
    /// </summary>
    public async Task DeleteProductvariantsAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling DeleteProductvariantsAsync");

        var url = $"/productvariants/{id}";

        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /productvariantsByProductId/{id}
    /// </summary>
    public async Task<List<ProductVariantDTO>> GetProductvariantsByProductIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetProductvariantsByProductIdAsync");

        var url = $"/productvariantsByProductId/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductVariantDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /GetAllDistilleries
    /// </summary>
    public async Task<List<DistilleryDTO>> GetGetAllDistilleriesAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetGetAllDistilleriesAsync");

        var url = $"/GetAllDistilleries";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<DistilleryDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Post /distillery
    /// </summary>
    public async Task CreateDistilleryAsync(DistilleryDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateDistilleryAsync");

        var url = $"/distillery";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /distillery/{id}
    /// </summary>
    public async Task GetDistilleryAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetDistilleryAsync");

        var url = $"/distillery/{id}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Put /distillery/{id}
    /// </summary>
    public async Task UpdateDistilleryAsync(long id, DistilleryDTO request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateDistilleryAsync");

        var url = $"/distillery/{id}";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Delete /distillery/{id}
    /// </summary>
    public async Task DeleteDistilleryAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling DeleteDistilleryAsync");

        var url = $"/distillery/{id}";

        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /imageAlbums
    /// </summary>
    public async Task<List<SharedAlbumDTO>> GetImageAlbumsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetImageAlbumsAsync");

        var url = $"/imageAlbums";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<SharedAlbumDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /images
    /// </summary>
    public async Task<List<ImageDTO>> GetImagesAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetImagesAsync");

        var url = $"/images";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ImageDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Put /createRelatedProductImageAlbums/{id}
    /// </summary>
    public async Task<RelatedProductDTO> UpdateCreateRelatedProductImageAlbumsAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling UpdateCreateRelatedProductImageAlbumsAsync");

        var url = $"/createRelatedProductImageAlbums/{id}";

        var response = await _httpClient.PutAsync(url, null, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<RelatedProductDTO>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    #endregion

    #region Auth

    /// <summary>
    /// Post /auth/token
    /// </summary>
    public async Task GetTokenAsync(TokenRequest request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetTokenAsync");

        var url = $"/auth/token";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    #endregion

    #region Debug

    /// <summary>
    /// Get /debug/token
    /// </summary>
    public async Task GetDebugTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetDebugTokenAsync");

        var url = $"/debug/token";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    #endregion

    #region Diagnostics

    /// <summary>
    /// Get /auth/ping
    /// </summary>
    public async Task<AnonymousTypeOfstringAndstringAndDateTime> GetAuthPingAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetAuthPingAsync");

        var url = $"/auth/ping";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AnonymousTypeOfstringAndstringAndDateTime>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /auth/whoami
    /// </summary>
    public async Task GetAuthWhoamiAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetAuthWhoamiAsync");

        var url = $"/auth/whoami";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Post /auth/decode-token
    /// </summary>
    public async Task CreateAuthDecodetokenAsync(DecodeTokenRequest request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling CreateAuthDecodetokenAsync");

        var url = $"/auth/decode-token";

        var content = new StringContent(
            JsonSerializer.Serialize(request, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    /// <summary>
    /// Get /auth/config
    /// </summary>
    public async Task GetAuthConfigAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetAuthConfigAsync");

        var url = $"/auth/config";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

    }

    #endregion

    #region Public

    /// <summary>
    /// Get /web/gins
    /// </summary>
    public async Task<List<GinsDTO>> GetWebGinsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetWebGinsAsync");

        var url = $"/web/gins";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<GinsDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }


    /// <summary>
    /// Get /filter/GinVariants
    /// </summary>
    public async Task<List<ProductVariantDTO>> GetFilterGinVariantsAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetFilterGinVariantsAsync");

        var url = $"/filter/GinVariants";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ProductVariantDTO>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /filter/CountryNames
    /// </summary>
    public async Task<List<string>> GetFilterCountryNamesAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetFilterCountryNamesAsync");

        var url = $"/filter/CountryNames";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Get /filter/DistilleryNames
    /// </summary>
    public async Task<List<KeyValuePairOflongAndstring>> GetFilterDistilleryNamesAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Calling GetFilterDistilleryNamesAsync");

        var url = $"/filter/DistilleryNames";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCodeAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<KeyValuePairOflongAndstring>>(responseContent, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    #endregion

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger?.LogError("API request failed with status {StatusCode}: {Content}",
                response.StatusCode, content);
            throw new HttpRequestException(
                $"Request failed with status {response.StatusCode}: {content}",
                null,
                response.StatusCode);
        }
    }
}
