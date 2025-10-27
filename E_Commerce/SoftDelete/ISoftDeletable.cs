namespace E_Commerce.SoftDelete
{
    public interface ISoftDeletable
    { 
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        string? DeletedBy { get; set; } // 🟢 مين اللي مسح

    }
}
