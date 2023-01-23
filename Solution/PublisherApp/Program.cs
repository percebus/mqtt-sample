using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace ConsoleApp
{
    internal class Program
    {
        // TODO move all of this to settings
        protected static string MQTT_BROKER_ADDRESS = Environment.GetEnvironmentVariable("MQTT_BROKER_ADDRESS") ?? "127.0.0.1";
        protected static int MQTT_BROKER_PORT = 1883;
        protected static IPAddress IPAddress = IPAddress.Parse(MQTT_BROKER_ADDRESS);
        protected static Guid Id = Guid.NewGuid();
        protected static string TOPIC = Environment.GetEnvironmentVariable("TOPIC") ?? "home/MyFirstMQTT/ConsoleApp";
        protected static string USER_NAME = Environment.GetEnvironmentVariable("USER_NAME");
        protected static string USER_PASSWORD = Environment.GetEnvironmentVariable("USER_PASSWORD");
        protected static bool SECURE = false;
        protected static bool RETAIN = true;
        protected static int SECONDS = 1000;
        //protected static int INTERVAL = 1 * SECONDS;
        protected static int INTERVAL = 300;

        //protected static MqttClient MqttClient = new MqttClient(IPAddress); // XXX DEPRECATED?
        protected static MqttClient MqttClient = new MqttClient(
            MQTT_BROKER_ADDRESS, MQTT_BROKER_PORT, SECURE,
            null, null, 
            MqttSslProtocols.TLSv1_2); 

        static ushort Publish(string topic, byte[] message) {
            return Program.MqttClient.Publish(
                topic,
                message,
                //MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                RETAIN);
        }

        static ushort Publish(string topic, string message) {
            Console.WriteLine("Sending message: '{0}'", message);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Publish(topic, bytes);
        }


        static void Main(string[] args)
        {
            string clientId = Id.ToString();
            if (String.IsNullOrEmpty(USER_NAME))
            {
                Program.MqttClient.Connect(clientId);
            }
            else 
            {
                Program.MqttClient.Connect(
                    clientId,
                    USER_NAME, USER_PASSWORD);
            }

            int i = 0;
            var oRandom = new Random();
            while (true) {
                i++;
                var oJObject = new JObject();
                    oJObject["id"] = i;
                    oJObject["deviceId"] = "bananas";
                    oJObject["version"] = 4;
                    oJObject["constant"] = 2;
                    oJObject["random"] = oRandom.Next(10);

                string jsonString = oJObject.ToString();
                Publish(TOPIC, jsonString);
                int ms = 1 * SECONDS;
                Thread.Sleep(INTERVAL);
            }
        }
    }
}
