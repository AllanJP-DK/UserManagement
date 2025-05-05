using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Repositories;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IGenericRepository<Address> _addressRepository;

        public AddressesController(
            IGenericRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses()
        {
            var addresses = await _addressRepository.GetAllAsync();

            var addressDtos = addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Street = a.Street,
                PostalCode = a.PostalCode
            }).ToList();

            return Ok(addressDtos);
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDto>> GetAddress(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            var addressDto = new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                PostalCode = address.PostalCode
            };

            return Ok(addressDto);
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<AddressDto>> CreateAddress(AddressCreateDto addressCreateDto)
        {
            var address = new Address
            {
                Street = addressCreateDto.Street,
                PostalCode = addressCreateDto.PostalCode
            };

            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();

            var addressDto = new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                PostalCode = address.PostalCode
            };

            return CreatedAtAction(nameof(GetAddress), new { id = addressDto.Id }, addressDto);
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, AddressCreateDto addressUpdateDto)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            address.Street = addressUpdateDto.Street;
            address.PostalCode = addressUpdateDto.PostalCode;

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            _addressRepository.Remove(address);
            await _addressRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}