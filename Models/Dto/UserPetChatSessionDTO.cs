using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Dto
{
    public class UserPetChatSessionDTO
    {
        public int Id { get; set; }
        public UserDto User1 { get; set; }
        public UserDto User2 { get; set; }
        public PetDTO Pet { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DataCreate
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public UserDto UserPet
        {
            get
            {
                if (Pet.User.Id == User1.Id)
                {
                    return User1;
                }
                else
                {
                    return User2;
                }
            }
        }
        public UserDto UserNotPet
        {
            get
            {
                if (Pet.User.Id != User1.Id)
                {
                    return User1;
                }
                else
                {
                    return User2;
                }
            }
        }
    }

    public class UserPetChatDTO
    {
        public int Id { get; set; }
        public int UserPetChatSessionId { get; set; }
        public UserDto SenderUser { get; set; }
        public string? Text { get; set; }
        public string? Image64 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ChatMessages
    {
        public int SessionId { get; set; }
        public string Text { get; set; }
        public bool IsSentByMe { get; set; }
        public Color CorSender { get; set; }
        public LayoutOptions PositionSender { get; set; }
        public string SenderUser { get; set; }
    }
    public class ChatMessageDTO
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public int SessionId { get; set; } // Adicionado SessionId
        public int? PetId { get; set; }
        public int? ProductId { get; set; }
        public string? Text { get; set; }
        public string? Image64 { get; set; }
        public int SenderUser { get; set; }
        public int ReceiverUser { get; set; }
    }
}
