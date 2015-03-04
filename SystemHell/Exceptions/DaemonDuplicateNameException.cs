namespace SystemHell.Exceptions
{
    public class DaemonDuplicateNameException : DaemonHostException
    {
        public DaemonDuplicateNameException(string name)
            : base("several modules named \"" + name + "\" are present in the configuration file. The module name must be unique")
        {
        }
    }
}