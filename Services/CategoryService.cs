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
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;

        public CategoryService(DataContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        public async Task<ResponseMessage<List<Category>>> GetCategoryList()
        {
            ResponseMessage<List<Category>> response = new ResponseMessage<List<Category>>();
            response.Data = new List<Category>();

            try
            {
                var categories = await Task.Run(() => _context.Category.ToList());
                if (categories != null && categories.Count > 0)
                {
                    response.Data = categories;
                    response.Count = categories.Count;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No categories found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseMessage<string>> AddCategory(Category category)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                category.Id = Guid.NewGuid().ToString();
                category.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.Category.AddAsync(category);
                int result = await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                response.Success = true;
                response.Count = 1;
                response.Message = "Category added successfully.";
                await _activityService.AddActivity(new Activity
                {
                    Description = $"Category '{category.Categories}' added",
                    CreatedOn = category.CreatedOn,
                    ActionTaken = "ADD",
                    CreatedBy = category.CreatedBy
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

        public async Task<ResponseMessage<string>> EditCategory(string ctgryId, Category category)
        {
            var result = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkExistence = await _context.Category.AnyAsync(x => x.Id == ctgryId);

                if (!checkExistence)
                {
                    result.Success = false;
                    result.Message = $"No suc item with id: {ctgryId}";

                    return result;
                }
                else
                {
                    var editData = await _context.Category.FirstOrDefaultAsync(x => x.Id == ctgryId);

                    editData.Categories = category.Categories;
                    editData.CreatedBy = category.CreatedBy;
                    editData.LastUpdateOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                    editData.UpdatedBy = category.UpdatedBy;

                    await _context.Category.AddAsync(editData);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    await _activityService.AddActivity(new Activity
                    {
                        Description = $"Category with id: {ctgryId} is edited",
                        CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                        ActionTaken = "EDIT",
                        CreatedBy = category.UpdatedBy
                    });

                    result.Success = true;
                    result.Message = $"Item with id: {ctgryId} successfully edited!";

                    return result;
                }

            }
            catch (System.Exception e)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.Message = e.Message.ToString();

                return result;
            }
        }

        public async Task<ResponseMessage<string>> DeleteCategory(string ctgryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var response = new ResponseMessage<string>();

            var checkExist = await _context.Category.AnyAsync(x => x.Id == ctgryId);

            try
            {
                if (!checkExist)
                {
                    response.Message = $"Category with id: {ctgryId} not found or wrong id!";
                    response.Success = false;

                    return response;
                }
                else
                {

                    await _context.Category.Where(s => s.Id == ctgryId).ExecuteDeleteAsync();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.Success = true;
                    response.Message = $"Category with id: {ctgryId} deleted successfully!";

                    return response;

                }
            }
            catch (System.Exception ex)
            {

                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message.ToString();
                return response;
            }

        }
    }
    public interface ICategoryService
    {
        Task<ResponseMessage<List<Category>>> GetCategoryList();
        Task<ResponseMessage<string>> AddCategory(Category category);
        Task<ResponseMessage<string>> EditCategory(string ctgryId, Category category);
        Task<ResponseMessage<string>> DeleteCategory(string ctgryId);
    }
}