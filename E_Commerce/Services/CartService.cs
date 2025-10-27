namespace E_Commerce.Services
{
    public class CartService(UserManager<ApplicationUser> _userManager, ApplicationDbContext _context) : ICartService
    {
        public async Task<OneOf<bool, ErrorResponse>> AddToCartAsync(string userId, AddToCartRequest request, CancellationToken cancellationToken = default)
        {

            var product = await _context.Products.FindAsync(request.ProductId, cancellationToken);
            if (product == null)
                return new ErrorResponse("product not found", StatusCodes.Status404NotFound);

            if (product.StockForReservation < request.Quantity)
                return new ErrorResponse($"available Quantity is {product.StockForReservation}", StatusCodes.Status400BadRequest);

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User.", StatusCodes.Status401Unauthorized);


            product.ReservedStock += request.Quantity;

            var existingCartItem = await _context
                .Carts
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId
                 && x.ApplicationUserId == userId, cancellationToken);

            if (existingCartItem is null)
            {
                Cart cart = new()
                {
                    Quantity = request.Quantity,
                    ProductId = request.ProductId,
                    ApplicationUserId = userId!
                };
                await _context.Carts.AddAsync(cart, cancellationToken);
            }
            else
            {
                existingCartItem.Quantity += request.Quantity;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;


        }

        public async Task<OneOf<CartResponse, ErrorResponse>> GetCartDetailsAsync(string userId, CancellationToken cancellationToken = default)
        {

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User", StatusCodes.Status401Unauthorized);


            var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);


            var cartDetailsResponse = cartItems.Select(x => new CartDetailsResponse(x.Product.Name, x.Quantity)).ToList();
            var totalPrice = cartItems.Sum(x => x.Product.Price * x.Quantity);


            return new CartResponse(cartDetailsResponse, totalPrice);
        }

        public async Task<bool> DecrementAsync(string userId, DecrementRequest request, CancellationToken cancellationToken = default)
        {
            var cartItems = await _context.Carts
                                              .Include(c => c.Product)
                                              .FirstOrDefaultAsync(c =>
                                                  c.ApplicationUserId == userId &&
                                                  c.ProductId == request.ProductId,
                                                  cancellationToken);

            if (cartItems is null)
                return false;

            var product = cartItems.Product;

            cartItems.Quantity--;
            product.ReservedStock--;

            if (cartItems.Quantity <= 0)
                _context.Carts.Remove(cartItems);

            await _context.SaveChangesAsync(cancellationToken);

            return true;


        }

        public async Task<bool> IncrementAsync(string userId, IncrementRequest request, CancellationToken cancellationToken = default)
        {


            var cart = await _context.Carts
                                          .Include(c => c.Product)
                                          .FirstOrDefaultAsync(c =>
                                           c.ApplicationUserId == userId &&
                                           c.ProductId == request.ProductId,
                                           cancellationToken);

            if (cart is null)
                return false;

            var product = cart.Product;

            cart.Quantity ++;
            product!.ReservedStock ++;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, DeleteRequest request, CancellationToken cancellationToken = default)
        {
            var cart = await _context.Carts
                                      .Include(c => c.Product)
                                      .FirstOrDefaultAsync(c =>
                                          c.ApplicationUserId == userId &&
                                          c.ProductId == request.ProductId,
                                          cancellationToken);

            if (cart is null)
                return false;

                cart.Product.ReservedStock -= cart.Quantity;

            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }


    
    }
}
