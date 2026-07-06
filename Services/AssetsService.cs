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
    public class AssetsService : IAssetsService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;
        private readonly IMovementService _movementService;

        public AssetsService(DataContext context, IActivityService activityService, IMovementService movementService)
        {
            _context = context;
            _activityService = activityService;
            _movementService = movementService;
        }


        public async Task<List<Assets>> GetAssetsList()
        {
            return await _context.Assets
            .Include(a => a.ChildAsset)
            .ToListAsync();
            // return await _context.Assets.ToListAsync();
        }

        public async Task<ResponseMessage<List<Movement>>> GetMovementHistoryById(string id)
        {
            ResponseMessage<List<Movement>> rs = new ResponseMessage<List<Movement>>();
            try
            {
                var assetCheck = await _context.Assets.FindAsync(id);
                if (assetCheck == null)
                {
                    rs.Success = false;
                    rs.Message = "Asset not found.";

                }
                else
                {
                    var movements = await _movementService.GetMovementList();
                    if (movements.Success)
                    {
                        var assetMovements = movements?.Data?.Where(m => m.AssetId == id).ToList();
                        if (assetMovements?.Count > 0)
                        {

                            rs.Success = true;
                            rs.Data = assetMovements;
                            rs.Message = "Movement history retrieved successfully.";
                        }
                    }
                    else
                    {
                        rs.Success = false;
                        rs.Message = "No movement history found for this asset.";
                    }
                }
            }
            catch (System.Exception e)
            {
                rs.Success = false;
                rs.Message = e.Message.ToString();

            }

            return rs;
        }

        public async Task<PagedResult<Assets>> GetPaginatedAssetList(PaginationParams paginationParams, string? search = null)
        {
            var result = new PagedResult<Assets> { Data = new List<Assets>() };
            try
            {
                IQueryable<Assets> query = _context.Assets.Include(a => a.ChildAsset);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    query = query.Where(a =>
                    a.AssetNumber.Trim().ToLower().Contains(search) ||
                    a.Category.Trim().ToLower().Contains(search) ||
                    a.AssetBranch.Trim().ToLower().Contains(search) ||
                    a.Type.Trim().ToLower().Contains(search) ||
                    a.Description.Trim().ToLower().Contains(search) ||
                    a.OwnerName.Trim().ToLower().Contains(search));
                }

                query = query.OrderByDescending(a => a.UpdatedOn);

                var totalCount = await query.CountAsync();

                var assets = await query
                            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                            .Take(paginationParams.PageSize)
                            .ToListAsync();
                result.Success = true;
                result.Data = assets;
                result.Pagination = new PaginationMetadata
                {
                    CurrentPage = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize),
                    HasPrevious = paginationParams.PageNumber > 1,
                    HasNext = paginationParams.PageNumber * paginationParams.PageSize < totalCount
                };
            }
            catch (System.Exception e)
            {
                result.Success = false;
                result.Message = e.Message.ToString();

                return result;
            }
            return result;
        }

        public async Task<ResponseMessage<string>> AddAsset(Assets asset)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                asset.Id = Guid.NewGuid().ToString();
                asset.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                asset.UpdatedBy = "No data yet";
                asset.UpdatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                response.Success = true;
                response.Message = "Asset added successfully.";
                await _activityService.AddActivity(new Activity
                {
                    Description = $"Asset '{asset.AssetNumber}' added",
                    CreatedOn = asset.CreatedOn,
                    ActionTaken = "ADD",
                    CreatedBy = asset.CreatedBy
                });

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message.ToString();
                return response;
            }

        }

        public async Task<ResponseMessage<string>> UpdateAsset(string id, Assets updatedAsset)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var editData = await _context.Assets.FirstOrDefaultAsync(x => x.Id == id);

                // Update fields

                editData.UpdatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                editData.UpdatedBy = updatedAsset.UpdatedBy;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                response.Success = true;
                response.Message = $"Asset: {updatedAsset.AssetNumber} updated successfully.";
                await _activityService.AddActivity(new Activity
                {
                    Description = $"Asset '{updatedAsset.AssetNumber}' updated",
                    CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    ActionTaken = "UPDATE",
                    CreatedBy = editData.UpdatedBy
                });
                return response;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;

                return response;
            }

        }

        public async Task<ResponseMessage<string>> DeleteAsset(string id)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            var asset = await _context.Assets.AnyAsync(x => x.Id == id);

            try
            {
                if (!asset)
                {
                    response.Success = false;
                    response.Message = "Asset not found.";
                    return response;
                }
                else
                {
                    var delData = await _context.Assets.FirstOrDefaultAsync(x => x.Id == id);
                    await _context.Assets.Where(x => x.Id == id).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();

                    response.Success = true;
                    response.Message = $"Asset: {id} deleted successfully.";

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
    public interface IAssetsService
    {
        // Task<ResponseMessage<List<MappedAssets>>> GetAssetsList();
        Task<List<Assets>> GetAssetsList();
        Task<ResponseMessage<string>> AddAsset(Assets asset);
        Task<ResponseMessage<string>> UpdateAsset(string id, Assets updatedAsset);
        Task<ResponseMessage<string>> DeleteAsset(string id);
        Task<ResponseMessage<List<Movement>>> GetMovementHistoryById(string id);
    }
}