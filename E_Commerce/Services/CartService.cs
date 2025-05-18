using E_Commerce.CustomExceptions;
using E_Commerce.Extension;
using Stripe.Checkout;

namespace E_Commerce.Services
{
    public class CartService(UserManager<ApplicationUser> userManager, ApplicationDbContext context , IHttpContextAccessor httpContextAccessor) : ICartService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<bool> AddToCartAsync(AddToCartRequest request, CancellationToken cancellationToken = default)
        {
      
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return  false;

            Cart cart = new ()
            {
                Count = request.Count,
                ProductId = request.ProductId,
                ApplicationUserId = userId!

            };

            var existingCartItem = await _context.Carts.FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.ApplicationUserId == userId, cancellationToken: cancellationToken);
            if (existingCartItem is null)
            {
                await _context.Carts.AddAsync(cart, cancellationToken);
            }
            else
            {
                existingCartItem.Count += request.Count;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<CartResponse> GetCartDetailsAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null;



            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return null!;

            var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);

            var totalPrice = cartItems.Sum(x => x.Product.Price * x.Count);

            var details = cartItems.Select(x =>
            new CartDetailsResponse(x.Product.Name, x.Count)).ToList();

            return new CartResponse(details, totalPrice);
        }

        public async Task<bool> DecrementAsync(DecrementRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return false;
            
            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return false;

           var cartItem =  await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);
            if (cartItem is null )
                return false;
            
            cartItem.Count-- ;

            if (cartItem.Count <= 0)
                _context.Carts.Remove(cartItem);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> IncrementAsync(IncrementRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return false;

            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);
            if (cart is null)
                return false;
            
            cart.Count++;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;


            var user = await _userManager.FindByIdAsync(userId!);
            if (user is null)
                return false;

            var cartItem = await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, cancellationToken);
            if (cartItem is null)
                return false;
            
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PayResponse> PayAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null!;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null!;


            var CartItem = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);

            if (CartItem.Count <= 0)
                return null!;


            var order = new Order
            {
                ApplicationUserId = userId,
                Items = CartItem.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Count,
                    UnitPrice = x.Product.Price
                }).ToList(),
                PaymentStatus = PaymentStatus.Pending
            };
           
            foreach (var item in CartItem)
            {

                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
                if (product is null)
                {
                    throw new Exception($"Product {item.Product.Name} is Not Found.");
                }


                if (product.Quantity < item.Count)
                {
                   throw new InsufficientStockException(item.Product.Name, item.Count, item.Product.Quantity);
                }

                if (product.Version != item.Product.Version)
                {
                    throw new Exception($"Product {item.Product.Name} has been modified since the last one.");
                }
                product.Quantity -= item.Count;
                product.Version++;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{"https://localhost:4200"}/checkout/success",
                CancelUrl = $"{"https://localhost:4200"}/checkout/cancel",
            };

            foreach (var item in CartItem)
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
                    Quantity = item.Count,
                });
            }
            var service = new SessionService();
            var session = await service.CreateAsync(options);

            order.StripeSessionId = session.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return new PayResponse(session.Url);
        }
    }
}
