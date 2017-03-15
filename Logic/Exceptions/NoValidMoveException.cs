using System;

namespace Backgammon.Logic.Exceptions
{
    class NoValidMoveException : Exception
    {
        public NoValidMoveException()
        {

        }

        public NoValidMoveException(String message)
            :base(message)
        {

        }
    }
}
