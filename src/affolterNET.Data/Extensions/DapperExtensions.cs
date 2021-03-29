using Dapper;

namespace affolterNET.Data.Extensions
{
    public static class DapperExtensions
    {
        /// <summary>
        ///   Dies wird benötigt, wenn eine Abfrage mit QueryMultiple() erstellt wird, und nach dem ersten Read()
        ///   nicht mehr weitergelesen wird.
        ///   Wenn dieser Aufruf nicht erfolgt, so wird eine Exception ausgelöst, weil der DataReader nicht
        ///   geschlossen wurde.
        /// </summary>
        /// <param name="gridReader"></param>
        public static void EndQueryMultiple(this SqlMapper.GridReader gridReader)
        {
            if (gridReader.Command == null)
            {
                // alles ok
                return;
            }

            while (!gridReader.IsConsumed)
            {
                gridReader.Read();
            }
        }
    }
}