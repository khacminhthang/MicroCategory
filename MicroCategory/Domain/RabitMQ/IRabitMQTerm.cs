namespace MicroCategory.Domain.RabitMQ
{
    public interface IRabitMQTerm
    {
        public void SendProductMessage<T>(T message);
    }
}
