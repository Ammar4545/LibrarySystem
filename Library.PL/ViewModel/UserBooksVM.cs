using Library.DAL.Entities;

namespace Library.PL.ViewModel
{
    public class UserBooksVM
    {
        public IEnumerable<Books> Books { get; set; }
    }
}
