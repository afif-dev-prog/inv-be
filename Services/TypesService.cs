using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;

namespace inventory_v2.Services
{
    public class TypesService : ITypesService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;

        public TypesService(DataContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        public async Task<ResponseMessage<List<Types>>> GetTypeList()
        {
            ResponseMessage<List<Types>> response = new ResponseMessage<List<Types>>();
            response.Data = new List<Types>();

            try
            {
                var types = await Task.Run(() => _context.Types.ToList());
                if (types != null && types.Count > 0)
                {
                    response.Data = types;
                    response.Count = types.Count;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No types found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseMessage<string>> AddType(Types type)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                type.Id = Guid.NewGuid().ToString();
                type.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.Types.AddAsync(type);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                response.Success = true;
                response.Message = "Type added successfully.";
                await _activityService.AddActivity(new Activity
                {
                    Description = $"Type '{type.Typess}' added",
                    CreatedOn = type.CreatedOn,
                    ActionTaken = "ADD",
                    CreatedBy = type.CreatedBy
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
    }
    public interface ITypesService
    {
        Task<ResponseMessage<List<Types>>> GetTypeList();
        Task<ResponseMessage<string>> AddType(Types type);
    }
}