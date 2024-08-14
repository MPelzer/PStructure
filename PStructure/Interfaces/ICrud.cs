using System.Collections.Generic;

namespace PStructure.Interfaces
{
    /// <summary>
    /// Basisinterface 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrud <T> 
    {
        IEnumerable<T> ReadRangeByPrimaryKey(out DbCom dbCom);
        
        T ReadByPrimaryKey(out DbCom dbCom);

        IEnumerable<T> UpdateRangeByPrimaryKey(out DbCom dbCom);

        T UpdateByPrimaryKey(out DbCom dbCom);
        
        IEnumerable<T> UpdateRangeByInstances(out DbCom dbCom);

        T UpdateByInstance(out DbCom dbCom);
        
        IEnumerable<T> InsertRangeByInstances(out DbCom dbCom);

        T InsertByInstance(out DbCom dbCom);

        T DeleteByPrimaryKey(out DbCom dbCom);

        IEnumerable<T> DeleteRangeByPrimaryKeys(out DbCom dbCom);

        IEnumerable<T> ReadAll(out DbCom dbCom);
    }
}