using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;
using Microsoft.EntityFrameworkCore;

namespace inventory_v2.Services
{
    public class MovementService : IMovementService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;
        private readonly IMovementCartService _cartService;

        public MovementService(DataContext context, IActivityService activityService, IMovementCartService cartService)
        {
            _context = context;
            _activityService = activityService;
            _cartService = cartService;
        }

        public async Task<ResponseMessage<List<Movement>>> GetMovementList()
        {
            ResponseMessage<List<Movement>> response = new ResponseMessage<List<Movement>>();
            response.Data = new List<Movement>();

            try
            {
                var movements = await Task.Run(() => _context.Movement.ToList());
                if (movements != null && movements.Count > 0)
                {
                    response.Data = movements;
                    response.Count = movements.Count;
                }
                else
                {
                    response.Success = true;
                    response.Message = "No movements found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseMessage<string>> AddMovement(Movement movement)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var prevData = await _context.Assets.FirstOrDefaultAsync(x => x.Id == movement.AssetId);

                //movement detail
                movement.Id = Guid.NewGuid().ToString();
                movement.OriginLocation = prevData.AssetLocation;
                movement.OriginBranch = prevData.AssetBranch;
                movement.OriginDepartment = prevData.AssetDepartment;
                movement.PreviousOwnerName = prevData.OwnerName;
                movement.PreviousOwnerBranch = prevData.OwnerBranch;
                movement.PreviousOwnerDepartment = prevData.OwnerDepartment;
                movement.AssetNumber = prevData.AssetNumber;
                movement.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                await _context.Movement.AddAsync(movement);

                //asset update part
                prevData.AssetBranch = movement.NewBranch;
                prevData.AssetDepartment = movement.NewDepartment;
                prevData.AssetLocation = movement.NewLocation;
                prevData.OwnerBranch = movement.NewOwnerBranch;
                prevData.OwnerDepartment = movement.NewOwnerDepartment;
                prevData.OwnerName = movement.NewOwnerName;
                prevData.UpdatedBy = movement.CreatedBy;
                prevData.UpdatedOn = movement.CreatedOn;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.Success = true;
                response.Message = "Movement recorded successfully.";


                await _activityService.AddActivity(new Activity
                {
                    Description = $"Movement of Asset '{movement.AssetNumber}' from '{movement.OriginLocation}' to '{movement.NewLocation}' added",
                    CreatedOn = movement.CreatedOn,
                    ActionTaken = "MOVE_ASSET",
                    CreatedBy = movement.CreatedBy
                });

                return response;

            }
            catch (Exception ex)
            {
                await transaction.CommitAsync();
                response.Success = false;
                response.Message = ex.Message.ToString();


                return response;
            }

        }
    }
    public interface IMovementService
    {
        Task<ResponseMessage<List<Movement>>> GetMovementList();
        Task<ResponseMessage<string>> AddMovement(Movement movement);
    }
}