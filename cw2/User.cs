using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace cw2
{
    class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public byte[] Salt { get; set; }

        public override string ToString()
        {
            return Id + ":" + Login + ":" + Password;
        }
    }
}
