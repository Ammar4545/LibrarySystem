using System.ComponentModel.DataAnnotations;

namespace Library.PL.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
