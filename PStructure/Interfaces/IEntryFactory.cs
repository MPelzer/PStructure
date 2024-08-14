using System.Data.Common;
using PStructure.Interfaces;

namespace PStructure
{
    public interface IEntryFactory <T>
    {
        T CreateDefaultEntry();

        T CreateEntryByDbReader(DbDataReader reader);

        T CreateEntryByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey);

        T CloneEntry(T originalItem);
    }
}