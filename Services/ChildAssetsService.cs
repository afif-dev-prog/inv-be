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
    public class ChildAssetsService : IChildAssetsService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;

        public ChildAssetsService(DataContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        public async Task<List<ChildAsset>> GetChildAssetsList()
        {

            var childAssets = await Task.Run(() => (
                from c in _context.ChildAsset
                select c
            ).ToListAsync());

            return childAssets;
        }

        public async Task<ResponseMessage<string>> AddChildAsset(string parentId, ChildAsset childAsset)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                childAsset.Id = Guid.NewGuid().ToString();
                childAsset.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                childAsset.ParentId = parentId;
                await _context.ChildAsset.AddAsync(childAsset);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();



                response.Success = true;
                response.Message = "Child asset added successfully.";

                await _activityService.AddActivity(new Activity
                {
                    Description = $"Item '{childAsset.AssetNumber}' added as child to asset: {parentId} ",
                    CreatedOn = childAsset.CreatedOn,
                    ActionTaken = "ADD",
                    CreatedBy = childAsset.CreatedBy
                });
                return response;
            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Count = 0;
                return response;
            }

        }

        public async Task<ResponseMessage<string>> UpdateChildAsset(string id, ChildAsset updatedChildAsset)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingChildAsset = await _context.ChildAsset.AnyAsync(x => x.Id == id);
                if (!existingChildAsset)
                {
                    response.Success = false;
                    response.Message = "Child asset not found.";
                    return response;
                    // return Task.FromResult(response);
                }
                else
                {
                    var editData = await _context.ChildAsset.FirstOrDefaultAsync(x => x.Id == id);
                    editData.AssetNumber = updatedChildAsset.AssetNumber;
                    editData.Description = updatedChildAsset.Description;
                    editData.SerialNumber = updatedChildAsset.SerialNumber;
                    editData.PONumber = updatedChildAsset.PONumber;
                    editData.Brand = updatedChildAsset.Brand;
                    editData.ReceivingDate = updatedChildAsset.ReceivingDate;
                    editData.Model = updatedChildAsset.Model;
                    editData.Supplier = updatedChildAsset.Supplier;
                    editData.AssetLocation = updatedChildAsset.AssetLocation;
                    editData.AssetBranch = updatedChildAsset.AssetBranch;
                    editData.AssetDepartment = updatedChildAsset.AssetDepartment;
                    editData.Status = updatedChildAsset.Status;
                    editData.Type = updatedChildAsset.Type;
                    editData.Category = updatedChildAsset.Category;
                    editData.Quantity = updatedChildAsset.Quantity;
                    editData.Capacity = updatedChildAsset.Capacity;
                    editData.PricePerUnit = updatedChildAsset.PricePerUnit;
                    editData.TotalPrice = updatedChildAsset.TotalPrice;
                    editData.OwnerName = updatedChildAsset.OwnerName;
                    editData.OwnerDepartment = updatedChildAsset.OwnerDepartment;
                    editData.OwnerBranch = updatedChildAsset.OwnerBranch;
                    editData.UpdatedBy = updatedChildAsset.UpdatedBy;
                    editData.UpdatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();



                    response.Success = true;
                    response.Message = "Child asset updated successfully.";
                    return response;

                }
            }
            catch (System.Exception e)
            {
                await transaction.RollbackAsync();

                response.Success = false;
                response.Message = e.Message.ToString();

                return response;
            }
        }

        public async Task<ResponseMessage<string>> DeleteChildAsset(string id)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingChildAsset = await _context.ChildAsset.AnyAsync(x => x.Id == id);
                if (!existingChildAsset)
                {
                    response.Success = false;
                    response.Message = "Child asset not found.";
                    return response;
                    // return Task.FromResult(response);
                }
                else
                {
                    await _context.ChildAsset.Where(x => x.Id == id).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();


                    response.Success = true;
                    response.Message = "Child asset deleted successfully.";
                    return response;
                }
            }
            catch (System.Exception e)
            {
                await transaction.RollbackAsync();

                response.Success = false;
                response.Message = e.Message.ToString();

                return response;
            }
        }
    }
    public interface IChildAssetsService
    {
        Task<List<ChildAsset>> GetChildAssetsList();
        Task<ResponseMessage<string>> AddChildAsset(string parentId, ChildAsset childAsset);
        Task<ResponseMessage<string>> UpdateChildAsset(string id, ChildAsset updatedChildAsset);
        Task<ResponseMessage<string>> DeleteChildAsset(string id);
    }
}