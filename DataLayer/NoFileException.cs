using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.DataLayer
{
    class NoFileException : Exception
    {
        public NoFileException()
        {

        }

        public NoFileException(String message)
            : base(message)
        {
        }
       
    }
}
