using SqlKata.Compilers;

namespace Avalanche.DataSource.Pgsql
{
    public class AvalanchePostgresCompiler : PostgresCompiler
    {
        public AvalanchePostgresCompiler()
        {
            //
            // Don't add " to the Table and Column names
            OpeningIdentifier = string.Empty;
            ClosingIdentifier = string.Empty;
        }

        public override string WrapValue(string value)
        {
            return value;
        }
    }
}
