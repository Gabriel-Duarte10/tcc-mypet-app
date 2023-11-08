using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Request
{
    public class ResetPasswordInitiateRequest
    {
        public string Email { get; set; }
    }
    public class CodePassword
    {
        public string Email { get; set; }
        public int CellphoneCode { get; set; }
    }
    public class ResetPasswordCompleteRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }   
}
