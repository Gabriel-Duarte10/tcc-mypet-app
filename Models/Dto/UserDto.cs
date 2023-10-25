using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cellphone { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public List<UserImageDto> UserImages { get; set; }
    }

    public class UserImageDto
    {
        public int UserId { get; set; }
        public string ImageName { get; set; }
        public string Image64 { get; set; }
    }
}
