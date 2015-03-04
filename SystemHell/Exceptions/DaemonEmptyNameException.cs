using System;

namespace SystemHell.Exceptions
{
    [Serializable]
    public class DaemonEmptyNameException : DaemonHostException
    {      
        public DaemonEmptyNameException()
            : base("You can not start because one of the modules has no name. Please verify that all modules have the Name attribute with a value set.")
        {
        }      
    }
}