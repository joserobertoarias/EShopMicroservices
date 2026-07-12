using Basket.API.Data;

namespace Basket.API.Basket.DeleteBasket
{

    public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
    public record DeleteBasketResult(bool Success);

    public class DeleteBasketHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {
        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            bool success = await repository.DeleteBasket(command.UserName, cancellationToken);
            return new DeleteBasketResult(success);
        }
    }
}
