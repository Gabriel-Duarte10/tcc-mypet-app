using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tcc_mypet_app.Models.Dto
{
    public class PetDTO : INotifyPropertyChanged
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
        public AnimalTypeDTO AnimalType { get; set; }
        public int CharacteristicId { get; set; }
        public CharacteristicDTO Characteristic { get; set; }
        public int BreedId { get; set; }
        public BreedDTO Breed { get; set; }
        public int SizeId { get; set; }
        public SizeDTO Size { get; set; }
        public UserDto User { get; set; }

        // For Images
        private List<PetImageDTO> _petImages;
        public List<PetImageDTO> PetImages
        {
            get { return _petImages; }
            set
            {
                if (_petImages != value)
                {
                    _petImages = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string GenderText
        {
            get
            {
                if (Gender == true)
                {
                    return "Macho";
                }
                else
                {
                    return "Fêmea";
                }
            }
        }
        public string Idade
        {
            get
            {
                var idade = DateTime.Now.Year - BirthYear;
                if (DateTime.Now.Month < BirthMonth)
                {
                    idade--;
                }
                return idade.ToString();
            }

        }
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
