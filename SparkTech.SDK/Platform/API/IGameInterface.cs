namespace SparkTech.SDK.Platform.API
{
    using SharpDX;

    public interface IGameInterface
    {
        void ChatShow(string text);

        void ChatPrint(string text);

        bool IsShopOpen();

        bool IsChatOpen();

        Point CursorPosition();
    }
}