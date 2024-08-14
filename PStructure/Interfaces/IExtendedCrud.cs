using System.Collections.Generic;
using PStructure.Interfaces;

namespace PStructure
{
    public interface IExtendedCrud <T>
    {
        void InsertRangeByPrimaryKeys(out DbCom dbCom);

        void InsertByPrimaryKey(out DbCom dbCom);
    }
}