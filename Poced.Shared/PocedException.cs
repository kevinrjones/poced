using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Poced.Shared
{
    [Serializable]
    public class PocedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PocedException()
        {
        }

        public PocedException(string message) : base(message)
        {
        }

        public PocedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PocedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
