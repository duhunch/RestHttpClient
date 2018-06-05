namespace Dragos.Net.Client
{
    public interface IAuthenticator : IRequestObserver
    {
        string CredentialName { get; }

        void Apply(WebClient webClient);
    }
}
