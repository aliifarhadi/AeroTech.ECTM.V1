using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.SourceFileAggregate
{
    public sealed class SourceFileRepository : ISourceFileRepository
    {
        private readonly EctmDbContext _dbContext;

        public SourceFileRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(SourceFile sourceFile, CancellationToken cancellationToken = default)
            => await _dbContext.SourceFiles.AddAsync(sourceFile, cancellationToken);

        public Task<SourceFile?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.SourceFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(sourceFile => sourceFile.Id == id, cancellationToken);

        public async Task<IReadOnlyList<SourceFile>> ListBySha256Async(IReadOnlyCollection<string> sha256s, CancellationToken cancellationToken = default)
            => await _dbContext.SourceFiles
                .AsNoTracking()
                .Where(sourceFile => sha256s.Contains(sourceFile.Sha256))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<SourceFile>> ListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
            => await _dbContext.SourceFiles
                .AsNoTracking()
                .Where(sourceFile => ids.Contains(sourceFile.Id))
                .ToListAsync(cancellationToken);

        public async Task<byte[]?> GetBytesAsync(long id, CancellationToken cancellationToken = default)
        {
            var compressedBytes = await _dbContext.Set<SourceFileContent>()
                .AsNoTracking()
                .Where(content => content.Id == id)
                .Select(content => content.CompressedBytes)
                .FirstOrDefaultAsync(cancellationToken);

            if (compressedBytes is null)
                return null;

            return SourceFileContent.Unpack(compressedBytes);
        }
    }
}
