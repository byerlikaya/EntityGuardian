using DataAuditing.Storages.SqlServer;

namespace EntityGuardian.Storages
{
    public class Storage
    {
        private readonly string _connectionString;

        public Storage(string connectionString)
        {
            _connectionString = connectionString;
            Initialize();
        }

        public void Initialize()
        {
            SqlServerInstaller sqşServerInstaller = new(_connectionString);
            sqşServerInstaller.Install();
        }
    }
}
