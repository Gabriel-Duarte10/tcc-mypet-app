using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Request
{
    public class UserCreateRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Cellphone { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public List<IFormFile> Images { get; set; }
    }
    public class UserUpdateRequest
    {
        public string Name { get; set; }
        public string Cellphone { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
