using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Request
{
    public class UserPetChatSessionRequest
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public int PetId { get; set; }
    }
}
