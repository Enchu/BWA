using BrosuhkaWinApp.Utils;
using System;

namespace BrosuhkaWinApp.Entities
{
    /// <summary>
    /// Various database functions.
    /// </summary>
    public class DbUtils
    {
        /// <summary>
        /// Browushka database connection.
        /// </summary>
        public static BrowushkaEntities Db { get; set; }

        /// <summary>
        /// Static <see cref="DbUtils"/> constructor.
        /// </summary>
        static DbUtils()
        {
            Db = new BrowushkaEntities();
        }

        /// <summary>
        /// Roll-backs database state.
        /// </summary>
        public static void RollBack()
        {
            Db?.Dispose();
            Db = new BrowushkaEntities();
        }

        /// <summary>
        /// Exception-safe save.
        /// </summary>
        public static bool SafeSave()
        {
            try
            {
                Db.SaveChanges();
                Msg.ShowInfo("Изменения успешно сохранены!");

                return true;
            }
            catch (Exception ex)
            {
                Msg.ShowError($"Произошла ошибка во время сохранения: {ex}");

                return false;
            }
        }
    }
}
