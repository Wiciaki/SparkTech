namespace SparkTech.SDK.API.Fragments
{
    using SharpDX;

    public interface IGameInterface
    {
        void ChatShow(string text);

        void ChatPrint(string text);

        bool IsShopOpen();

        bool IsChatOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector3 pos);
    }
}