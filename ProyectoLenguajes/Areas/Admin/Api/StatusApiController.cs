using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoLenguajes.Areas.Admin.Api
{
    [Area("Admin")]
    [Route("Admin/Api/[controller]")]
    [ApiController]
    public class StatusApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatusApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/api/StatusApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var statuses = _unitOfWork.Status.GetAll()
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.TimeToNextStatus,
                    s.NextStatusId
                })
                .ToList();

            return Ok(statuses);
        }

        // GET: Admin/api/StatusApi/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var status = _unitOfWork.Status.Get(s => s.Id == id);
            if (status == null)
                return NotFound(new { Success = false, Message = "Status not found." });

            return Ok(new
            {
                status.Id,
                status.Name,
                status.TimeToNextStatus,
                status.NextStatusId
            });
        }

        // POST: Admin/api/StatusApi
        [HttpPost]
        public IActionResult Create([FromBody] Status status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _unitOfWork.Status.Add(status);
            _unitOfWork.Save();

            return Ok(new { Success = true, Message = "Status created successfully.", Status = status });
        }

        // POST: Admin/api/StatusApi/updateTime
        [HttpPost("updateTime")]
        public IActionResult UpdateTime([FromBody] List<Status> updatedStatuses)
        {
            if (updatedStatuses == null || !updatedStatuses.Any())
                return BadRequest(new { Success = false, Message = "Empty status list." });

            foreach (var input in updatedStatuses)
            {
                var dbStatus = _unitOfWork.Status.Get(s => s.Id == input.Id);
                if (dbStatus != null)
                {
                    dbStatus.TimeToNextStatus = input.TimeToNextStatus;
                    dbStatus.NextStatusId = input.NextStatusId;
                    _unitOfWork.Status.Update(dbStatus);
                }
            }

            _unitOfWork.Save();
            return Ok(new { Success = true, Message = "Statuses updated successfully." });
        }

        // DELETE: Admin/api/StatusApi/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var status = _unitOfWork.Status.Get(s => s.Id == id);
            if (status == null)
                return NotFound(new { Success = false, Message = "Status not found." });

            _unitOfWork.Status.Remove(status);
            _unitOfWork.Save();

            return Ok(new { Success = true, Message = "Status deleted successfully." });
        }
    }
}
