using Catalog.API.Products.GetProductByCategory;

namespace Catalog.API.Products.GetProductsByCategoryByCategory
{
    //public record GetProductByCategorysRequest();
    public record GetProductByCategorysResponse(IEnumerable<Product> products);

    public class GetProductsByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));
                var response = result.Adapt<GetProductByCategorysResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProductByCategory")
            .Produces<GetProductByCategorysResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Producst by Category")
            .WithDescription("Get Product by Category");
        }
    }
}
