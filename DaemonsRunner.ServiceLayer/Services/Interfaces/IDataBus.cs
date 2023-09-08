namespace DaemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IDataBus
    {
        IDisposable RegisterHandler<T>(Action<T> handler);

        void Send<T>(T message);
    }
}
