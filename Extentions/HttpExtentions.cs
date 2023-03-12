using System.Text.Json;
using Dating_App.Helpers;
using Microsoft.AspNetCore.Http.Json;

namespace Dating_App.Extentions;

public static class HttpExtentions
{
    public static void AddPaginationHeader(this HttpResponse respons , int currentPage
    , int itemsPrePage , int totalItems , int totalPages)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPrePage, totalItems, totalPages);
        var optins = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        respons.Headers.Add("Pagination" , JsonSerializer.Serialize(paginationHeader));
        respons.Headers.Add("Access-Control-Expose-Headers" , "Pagination");
    }
}