using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Angular_MasterDetails.Models;
using Angular_MasterDetails.DTO;
using Newtonsoft.Json;

namespace Angular_MasterDetails.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingEntriesController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookingEntriesController(BookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this._env = env;
        }

        [HttpGet]
        [Route("GetSpots")]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            return await _context.Spots.ToListAsync();
        }
        
        [HttpGet]
        [Route("GetClients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        } 
       

        // GET: api/BookingEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingEntries()
        {
            List<BookingDTO> bookingSpots = new List<BookingDTO>();

            var allClients = _context.Clients.ToList();
            foreach (var client in allClients)
            {
                var spotList = _context.BookingEntries.Where(x => x.ClientId == client.ClientId).Select(x => new Spot { SpotId = x.SpotId }).ToList();
                bookingSpots.Add(new BookingDTO
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    BirthDate = client.BirthDate,
                    PhoneNo = client.PhoneNo,
                    MaritalStatus = client.MaritalStatus,
                    Picture = client.Picture,
                    SpotItems = spotList.ToArray()
                });
            }

         
            return bookingSpots;
        }


        // POST: api/BookingEntries
        [HttpPost]
        public async Task<ActionResult<BookingEntry>> PostBookingEntry([FromForm] BookingDTO bookingDTO)
        {

            var spotItems = JsonConvert.DeserializeObject<Spot[]>(bookingDTO.spotsStringify);

            Client client = new Client
            {
                ClientName = bookingDTO.ClientName,
                BirthDate = bookingDTO.BirthDate,
                PhoneNo = bookingDTO.PhoneNo,
                MaritalStatus = bookingDTO.MaritalStatus
            };

            if (bookingDTO.PictureFile != null)
            {
                var webroot = _env.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookingDTO.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await bookingDTO.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                client.Picture = fileName;
            }

            foreach (var item in spotItems)
            {
                var bookingEntry = new BookingEntry
                {
                    Client = client,
                    ClientId = client.ClientId,
                    SpotId = item.SpotId
                };
                _context.Add(bookingEntry);
            }

            await _context.SaveChangesAsync();

            return Ok(client);
        }

    }
}
