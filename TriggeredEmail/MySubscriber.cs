namespace TriggeredEmail
{
    public class MySubscriber
    {
        public MySubscriber(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public string Nome { get; }
        public string Email { get; }
    }
}
