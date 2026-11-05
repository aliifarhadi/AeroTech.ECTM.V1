using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class SourceRows
    {
        public static IEnumerable<IReadOnlyList<string>> Read(string fileName, byte[] content, char delimiter, long maximumPartBytes)
        {
            if (SourceFileKinds.IsCsv(fileName))
                return CsvRows.Read(content, delimiter);

            if (SourceFileKinds.IsXlsx(fileName))
                return XlsxRows.Read(content, maximumPartBytes);

            throw ExceptionFactory.ImportFileTypeNotSupported(fileName);
        }
    }
}
