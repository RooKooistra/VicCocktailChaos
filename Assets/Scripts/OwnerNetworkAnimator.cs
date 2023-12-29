using Unity.Netcode.Components;

public class OwnerNetworkAnimator : NetworkAnimator
{
    /// <summary>
    /// Used to override Server for client/host game architecture
    /// </summary>
    /// <returns></returns>
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}