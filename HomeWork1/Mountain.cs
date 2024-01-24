using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork1
{
    public class Mountain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public string Country { get; set; }
        public string ToStringSql() => $"('{Name}', {Height}, '{Country}')";
    }
}
