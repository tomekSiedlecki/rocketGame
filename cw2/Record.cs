using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class Record
    {
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            string result = User.Login + ": " + Score;
            return result;
        }
    }
}
