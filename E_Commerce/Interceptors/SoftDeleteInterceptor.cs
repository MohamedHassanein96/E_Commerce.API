using E_Commerce.SoftDelete;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace E_Commerce.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
                return new ValueTask<InterceptionResult<int>>(result);

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is null || entry.State != EntityState.Deleted || !(entry.Entity is ISoftDeletable entity))
                    continue;

                entry.State = EntityState.Modified;
                entity.Delete();

            }
            return new ValueTask<InterceptionResult<int>>(result);
        }
    }
}
