namespace TModLoaderMaintainer.Models.DataTransferObjects
{
    public class ServerActionDto<TActionType>
    {
        public Queue<TActionType> Actions { get; set; }

        public ServerActionDto()
        {
            Actions = new Queue<TActionType>();
        }

        public void ClearActions()
        {
            if (Actions.Any())
                Actions.Clear();
        }
    }
}
