namespace E_Commerce.ISoftDeletable
{
    public interface ISoftDeletable
    { 
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }

        public void Delete();

        public void UndoDelete();
        
    }
}
