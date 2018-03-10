using AppDto;


public class QueueDtoListener : BaseDtoListener<QueueDto>
{
    protected override void HandleNotify(QueueDto notify)
    {
        LoginManager.Instance.UpdateLoginQueueData(notify);
    }

}
