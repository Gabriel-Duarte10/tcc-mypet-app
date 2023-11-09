using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Request
{
    public class PetRequest
    {
        public string Name { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public bool Gender { get; set; }
        public string Description { get; set; }
        public bool IsNeutered { get; set; }
        public bool IsVaccinated { get; set; }
        public bool AdoptionStatus { get; set; }

        // Foreign Keys
        public int CharacteristicId { get; set; }
        public int BreedId { get; set; }
        public int SizeId { get; set; }
        public int UserId { get; set; }

        public List<IFormFile> Images { get; set; }
    }

    public class FavoritePetRequest
    {
        public int PetId { get; set; }
        public int UserId { get; set; }
    }

    public class ReportedPetRequest
    {
        public int PetId { get; set; }
    }
    public class FilterModel
    {
        public int? AnimalTypeId { get; set; }
        public int? BreedId { get; set; }
        public int? CharacteristicId { get; set; }
        public int? SizeId { get; set; }
        public bool? Status { get; set; } // Você pode querer mudar para bool ou enum, dependendo de como está armazenando
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string State { get; set; }
    }
}
