using System;
using System.Configuration;
using System.ServiceModel;
using TriggeredEmail.MktCloudSoapAPI;

namespace TriggeredEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://[YOUR_SUBDOMAIN].soap.marketingcloudapis.com/etframework.wsdl
            //https://[YOUR_SUBDOMAIN].soap.marketingcloudapis.com/Service.asmx

            string nome, email;

            Console.WriteLine("## Bem-vindo ao SorteOnline! ##");
            Console.WriteLine();

            Console.Write("Digite seu nome: ");
            nome = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Digite seu e-mail: ");
            email = Console.ReadLine();
            Console.WriteLine();

            MySubscriber subscriber = new MySubscriber(nome, email);

            //Criar TriggeredSendDefinition

            //Criar Subscribers

            //Criar TriggeredSend
            TriggeredSend triggeredSend = GetTriggeredSend(subscriber);

            string reqId, status;
            SoapClient client = GetSoapClient();

            APIObject[] objects = new APIObject[1] { triggeredSend };

            try
            {
                CreateResult[] response = client.Create(new CreateOptions(), objects, out reqId, out status);

                foreach (var resp in response)
                {
                    Console.WriteLine("Status Message: " + resp.StatusMessage);
                    Console.WriteLine();
                }

                Console.WriteLine("Obrigado por se registrar! Em breve você receberá um e-mail de confirmação.");
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private static TriggeredSend GetTriggeredSend(MySubscriber subscriber)
        {
            TriggeredSend triggeredSend = new TriggeredSend();
            triggeredSend.TriggeredSendDefinition = GetTriggeredSendDefinition();
            triggeredSend.Subscribers = GetSubscriberList(new MySubscriber[] { subscriber });

            return triggeredSend;
        }

        private static SoapClient GetSoapClient()
        {
            //Criar HttpBasicBiding
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "SOAPBinding";
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.OpenTimeout = new TimeSpan(0, 5, 0);
            binding.CloseTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.MaxReceivedMessageSize = 1000000;

            //Criar EndPoint
            EndpointAddress endpoint = new EndpointAddress("https://[YOUR_SUBDOMAIN].soap.marketingcloudapis.com/Service.asmx");

            //Criar SoapClient
            SoapClient client = new SoapClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["sfUser"];
            client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["sfPass"];

            return client;
        }

        private static TriggeredSendDefinition GetTriggeredSendDefinition()
        {
            TriggeredSendDefinition definition = new TriggeredSendDefinition();
            definition.CustomerKey = "TrigConfirmacaoCadastro";

            return definition;
        }

        private static Subscriber[] GetSubscriberList(MySubscriber[] subscribers)
        {
            Subscriber[] subs = new Subscriber[1];

            foreach (var sub in subscribers)
            {
                Subscriber subscriber = new Subscriber
                {
                    SubscriberKey = sub.Email,
                    EmailAddress = sub.Email
                };

                MktCloudSoapAPI.Attribute[] attributes = new MktCloudSoapAPI.Attribute[3]
                {
                    new MktCloudSoapAPI.Attribute
                    {
                        Name = "NOME",
                        Value = sub.Nome
                    },
                    new MktCloudSoapAPI.Attribute
                    {
                        Name = "DATA_CADASTRO",
                        Value = DateTime.Now.ToString("yyyy/MM/dd")
                    },
                    new MktCloudSoapAPI.Attribute
                    {
                        Name = "CUPOM",
                        Value = "ABCDEF"
                    }
                };

                subscriber.Attributes = attributes;

                subs[0] = subscriber;
            }

            return subs;
        }
    }
}
