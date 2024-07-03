namespace GameLogic.AssetsModule
{
    public interface ICommandHandler<in T>
    {
        void Handle(T cmd);
    }
}