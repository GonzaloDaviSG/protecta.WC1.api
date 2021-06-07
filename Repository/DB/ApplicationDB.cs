using Microsoft.EntityFrameworkCore;

namespace protecta.WC1.api.Repository.DB
{
    public class ApplicationDB
    {
        public static DbContextOptions<ApplicationDbContext> UsarPrincipal()
        {
            return UsarOracleGestor();
        }
        public static DbContextOptions<ApplicationDbContext> UsarPrincipalGestor()
        {
            return UsarOracleGestor();
        }

        public static DbContextOptions<ApplicationDbContext> UsarMemoria()
        {
            var OptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OptionsBuilder.UseInMemoryDatabase("DBTemp");
            return OptionsBuilder.Options;
        }

        public static DbContextOptions<ApplicationDbContext> UsarOracle()
        {
            //  string connectionString = Utils.Config.AppSetting["ConnectionString:LAFT"];
            string connectionString = Utils.Config.AppSetting["ConnectionString:LAFT"];

            var OptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OptionsBuilder.UseOracle(connectionString, b => b.UseOracleSQLCompatibility("11"));
            return OptionsBuilder.Options;
        }
        public static DbContextOptions<ApplicationDbContext> UsarOracleGestor()
        {
            string connectionString = Utils.Config.AppSetting["ConnectionString:TIMEP"];
            var OptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OptionsBuilder.UseOracle(connectionString, b => b.UseOracleSQLCompatibility("11"));
            return OptionsBuilder.Options;
        }
    }
}
