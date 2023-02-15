using Angular_MasterDetails.Models;

namespace Angular_MasterDetails.DTO
{
    public class BookingDTO
    {
       public int ClientId { get; set; }

        public string ClientName { get; set; }
        
        public DateTime BirthDate { get; set; }

        public int PhoneNo { get; set; }

        public string Picture { get; set; }

        public IFormFile PictureFile { get; set; }

        public bool MaritalStatus { get; set; }

        public string spotsStringify { get; set; }

        public Spot[] SpotItems { get; set; }
    }
}
