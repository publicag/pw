namespace PWSerwer.Client
{
    public interface IClient
    {
        void Finished();
        void FileProcessed(string fileName);
    }
}
