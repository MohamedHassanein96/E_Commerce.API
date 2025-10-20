using E_Commerce.CustomExceptions;
using E_Commerce.Extension;
using Stripe.Checkout;
using OneOf;

namespace E_Commerce.Services
{
    public class CartService(UserManager<ApplicationUser> userManager, ApplicationDbContext context , IHttpContextAccessor httpContextAccessor) : ICartService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<OneOf<bool, ErrorResponse>> AddToCartAsync(AddToCartRequest request, CancellationToken cancellationToken = default)
        {
      
            var product = await _context.Products.FindAsync(request.ProductId,cancellationToken);
            if (product == null)
                return new ErrorResponse("product not found");

            if (product.StockForReservation < request.Quantity)
                return new ErrorResponse($"available Quantity is {product.AvailableStock}");

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User.");


            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {

                product.ReservedStock += request.Quantity;

                var existingCartItem = await _context
                    .Carts
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId &&
                    x.ApplicationUserId == userId,
                    cancellationToken: cancellationToken);

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
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new ErrorResponse("An unexpected error occurred while processing your payment. Please try again later.");
            }
        }

        public async Task<OneOf<CartResponse,ErrorResponse>> GetCartDetailsAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");



            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User");

            var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);

            var totalPrice = cartItems.Sum(x => x.Product.Price * x.Quantity); 

            var details = cartItems.Select(x =>
            new CartDetailsResponse(x.Product.Name, x.Quantity)).ToList();

            return new CartResponse(details, totalPrice);
        }

        public async Task<OneOf<bool,ErrorResponse>> DecrementAsync(DecrementRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");


            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User");


            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
                if (product is null) return false;

                var cartItem = await _context.Carts.FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);

                if (cartItem is null) return false;

                cartItem.Quantity--;

                product.ReservedStock--; // التعديل المهم هنا

                if (cartItem.Quantity <= 0)
                    _context.Carts.Remove(cartItem);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }

        public async Task<OneOf<bool, ErrorResponse>> IncrementAsync(IncrementRequest request, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User");

            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);
            if (cart is null)
                return new ErrorResponse("Cart is Empty");

            cart.Quantity++;
            product.ReservedStock += 1;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<OneOf<bool,ErrorResponse>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User Not Found");

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return new ErrorResponse("Invalid User");

            var cartItem = await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);
            if (cartItem is null)
                return new ErrorResponse("Cart is Empty");

            product.ReservedStock -= cartItem.Quantity;
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<OneOf<PayResponse,ErrorResponse>> PayAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResponse("User not found.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return new ErrorResponse("Invalid user.");


            var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);

            if (!cartItems.Any())
                return new ErrorResponse("Cart is empty.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var order = new Order
                {
                    ApplicationUserId = userId,
                    Items = cartItems.Select(x => new OrderItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        UnitPrice = x.Product.Price
                    }).ToList(),
                    PaymentStatus = PaymentStatus.Pending
                };

                foreach (var item in cartItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);

                    if (product is null)
                        throw new Exception($"Product {item.Product.Name} not found.");

                    if (product.StockForReservation < item.Quantity)
                        throw new InsufficientStockException(item.Product.Name, item.Quantity, product.StockForReservation);

                    if (product.Version != item.Product.Version)
                        throw new Exception($"Product {item.Product.Name} has been modified.");

                  
                    product.Version++;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);


                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = "https://localhost:4200/checkout/success?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = $"{"https://localhost:4200"}/checkout/cancel",
                };

                foreach (var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Quantity,
                    });
                }
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                order.StripeSessionId = session.Id;
                await _context.SaveChangesAsync(cancellationToken);

                // حذف محتويات الكارت بعد تسجيل الأوردر
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                var invoiceUrl = $"https://yourapp.com/invoices/{session.Id}.pdf";

                return new PayResponse("Success", session.Id ,null, session.Url);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        } 
    }
}
