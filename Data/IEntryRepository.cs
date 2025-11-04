// interface because during development we have an in-memory repository and during production
// we will be using the sqlite one - so we need the repositories to implement this interface

using PWMan.Core;

namespace PWMan.Data;

public interface IEntryRepository
{
    void SaveEntry(Entry entry);
    Entry? GetEntry(Guid id);
    List<Entry> GetAllEntries();
    void DeleteEntry(Guid id);
    void SaveAllEntries(List<Entry> entries);
}
