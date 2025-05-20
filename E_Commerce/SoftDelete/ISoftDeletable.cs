namespace E_Commerce.SoftDelete
{
    public interface ISoftDeletable
    { 
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }

        public void Delete();

        public void UndoDelete();
        
    }
}
