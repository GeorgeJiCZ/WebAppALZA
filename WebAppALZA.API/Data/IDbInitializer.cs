
namespace WebAppALZA.API.Data
{
    public interface IDbInitializer
    {
        /// <summary>
        /// Applies any pending migrations for the context to database.
        /// Will create the database if it does not already exist.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds default values to database
        /// </summary>
        void SeedData();
    }
}
