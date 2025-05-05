using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessRightsController : ControllerBase
    {
        private readonly IGenericRepository<AccessRight> _accessRightRepository;

        public AccessRightsController(
            IGenericRepository<AccessRight> accessRightRepository)
        {
            _accessRightRepository = accessRightRepository;
        }

        // GET: api/AccessRights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessRightDto>>> GetAccessRights()
        {
            var accessRights = await _accessRightRepository.GetAllAsync();

            var accessRightDtos = accessRights.Select(ar => new AccessRightDto
            {
                Id = ar.Id,
                Description = ar.Description
            }).ToList();

            return Ok(accessRightDtos);
        }

        // GET: api/AccessRights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccessRightDto>> GetAccessRight(Guid id)
        {
            var accessRight = await _accessRightRepository.GetByIdAsync(id);

            if (accessRight == null)
            {
                return NotFound();
            }

            var accessRightDto = new AccessRightDto
            {
                Id = accessRight.Id,
                Description = accessRight.Description
            };

            return Ok(accessRightDto);
        }

        // POST: api/AccessRights
        [HttpPost]
        public async Task<ActionResult<AccessRightDto>> CreateAccessRight(AccessRightCreateDto accessRightCreateDto)
        {
            var accessRight = new AccessRight
            {
                Description = accessRightCreateDto.Description
            };

            await _accessRightRepository.AddAsync(accessRight);
            await _accessRightRepository.SaveChangesAsync();

            var accessRightDto = new AccessRightDto
            {
                Id = accessRight.Id,
                Description = accessRight.Description
            };

            return CreatedAtAction(nameof(GetAccessRight), new { id = accessRightDto.Id }, accessRightDto);
        }

        // PUT: api/AccessRights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccessRight(Guid id, AccessRightCreateDto accessRightUpdateDto)
        {
            var accessRight = await _accessRightRepository.GetByIdAsync(id);

            if (accessRight == null)
            {
                return NotFound();
            }

            accessRight.Description = accessRightUpdateDto.Description;

            _accessRightRepository.Update(accessRight);
            await _accessRightRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/AccessRights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccessRight(Guid id)
        {
            var accessRight = await _accessRightRepository.GetByIdAsync(id);

            if (accessRight == null)
            {
                return NotFound();
            }

            _accessRightRepository.Remove(accessRight);
            await _accessRightRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}