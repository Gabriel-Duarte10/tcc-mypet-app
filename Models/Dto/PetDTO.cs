using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Dto
{
    public class PetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public bool Gender { get; set; }
        public string Description { get; set; }
        public bool IsNeutered { get; set; }
        public bool IsVaccinated { get; set; }
        public bool AdoptionStatus { get; set; }
        public virtual DateTime? CreatedAt { get; set; }

        // Foreign Keys
        public int AnimalTypeId { get; set; }
        public int CharacteristicId { get; set; }
        public int BreedId { get; set; }
        public int SizeId { get; set; }
        public UserDto User { get; set; }

        // For Images
        public List<PetImageDTO> PetImages { get; set; }
    }
    public class PetImageDTO
    {
        public int PetId { get; set; }
        public string ImageName { get; set; }
        public string Image64 { get; set; }
    }

    public class FavoritePetDto
    {
        public int PetId { get; set; }
        public int UserId { get; set; }
    }

    public class ReportedPetDto
    {
        public int PetId { get; set; }
        public int Counter { get; set; }
    }
}
