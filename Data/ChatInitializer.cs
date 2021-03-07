namespace ChatApp.Data
{
    public class ChatInitializer
    {
        public static void Initialize(ChatContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
