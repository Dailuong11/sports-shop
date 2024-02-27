using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public partial class RefreshToken
    {
        public int AccountId { get; set; }
        public string TokenId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
