using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;
using Microsoft.EntityFrameworkCore;

namespace inventory_v2.Services
{
    public class MovementCartService : IMovementCartService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;

        public MovementCartService(DataContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        // cart list

        public async Task<ResponseMessage<List<MovementCart>>> GetMovementCart()
        {
            ResponseMessage<List<MovementCart>> rs = new ResponseMessage<List<MovementCart>>();
            try
            {
                var cartList = await _context.MovementCart.ToListAsync();
                rs.Success = true;
                rs.Data = cartList;
                rs.Count = cartList.Count;
            }
            catch (System.Exception e)
            {

                rs.Success = false;
                rs.Message = $"Error: {e.Message}";
            }
            return rs;
        }

        // add to cart

        public async Task<ResponseMessage<string>> AddToMovementCart(MovementCart movementCart)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                movementCart.Id = Guid.NewGuid().ToString();
                movementCart.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                if (await IsAssetNumberInCart(movementCart.AssetInvolved))
                {
                    response.Success = false;
                    response.Message = "Item already exists in movement cart.";
                    return response; // ← MISSING RETURN HERE!
                }

                await _context.MovementCart.AddAsync(movementCart);
                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "Added to movement cart successfully.";


                // Handle both PascalCase and camelCase
                var jsonDoc = System.Text.Json.JsonDocument.Parse(movementCart.JsonObject);
                var assetNumber = jsonDoc.RootElement.TryGetProperty("AssetNumber", out var pascalProp)
                    ? pascalProp.GetString()
                    : jsonDoc.RootElement.GetProperty("assetNumber").GetString();

                await _activityService.AddActivity(new Activity
                {
                    Description = $"Asset '{assetNumber}' added to movement cart",
                    CreatedOn = movementCart.CreatedOn,
                    ActionTaken = "ADD_TO_CART",
                    CreatedBy = movementCart.CreatedBy
                });
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = $"Error: {ex.Message}"; // Removed redundant .ToString()
                return response;
            }
        }


        public async Task<ResponseMessage<string>> RemoveItemFromCart(string id)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var item = await _context.MovementCart.AnyAsync(x => x.Id == id);
                if (!item)
                {
                    response.Success = false;
                    response.Message = "Item not found in movement cart.";
                    return response;
                }
                else
                {
                    await _context.MovementCart.Where(x => x.Id == id).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var delData = await _context.MovementCart.FirstOrDefaultAsync(x => x.Id == id);

                    response.Success = true;
                    response.Message = "Item removed from movement cart successfully.";


                    await _activityService.AddActivity(new Activity
                    {
                        Description = $"Item with Asset '{delData.AssetInvolved}' removed from movement cart",
                        CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                        ActionTaken = "REMOVE_FROM_CART",
                        CreatedBy = delData.CreatedBy
                    });
                    return response;
                }

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
                return response;
            }
        }
        public async Task<bool> IsAssetNumberInCart(string assetNumber)
        {
            var exists = await _context.MovementCart.AnyAsync(mc => assetNumber == mc.AssetInvolved);
            if (exists)
            {
                return true;
            }
            else
            {

                return false;
            }
        }
    }
    public interface IMovementCartService
    {
        Task<ResponseMessage<List<MovementCart>>> GetMovementCart();
        Task<ResponseMessage<string>> AddToMovementCart(MovementCart movementCart);
        Task<ResponseMessage<string>> RemoveItemFromCart(string id);
    }
}